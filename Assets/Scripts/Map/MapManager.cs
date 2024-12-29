using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase walkableTile; 
    public TileBase nonWalkableTile;
    public TileBase startTile;
    public TileBase endTile;

    public GameObject spawnerPrefab;
    

    public Vector3Int mazeOrigin = Vector3Int.zero;

    public int regionWidth = 10;
    public int regionHeight = 10;

    private Dictionary<(int, int), CellT> globalMap = new Dictionary<(int, int), CellT>();
    private Dictionary<(int, int), Region> globalRegionMap = new Dictionary<(int, int), Region>();
    private Dictionary<(int, int), GameObject> spawnerPositions = new Dictionary<(int, int), GameObject>();

    void Start()
    {

        // 0 = Left, 1 = Right, 2 = Bottom, 3 = Top
        // This will be used to create start map
        CreateEmptyRegion(0, 0, regionWidth, regionHeight);
        CreateInitialRegion(1, 0, regionWidth, regionHeight,0);
        CreateInitialRegion(-1, 0, regionWidth, regionHeight, 1);
        CreateInitialRegion(0, 1, regionWidth, regionHeight, 2);
        CreateInitialRegion(0, -1, regionWidth, regionHeight, 3);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);           
            Plane tilemapPlane = new Plane(Vector3.up, Vector3.zero); 

            if (tilemapPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPosition = ray.GetPoint(enter); 
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

                (int regionX, int regionY) = (
                    Mathf.FloorToInt((float)cellPosition.x / regionWidth),
                    Mathf.FloorToInt((float)cellPosition.y / regionHeight)
                );

                Debug.Log(cellPosition);

                Debug.Log($"Clicked on region at ({regionX}, {regionY})");

                ExpandRegion(regionX, regionY);
   
            }       
        }   
    }

    private void CreateEmptyRegion(int regionX, int regionY, int width, int height)
    {
        Region initialRegion = new Region(regionX, regionY, width, height);
        
        AddRegionToMap(initialRegion);
        DrawRegionOnTilemap(initialRegion);
    }
    private void CreateInitialRegion(int regionX, int regionY, int width, int height, int startSide)
    {
        
        Region initialRegion = new Region(regionX, regionY, width, height);
        CellT startCell = null;
        int startX = 0, startY = 0;
  
        switch (startSide)
        {
            case 0: // Left
                startX = 0;
                startY = height / 2;
                startCell = new CellT(startX, startY) { IsOpenLeft = true }; // Connect this cell to the right
                break;
            case 1: // Right
                startX = width - 1;
                startY = height / 2;
                startCell = new CellT(startX, startY) { IsOpenRight = true }; // Connect this cell to the left
                break;
            case 2: // Bottom
                startX = width / 2;
                startY = 0;
                startCell = new CellT(startX, startY) { IsOpenDown = true }; // Connect this cell to the top
                break;
            case 3: // Top
                startX = width / 2;
                startY = height - 1;
                startCell = new CellT(startX, startY) { IsOpenUp = true }; // Connect this cell to the bottom
                break;
            default:
                Debug.LogError($"Invalid startSide: {startSide}");
                return;
        }

        // Add the start cell to both the list and the nested array. important
        initialRegion.startCells.Add(startCell);
        initialRegion.Cells[startX, startY] = startCell;

        // genreate the region now
        initialRegion.GeneratePath(globalRegionMap);
        AddRegionToMap(initialRegion);
        DrawRegionOnTilemap(initialRegion);
        AddRegionSpawners(initialRegion);
    }


    // adds each cell of a region to the global map dictionary and adds region to globalregion map too. IMPORTANT!!, 
    private void AddRegionToMap(Region region)
    {
        for (int x = 0; x < region.Width; x++)
        {
            for (int y = 0; y < region.Height; y++)
            {
                var globalCoords = region.LocalToGlobal(x, y);
                globalMap[globalCoords] = region.Cells[x, y];
            }
        }
        globalRegionMap[(region.RegionX, region.RegionY)] = region;
    }

    public void AddRegionSpawners(Region region)
    {
        Dictionary<(int x, int y), GameObject> spawners = new Dictionary<(int x, int y), GameObject>();

        // Iterate through end cells to place spawners just outside the region
        foreach (CellT endCell in region.endCells)
        {
            Vector2Int spawnerPosition = Vector2Int.zero;

            // Calculate the spawner position based on the end cell's direction
            if (endCell.X == 0) // Left edge
            {
                spawnerPosition = new Vector2Int((region.RegionX * region.Width) -1, endCell.Y + (region.RegionY * region.Height));
            }
            else if (endCell.X == region.Width - 1) // Right edge
            {
                spawnerPosition = new Vector2Int(((region.RegionX +1) * region.Width), endCell.Y + (region.RegionY * region.Height));
            }
            else if (endCell.Y == 0) // Bottom edge
            {
                spawnerPosition = new Vector2Int(endCell.X + (region.RegionX * region.Width), (region.RegionY* region.Height)-1);
            }
            else if (endCell.Y == region.Height - 1) // Top edge
            {
                spawnerPosition = new Vector2Int(endCell.X + (region.RegionX * region.Width), ((region.RegionY + 1) * region.Height));
            }

            

            // Instantiate the spawner GameObject (you can handle pooling here if needed)
            GameObject spawner = GameObject.Instantiate(spawnerPrefab, tilemap.CellToWorld(new Vector3Int (spawnerPosition.x,spawnerPosition.y,0)), Quaternion.identity);

            // Add the spawner to the dictionary
            spawnerPositions[(spawnerPosition.x, spawnerPosition.y)] = spawner;
            
        }

       
    }

    
    void RemoveSpawnersInExpandedRegion(Region region)
    {
        
        List<(int x,int y)> spawnersToRemove = new List<(int x, int y)>();

        int regionLeft = (region.RegionX * region.Width) -1;
        int regionRight = (region.RegionX + 1) * region.Width;
        int regionBottom = region.RegionY * region.Height -1;
        int regionTop = (region.RegionY + 1) * region.Height;

        // when iterating dictionaries we can modify so thats why we make two.
        foreach (var spawnerEntry in spawnerPositions)
        {
            (int x, int y) = spawnerEntry.Key;

            if (x >= regionLeft && x <= regionRight && y >= regionBottom && y <= regionTop)
            {
                Debug.Log($"Spawner at ({x}, {y}) marked for removal");
                spawnersToRemove.Add((x, y));
            }
            else
            {
                Debug.Log($"Spawner at ({x}, {y}) is outside expanded region borders");
            }


        }

        foreach (var spawnerPosition in spawnersToRemove)
        {
            GameObject spawner = spawnerPositions[spawnerPosition];
            Destroy(spawner); // Remove from the scene
            spawnerPositions.Remove(spawnerPosition); // Remove from dictionary
        }
    }


    /* Expand Region is called when a region is clicked on and it is passed the region coords
     * What it does is manages the logic between regions and how they connect to each other
     * it checks neihbours and sees if they have any paths that lead into the one the player wants to unlock
     * if they do then we calculate this regions startcells
     * once the regions startcells are assigned, we send it to be have its path generated
     * if thats succesful, we then add the region to the dictionary of cells and also regions
     * and then draw it on the tilemap   
     */

    public void ExpandRegion(int newRegionX, int newRegionY)
    {
        if (globalRegionMap.ContainsKey((newRegionX, newRegionY)))
        {
            Debug.LogWarning($"Region at ({newRegionX}, {newRegionY}) already exists.");
            return;
        }

        Region newRegion = new Region(newRegionX, newRegionY, regionWidth, regionHeight);
        
        List<Region> neighbouringRegions = GetNeighbouringRegions(newRegion);

        
        if (!(neighbouringRegions.Count > 0))
        {
            Debug.Log("you clicked on an unlockable region");
            return;
        }
        else if (neighbouringRegions.Count == 4)
        {
            Debug.Log("you landlocked cant open this or this is already unlocked");
            return;
        }
        bool foundMatchingEndCell = false;
        
        foreach (Region neighbour in neighbouringRegions)
        { 
            
            (int x, int y) direction = (neighbour.RegionX - newRegionX, neighbour.RegionY - newRegionY);

            List<CellT> matchingEndCells = FindEndCellFromNeighbour(neighbour, direction);

            if (matchingEndCells.Count == 0)
            {    
                // only happens when no paths lead to this region so we cant have this unlockable
                continue;
            }

            foundMatchingEndCell = true;
            
            foreach (CellT matchingEndCell in matchingEndCells)
            {
                CellT newStartCell = null;

                if (direction == (1, 0)) // Right
                {
                    newStartCell = new CellT(regionWidth - 1, matchingEndCell.Y);
                    matchingEndCell.IsOpenLeft = true;
                    newStartCell.IsOpenRight = true;
                }
                else if (direction == (-1, 0)) // Left
                {
                    newStartCell = new CellT(0, matchingEndCell.Y);
                    matchingEndCell.IsOpenRight = true;
                    newStartCell.IsOpenLeft = true;
                }
                else if (direction == (0, 1)) // Up
                {
                    newStartCell = new CellT(matchingEndCell.X, regionHeight - 1);
                    matchingEndCell.IsOpenDown = true;
                    newStartCell.IsOpenUp = true;
                }
                else if (direction == (0, -1)) // Down
                {
                    newStartCell = new CellT(matchingEndCell.X, 0);
                    matchingEndCell.IsOpenUp = true;
                    newStartCell.IsOpenDown = true;
                }

                if (newStartCell != null)
                {
                    newRegion.startCells.Add(newStartCell);

                    // Assign to Cells array
                    newRegion.Cells[newStartCell.X, newStartCell.Y] = newStartCell;

                    Debug.Log($"Added Start Cell: X = {newStartCell.X}, Y = {newStartCell.Y}");
                }

            }
        }

        if (!foundMatchingEndCell)
        {
            Debug.LogError($"No paths direct to this region, so we cant form a region here");
            return;
        }
        RemoveSpawnersInExpandedRegion(newRegion);
        newRegion.GeneratePath(globalRegionMap);
        
        AddRegionToMap(newRegion);
        DrawRegionOnTilemap(newRegion);  
        AddRegionSpawners(newRegion);
    }


    private List<Region> GetNeighbouringRegions(Region region)
    {
        List<Region> neighbours = new List<Region>();

        (int x, int y)[] offsets = new (int, int)[]
        {
        (-1, 0), // Left
        (1, 0),  // Right
        (0, -1), // Bottom
        (0, 1)   // Top
        };

        foreach (var (dx, dy) in offsets)
        {
            // get neihbour coords
            (int neighbourX, int neighbourY) = (region.RegionX + dx, region.RegionY + dy);

            // Check if the neighbor exists in the dictionary
            if (globalRegionMap.TryGetValue((neighbourX, neighbourY), out Region neighbourRegion))
            {
                neighbours.Add(neighbourRegion);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// It finds the neighbouring end cell that go into this region you want to unlock
    /// </summary>
    /// <param name="neighbourRegion"></param>
    /// <param name="direction"> The direction from the region you want to unlock , to its neighbouring regions</param>
    /// <returns></returns>
    private List<CellT> FindEndCellFromNeighbour(Region neighbourRegion, (int x, int y) direction)
    {
        List<CellT> matchingEndCells = new List<CellT>();

        //Debug.Log(region.endCells);
        foreach (var endCell in neighbourRegion.endCells)
        {
            //define local cell coords in this region
            int regionLeft = 0;
            int regionRight = regionWidth - 1;
            int regionBottom = 0;
            int regionTop = regionHeight - 1;
 

            // Check if the endCell is adjacent to the specified boundary
            if (direction == (1, 0) && endCell.X == regionLeft && !endCell.IsOpenLeft) // Right
            {
                matchingEndCells.Add(endCell);
            }
            else if (direction == (-1, 0) && endCell.X == regionRight && !endCell.IsOpenRight) // Left
            {
                matchingEndCells.Add(endCell);
            }
            else if (direction == (0, 1) && endCell.Y == regionBottom && !endCell.IsOpenDown) // Up
            {
                
                matchingEndCells.Add(endCell);
            }
            else if (direction == (0, -1) && endCell.Y == regionTop && !endCell.IsOpenUp) // Down
            {
                matchingEndCells.Add(endCell);
            }
        }
        return matchingEndCells; 
    }


    private void DrawRegionOnTilemap(Region region)
    {
        for (int x = 0; x < region.Width; x++)
        {
            for (int y = 0; y < region.Height; y++)
            {
                CellT cell = region.Cells[x, y];
                (int globalX, int globalY) = region.LocalToGlobal(x, y);

                Vector3Int cellPosition = mazeOrigin + new Vector3Int(globalX, globalY, 0);
                TileBase tileToUse;
               
                if (region.startCells.Contains(cell))
                {
                    tileToUse = startTile; 
                }
                else if (region.endCells.Contains(cell))
                {
                    tileToUse = endTile; 
                }
                else
                {
                    tileToUse = cell.IsWalkable ? walkableTile : nonWalkableTile;
                }
                tilemap.SetTile(cellPosition, tileToUse);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldPosition, 0.2f);
    }
}
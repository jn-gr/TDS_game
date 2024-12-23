using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase walkableTile; 
    public TileBase nonWalkableTile; 
    

    public Vector3Int mazeOrigin = Vector3Int.zero;

    public int regionWidth = 10;
    public int regionHeight = 10;

    private Dictionary<(int, int), CellT> globalMap = new Dictionary<(int, int), CellT>();
    private Dictionary<(int, int), Region> globalRegionMap = new Dictionary<(int, int), Region>();

    void Start()
    {
        // testing 
        CreateInitialRegion(0, 0, regionWidth, regionHeight);
        CreateInitialRegion(1, 0, regionWidth, regionHeight);
        CreateInitialRegion(0, 1, regionWidth, regionHeight);
        CreateInitialRegion(-1, 0, regionWidth, regionHeight);


    
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);           
            Plane tilemapPlane = new Plane(Vector3.up, Vector3.zero); // Adjust Vector3.up to match your tilemap's normal

            if (tilemapPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPosition = ray.GetPoint(enter); 
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

                (int regionX, int regionY) = (
                    Mathf.FloorToInt((float)cellPosition.x / regionWidth),
                    Mathf.FloorToInt((float)cellPosition.y / regionHeight)
                );

                Debug.Log(cellPosition);

                if (globalRegionMap.TryGetValue((regionX, regionY), out Region clickedRegion))
                {
                    Debug.Log($"Clicked on region at ({regionX}, {regionY})");

                    // this is where we expand regions
                    
                }
                else
                {
                    Debug.LogError($"No region found at ({regionX}, {regionY})");
                } 
                Debug.DrawLine(ray.origin, worldPosition, Color.red, 1f);
            }       
        }   
    }

    private void CreateInitialRegion(int regionX, int regionY, int width, int height)
    {
        Region initialRegion = new Region(regionX, regionY, width, height);
        globalRegionMap[(regionX,regionY)] = initialRegion;
        initialRegion.GeneratePath();
        AddRegionToMap(initialRegion);
        DrawRegionOnTilemap(initialRegion);

        //Debug.Log(initialRegion.startCell.X);
        //Region newregion = ExpandRegion(initialRegion, regionX, regionY+1);
        

        // going any other direction doesnt work right now, because the endcell is acctually the start for region unlocking to left
        // i will fix this
    }


    // adds each cell of a region to the global map dictionary . IMPORTANT!!, 
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
    }
    // newregion x and y will be given by calulating where the mouse clicks on the tilemap,
    // and then with logic you can calculate which region was clicked by dividng by width and height

    // this function needs to be changed, its better to not expand from a region but rather create region and then connect with adjacent.

    //the coordinates are checked with neighbouring regions, and checks if they have any endcells that point towards this region
    // if so then we create a startcell 
    // this should also work for if many paths lead to this region.
    // theoritcally, if 3 regions all path towards this one, it should lead all 3 paths to the only remaining region left
    public Region ExpandRegion(int newRegionX, int newRegionY)
    {
        Region newRegion = new Region(newRegionX, newRegionY, regionWidth, regionHeight);
        globalRegionMap[(newRegionX,newRegionY)] = newRegion;
        List<Region> neighbouringRegions = GetNeighbouringRegions(newRegion);

        foreach (Region neighbour in neighbouringRegions)
        {
            (int x, int y) direction = (newRegionX - neighbour.RegionX, newRegionY - neighbour.RegionY);

            CellT matchingEndCell = FindMatchingEndCell(neighbour, direction);
            if (matchingEndCell == null)
            {
                Debug.LogError($"No matching endCell found for direction: {direction}");
                return null;
            }

            if (direction == (1, 0)) // Right
            {
                newRegion.startCells.Add(new CellT(matchingEndCell.X + 1, matchingEndCell.Y));
                matchingEndCell.IsOpenRight = true;
                newRegion.startCells[^1].IsOpenLeft = true; 
            }
            else if (direction == (-1, 0)) // Left
            {
                newRegion.startCells.Add(new CellT(matchingEndCell.X - 1, matchingEndCell.Y));
                matchingEndCell.IsOpenLeft = true;
                newRegion.startCells[^1].IsOpenRight = true;
            }
            else if (direction == (0, 1)) // Up
            {
                newRegion.startCells.Add(new CellT(matchingEndCell.X, matchingEndCell.Y + 1));
                matchingEndCell.IsOpenUp = true;
                newRegion.startCells[^1].IsOpenDown = true; 
            }
            else if (direction == (0, -1)) // Down
            {
                newRegion.startCells.Add(new CellT(matchingEndCell.X, matchingEndCell.Y - 1));
                matchingEndCell.IsOpenDown = true;
                newRegion.startCells[^1].IsOpenUp = true;
            }



        }
        newRegion.GeneratePath();
        AddRegionToMap(newRegion);
        DrawRegionOnTilemap(newRegion);
        return newRegion;
    }


    private List<Region> GetNeighbouringRegions(Region region)
    {
        List<Region> neighbours = new List<Region>();

        // Define offsets for adjacent regions (left, right, top, bottom)
        (int x, int y)[] offsets = new (int, int)[]
        {
        (-1, 0), // Left
        (1, 0),  // Right
        (0, -1), // Bottom
        (0, 1)   // Top
        };

        foreach (var (dx, dy) in offsets)
        {
            // Calculate neighbor's coordinates
            (int neighbourX, int neighbourY) = (region.RegionX + dx, region.RegionY + dy);

            // Check if the neighbor exists in the dictionary
            if (globalRegionMap.TryGetValue((neighbourX, neighbourY), out Region neighbourRegion))
            {
                neighbours.Add(neighbourRegion);
            }
        }

        return neighbours;
    }
    private CellT FindMatchingEndCell(Region region, (int x, int y) direction)
    {
        //Debug.Log(region.endCells);
        foreach (var endCell in region.endCells)
        {
            if (direction == (1, 0) && !endCell.IsOpenRight) // Right
            {
                return endCell;
            }
            else if (direction == (-1, 0) && !endCell.IsOpenLeft) // Left
            {
                return endCell;
            }
            else if (direction == (0, 1) && !endCell.IsOpenUp) // Up
            {
                return endCell;
            }
            else if (direction == (0, -1) && !endCell.IsOpenDown) // Down
            {
                return endCell;
            }
        }
        return null; // No matching endCell found
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
                TileBase tileToUse = cell.IsWalkable ? walkableTile : nonWalkableTile;              
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

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

    void Start()
    {
        // testing 
        CreateInitialRegion(0, 0, regionWidth, regionHeight);
    }

    public void CreateInitialRegion(int regionX, int regionY, int width, int height)
    {
        Region initialRegion = new Region(regionX, regionY, width, height);
        initialRegion.GeneratePath();
        AddRegionToMap(initialRegion);
        DrawRegionOnTilemap(initialRegion);

        Debug.Log(initialRegion.startCell.X);
        Region newregion = ExpandRegion(initialRegion,regionX +1, regionY);
        Region newregion1 = ExpandRegion(newregion,regionX +2, regionY);
        Region newregion2 = ExpandRegion(newregion1, regionX + 3, regionY);
        
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
    public Region ExpandRegion(Region existingRegion, int newRegionX, int newRegionY)
    {
        // caclulate the direction between regions
        (int x, int y) direction = (newRegionX - existingRegion.RegionX, newRegionY - existingRegion.RegionY);

        Region newRegion = new Region(newRegionX, newRegionY, regionWidth, regionHeight);

        // Set startCell and endCell based on direction
        if (direction == (1, 0)) // Right
        {
            newRegion.startCell = new CellT(existingRegion.endCell.X + 1, existingRegion.endCell.Y);
            // i probably dont need isopenright and all that, but for now ill keep
            existingRegion.endCell.IsOpenRight = true;
            newRegion.startCell.IsOpenLeft = true;
        }
        else if (direction == (-1, 0)) // Left
        {
            newRegion.startCell = new CellT(existingRegion.endCell.X - 1, existingRegion.endCell.Y);
            existingRegion.endCell.IsOpenLeft = true;
            newRegion.startCell.IsOpenRight = true;
        }
        else if (direction == (0, 1)) // Up
        {
            newRegion.startCell = new CellT(existingRegion.endCell.X, existingRegion.endCell.Y + 1);
            existingRegion.endCell.IsOpenUp = true;
            newRegion.startCell.IsOpenDown = true;
        }
        else if (direction == (0, -1)) // Down
        {
            newRegion.startCell = new CellT(existingRegion.endCell.X, existingRegion.endCell.Y - 1);
            existingRegion.endCell.IsOpenDown = true;
            newRegion.startCell.IsOpenUp = true;
        }
        else
        {
            Debug.LogError($"wrong direction: {direction}");
        }

       
        newRegion.GeneratePath();

        // add the new region to the map and draw it
        AddRegionToMap(newRegion);
        DrawRegionOnTilemap(newRegion);
        return newRegion;
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
}

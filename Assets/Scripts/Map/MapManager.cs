using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public GameObject walkableCellPrefab;
    public GameObject nonWalkableCellPrefab;

    public float cellSize = 1;

    public Vector3 mazeOrigin = Vector3.zero;

    private Dictionary<(int, int), CellT> globalMap = new Dictionary<(int, int), CellT>();

    void Start()
    {
        // Example: create an initial region at (0,0) with size 10x10
        CreateInitialRegion(0, 0, 10, 10);
    }
    public void CreateInitialRegion(int originX, int originY, int width, int height)
    {
        Region initialRegion = new Region(originX, originY, width, height);
        initialRegion.GeneratePath();
        AddRegionToMap(initialRegion);
        InstantiateRegionCells(initialRegion);
        
    }

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

    public void ExpandRegion(int attachGlobalX, int attachGlobalY, int newWidth, int newHeight, int offsetX, int offsetY)
    {
        int newOriginX = attachGlobalX + offsetX;
        int newOriginY = attachGlobalY + offsetY;

        Region newRegion = new Region(newOriginX, newOriginY, newWidth, newHeight);
        newRegion.GeneratePath();

        ConnectRegions(attachGlobalX, attachGlobalY, newRegion);

        AddRegionToMap(newRegion);
        InstantiateRegionCells(newRegion);
    }

    private void ConnectRegions(int attachGlobalX, int attachGlobalY, Region newRegion)
    {
        // Simplified version of connecting one cell - assume that (attachGlobalX, attachGlobalY)
        // lines up directly with one of newRegion's cells. We’ll choose the cell at (0,0) for simplicity.
        // In a real scenario, you'd pick the correct boundary cell.

        int localX = attachGlobalX - newRegion.OriginX;
        int localY = attachGlobalY - newRegion.OriginY;

        var attachingCell = newRegion.GetCellLocal(localX, localY);
        var existingCell = globalMap[(attachGlobalX, attachGlobalY)];

        // Carve a passage between existingCell and attachingCell in some direction.
        // For simplicity, assume the new region is placed to the right of the existing cell:
        // existing cell is at (attachGlobalX, attachGlobalY)
        // new region starts at (attachGlobalX+1, attachGlobalY) basically.
        // That means dx = 1, from existing to attaching cell.
        existingCell.IsOpenRight = true;
        attachingCell.IsOpenLeft = true;
    }

    private void InstantiateRegionCells(Region region)
    {
        GameObject regionGO = new GameObject("Region_" + region.OriginX + "_" + region.OriginY);
        regionGO.transform.SetParent(GameObject.Find("Map").transform); // or another root like MapRoot

        for (int x = 0; x < region.Width; x++)
        {
            for (int y = 0; y < region.Height; y++)
            {
                CellT cell = region.Cells[x, y];
                (int globalX, int globalY) = region.LocalToGlobal(x, y);

                // Determine world position:
                Vector3 cellPosition = mazeOrigin + new Vector3(globalX * cellSize, 0f, globalY * cellSize);
                //Vector3 cellPosition = mazeOrigin + new Vector3(globalX , 0f, globalY);

                // Choose the prefab based on walkability:
                GameObject prefabToUse = cell.IsWalkable ? walkableCellPrefab : nonWalkableCellPrefab;

                GameObject cellGO = Instantiate(prefabToUse, cellPosition, Quaternion.identity, regionGO.transform);
                cellGO.transform.localScale = new Vector3(cellSize, cellGO.transform.localScale.y, cellSize);

                
            }
        }
    }
}

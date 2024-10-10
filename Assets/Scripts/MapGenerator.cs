using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap MainTilemap;
    public int mapWidth = 50;
    public int mapHeight = 50;

    private Dictionary<Vector3Int, Cell> gridCells = new Dictionary<Vector3Int, Cell>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Cell newCell = new Cell(cellPosition);
                gridCells[cellPosition] = newCell;

                // Optionally: Set tile or color on tilemap
                // tilemap.SetTile(cellPosition, yourTile);
            }
        }
    }
}

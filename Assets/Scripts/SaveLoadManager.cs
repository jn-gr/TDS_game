using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


//[System.Serializable]
//public class CellTData
//{
//    public int X;
//    public int Y;

//    public bool IsOpenUp;
//    public bool IsOpenDown;
//    public bool IsOpenLeft;
//    public bool IsOpenRight;

//    public string objectPlacedOnCellName; // Store object name if needed
//}

//[System.Serializable]
//public class RegionData
//{
//    public int Width;
//    public int Height;

//    public List<List<CellTData>> Cells; // Serialized version of the 2D array
//    public int RegionX;
//    public int RegionY;

//    public List<CellTData> StartCells;
//    public List<CellTData> EndCells;
//}

//[System.Serializable]
//public class SpawnerData
//{
//    public int x;
//    public int y;

//}

//[System.Serializable]
//public class SaveData
//{
//    public List<CellTData> GlobalMapCells;
//    public List<RegionData> Regions;
//    public List<SpawnerData> Spawners;
//}


//public class SaveLoadManager : MonoBehaviour
//{
//    public static SaveLoadManager Instance;

//    public bool isLoad = false;

//    // Reference to the data to save/load
//    public Dictionary<(int, int), CellT> globalMap ;
//    public Dictionary<(int, int), Region> globalRegionMap;
//    public Dictionary<(int, int), Spawner> spawnerPositions;

//    private void Awake()
//    {
//        // Singleton pattern
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void OnLoadGameClicked()
//    {
//        LoadGame();

//        isLoad = true;


//    }


//    public void SaveGame()
//    {

//        globalMap = MapManager.Instance.globalMap;
//        globalRegionMap = MapManager.Instance.globalRegionMap;
//        spawnerPositions = MapManager.Instance.spawnerPositions;
//    SaveData saveData = new SaveData();

//        saveData.Regions = new List<RegionData>();
//        saveData.GlobalMapCells = new List<CellTData>();
//        // Convert Regions
//        foreach (var kvp in globalRegionMap)
//        {
//            Region region = kvp.Value;
//            RegionData regionData = new RegionData
//            {
//                Width = region.Width,
//                Height = region.Height,
//                RegionX = region.RegionX,
//                RegionY = region.RegionY,
//                StartCells = region.startCells.ConvertAll(c => ConvertCell(c)),
//                EndCells = region.endCells.ConvertAll(c => ConvertCell(c)),
//                //Cells = ConvertCellsArray((region.RegionX, region.RegionY), region.Cells)
//            };
//            saveData.Regions.Add(regionData);
//        }

//        // Convert globalMap

//        foreach (var cell in globalMap)
//        {

//            saveData.GlobalMapCells.Add(ConvertCell(cell.Value));
//        }



//        // Convert spawnerPositions to serializable format
//        saveData.Spawners = new List<SpawnerData>();
//        spawnerPositions = MapManager.Instance.spawnerPositions;
//        foreach (var kvp in spawnerPositions)
//        {
//            saveData.Spawners.Add(new SpawnerData
//            {
//                x = kvp.Key.Item1,
//                y = kvp.Key.Item2,

//            });
//        }

//        // Serialize and save to file
//        string json = JsonUtility.ToJson(saveData, true);
//        string path = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");
//        System.IO.File.WriteAllText(path, json);
//        Debug.Log("Game Saved: " + path);
//        globalMap.Clear();
//        globalRegionMap.Clear();
//        spawnerPositions.Clear();
//    }

//    /// <summary>
//    /// Converts from Cell to serialised data
//    /// </summary>
//    /// <param name="cell"></param>
//    /// <returns></returns>
//    private CellTData ConvertCell(CellT cell)
//    {
//        return new CellTData
//        {

//            X = cell.X,                          // Save the X coordinate
//            Y = cell.Y,                          // Save the Y coordinate
//            IsOpenUp = cell.IsOpenUp,            // Save the open state (Up)
//            IsOpenDown = cell.IsOpenDown,        // Save the open state (Down)
//            IsOpenLeft = cell.IsOpenLeft,        // Save the open state (Left)
//            IsOpenRight = cell.IsOpenRight,      // Save the open state (Right)
//            objectPlacedOnCellName = cell.objectPlacedOnCell?.name // Save the name of the placed object
//        };
//    }

//    /// <summary>
//    /// converts from serialised data into data unity can read
//    /// </summary>
//    /// <param name="cell"></param>
//    /// <returns></returns>
//    private CellT ConvertCell(CellTData cellTData) {

//        return new CellT(cellTData.X, cellTData.Y)
//        { 
//            IsOpenUp = cellTData.IsOpenUp,          // Set open state (Up)
//            IsOpenDown = cellTData.IsOpenDown,      // Set open state (Down)
//            IsOpenLeft = cellTData.IsOpenLeft,      // Set open state (Left)
//            IsOpenRight = cellTData.IsOpenRight,    // Set open state (Right)
//            objectPlacedOnCell = string.IsNullOrEmpty(cellTData.objectPlacedOnCellName)
//            ? null
//            : GameObject.Find(cellTData.objectPlacedOnCellName) // Find GameObject by name if it exists
//        };
//    }
//    private List<List<CellTData>> ConvertCellsArray(Region region, CellT[,] cells)
//    {
//        var list = new List<List<CellTData>>(); // Initialize the nested list
//        for (int i = 0; i < cells.GetLength(0); i++)    // Iterate through rows
//        {
//            var row = new List<CellTData>();   // Create a new list for each row
//            for (int j = 0; j < cells.GetLength(1); j++) // Iterate through columns
//            {
//                row.Add(ConvertCell(cells[i, j]));      // Convert each CellT to CellTSerializable
//            }
//            list.Add(row);                              // Add the row to the main list
//        }
//        return list;                                    // Return the nested list
//    }

//    private CellT[,] ConvertCellsArray(List<List<CellTData>> cellsList)
//    {
//        int rows = cellsList.Count;               // Get the number of rows
//        int cols = cellsList[0].Count;            // Get the number of columns (assuming uniform structure)
//        CellT[,] cells = new CellT[rows, cols];   // Initialize a 2D array of CellT

//        for (int i = 0; i < rows; i++)
//        {
//            for (int j = 0; j < cols; j++)
//            {
//                cells[i, j] = ConvertCell(cellsList[i][j]); // Convert each CellTSerializable into a CellT
//            }
//        }
//        return cells; // Return the reconstructed 2D array
//    }

//    public void LoadGame()
//    {
//        string path = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");

//        if (System.IO.File.Exists(path))
//        {
//            string json = System.IO.File.ReadAllText(path);
//            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

//            // Clear existing data


//            // Load Regions
//            foreach (var regionData in saveData.Regions)
//            {
//                Debug.Log(regionData.Cells);
//                Region region = new Region(regionData.RegionX, regionData.RegionY, regionData.Width, regionData.Height)
//                {
//                    startCells = regionData.StartCells.ConvertAll(c => ConvertCell(c)),
//                    endCells = regionData.EndCells.ConvertAll(c => ConvertCell(c))
//                    //Cells = ConvertCellsArray(regionData.Cells)
//                };
//                globalRegionMap[(region.RegionX, region.RegionY)] = region;
//            }

//            // Load globalMap
//            foreach (var cellData in saveData.GlobalMapCells)
//            {
//                CellT cell = ConvertCell(cellData);
//                globalMap[(cell.X, cell.Y)] = cell;
//            }

//            foreach (var spawnerData in saveData.Spawners)
//            {

//                Spawner spawner = new Spawner();
//                spawnerPositions[(spawnerData.x, spawnerData.y)] = spawner;
//            }



//            Debug.Log("Game Loaded");
//        }
//        else
//        {
//            Debug.LogWarning("Save file not found!");
//        }


//    }
//}

[System.Serializable]
public class KeyValuePairCell
{
    public int GlobalX;
    public int GlobalY;
    public SerializableCellT Cell;
}

[System.Serializable]
public class SerializableCellT
{
    
    public int X;
    public int Y;
    public bool IsOpenUp;
    public bool IsOpenDown;
    public bool IsOpenLeft;
    public bool IsOpenRight;
    public string ObjectPlacedOnCellName; // Store GameObject by name or unique ID
}

[System.Serializable]
public class SerializableRegion
{
    public int Width;
    public int Height;
    public int RegionX;
    public int RegionY;
    public SerializableCellT[] Cells; // Flattened array for serialization
    public int[] StartCellsIndices; // Indices of start cells in the Cells array
    public int[] EndCellsIndices; // Indices of end cells in the Cells array
}


[System.Serializable]
public class SerializableSpawner
{
    public int X;
    public int Y;
}

[System.Serializable]
public class SerializableGameData
{
    public KeyValuePairCell[] GlobalMapCells;
    public SerializableRegion[] GlobalRegions;
    public SerializableSpawner[] Spawns;
}
public class SaveLoadUtility
{
    public static SerializableGameData ConvertToSerializable(MapManager mapManager)
    {
        var gameData = new SerializableGameData();

        // Serialize globalMap
        var cellPairs = new List<KeyValuePairCell>();
        foreach (var cellPair in mapManager.globalMap)
        {
            var globalCoords = cellPair.Key;
            var cell = cellPair.Value;
            cellPairs.Add(new KeyValuePairCell
            {
                GlobalX = globalCoords.Item1,
                GlobalY = globalCoords.Item2,
                Cell = new SerializableCellT
                {
                    X = cell.X,
                    Y = cell.Y,
                    IsOpenUp = cell.IsOpenUp,
                    IsOpenDown = cell.IsOpenDown,
                    IsOpenLeft = cell.IsOpenLeft,
                    IsOpenRight = cell.IsOpenRight,
                    ObjectPlacedOnCellName = cell.objectPlacedOnCell?.name
                }
            });
        }
        gameData.GlobalMapCells = cellPairs.ToArray();

        // Serialize globalRegionMap
        List<SerializableRegion> regionList = new List<SerializableRegion>();
        foreach (var regionPair in mapManager.globalRegionMap)
        {
            Region region = regionPair.Value;

            List<SerializableCellT> regionCells = new List<SerializableCellT>();
            Dictionary<CellT, int> cellToIndex = new Dictionary<CellT, int>();

            for (int y = 0; y < region.Height; y++)
            {
                for (int x = 0; x < region.Width; x++)
                {
                    var cell = region.Cells[x, y];
                    regionCells.Add(new SerializableCellT
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpenUp = cell.IsOpenUp,
                        IsOpenDown = cell.IsOpenDown,
                        IsOpenLeft = cell.IsOpenLeft,
                        IsOpenRight = cell.IsOpenRight,
                        ObjectPlacedOnCellName = cell.objectPlacedOnCell?.name
                    });
                    cellToIndex[cell] = regionCells.Count - 1;
                }
            }

            regionList.Add(new SerializableRegion
            {
                Width = region.Width,
                Height = region.Height,
                RegionX = region.RegionX,
                RegionY = region.RegionY,
                Cells = regionCells.ToArray(),
                StartCellsIndices = region.startCells.Select(c => cellToIndex[c]).ToArray(),
                EndCellsIndices = region.endCells.Select(c => cellToIndex[c]).ToArray()
            });
        }
        gameData.GlobalRegions = regionList.ToArray();

        List<SerializableSpawner> spawnerList = new List<SerializableSpawner>();
        foreach( var spawnerPos in mapManager.spawnerPositions.Keys)
        {
            spawnerList.Add(new SerializableSpawner
            {
                X = spawnerPos.Item1,
                Y = spawnerPos.Item2
            });

        }

        gameData.Spawns = spawnerList.ToArray();    
        

        return gameData;
    }

    public static void LoadFromSerializable(MapManager mapManager, SerializableGameData gameData)
    {
        // Deserialize globalMap
        mapManager.globalMap.Clear();
        foreach (var pair in gameData.GlobalMapCells)
        {
            CellT cell = new CellT(pair.Cell.X, pair.Cell.Y)
            {
                IsOpenUp = pair.Cell.IsOpenUp,
                IsOpenDown = pair.Cell.IsOpenDown,
                IsOpenLeft = pair.Cell.IsOpenLeft,
                IsOpenRight = pair.Cell.IsOpenRight
                // ObjectPlacedOnCell can be reconstructed if needed
            };
            mapManager.globalMap[(pair.GlobalX, pair.GlobalY)] = cell;
        }

        // Deserialize globalRegionMap
        mapManager.globalRegionMap.Clear();
        foreach (var serialRegion in gameData.GlobalRegions)
        {
            Region region = new Region(serialRegion.RegionX, serialRegion.RegionY, serialRegion.Width, serialRegion.Height )
            {
                Cells = new CellT[serialRegion.Width, serialRegion.Height]
            };

            for (int i = 0; i < serialRegion.Cells.Length; i++)
            {
                var serialCell = serialRegion.Cells[i];
                region.Cells[serialCell.X, serialCell.Y] = new CellT(serialCell.X,serialCell.Y)
                {
                    IsOpenUp = serialCell.IsOpenUp,
                    IsOpenDown = serialCell.IsOpenDown,
                    IsOpenLeft = serialCell.IsOpenLeft,
                    IsOpenRight = serialCell.IsOpenRight
                };
            }

            region.startCells = serialRegion.StartCellsIndices
                .Select(index => region.Cells[serialRegion.Cells[index].X, serialRegion.Cells[index].Y]).ToList();
            region.endCells = serialRegion.EndCellsIndices
                .Select(index => region.Cells[serialRegion.Cells[index].X, serialRegion.Cells[index].Y]).ToList();

            mapManager.globalRegionMap[(region.RegionX, region.RegionY)] = region;
        }

        foreach (var spawn in gameData.Spawns)
        {
            // Instantiate the spawner prefab at the correct position
            Vector3 spawnPosition = mapManager.tilemap.GetCellCenterWorld(new Vector3Int(spawn.X, spawn.Y, 0));
            Spawner spawner = GameObject.Instantiate(mapManager.spawnerPrefab, spawnPosition, Quaternion.identity).GetComponent<Spawner>();

            // Add the spawner to the dictionary
            mapManager.spawnerPositions[(spawn.X, spawn.Y)] = spawner;
        }
    }
}


public class SaveLoadManager : MonoBehaviour
{
    private const string SaveFilePath = "savegame.json";

    public static void SaveGame()
    {
        SerializableGameData data = SaveLoadUtility.ConvertToSerializable(MapManager.Instance);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFilePath), json);
        Debug.Log("Game Saved");
    }

    public static void LoadGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFilePath);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SerializableGameData data = JsonUtility.FromJson<SerializableGameData>(json);
            SaveLoadUtility.LoadFromSerializable(MapManager.Instance, data);
            Debug.Log("Game Loaded");
            MapManager.Instance.LoadGame();
            foreach (var regionPair in MapManager.Instance.globalRegionMap)
            {
                var coords = regionPair.Key; // This is the (int, int) tuple key
                Region region = regionPair.Value; // This is the Region object

                Debug.Log($"Region Coordinates: X = {coords.Item1}, Y = {coords.Item2}");
            }
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }
}
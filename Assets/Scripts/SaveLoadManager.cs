using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class DifficultySerializable
{
    public DifficultyLevel Difficulty;
}

[System.Serializable]
public class SkillsSerializable
{
    public int skill1;
    public int skill2;
    public int skill3;
    public int skill4;
    public int skill5;
    public int skill6;
    public int skill7;

}

[System.Serializable]
public class GameStats
{
    public float CurrentHealth;
    public float Currency;
    public float Experience;
    public int TotalKills;
    public int Score;
    public int WaveNum;
}

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
    public int TowerType;
    public int TowerIndex;
    public string ObjectPlacedOnCellName;
}

[System.Serializable]
public class SerializableRegion
{
    public int Width;
    public int Height;
    public int RegionX;
    public int RegionY;
    public SerializableCellT[] Cells; 
    public int[] StartCellsIndices; // Indices of start cells in the Cells array
    public int[] EndCellsIndices; 
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
    public DifficultySerializable Difficulty;
    public SkillsSerializable Skills;
    public GameStats Stats;
    public KeyValuePairCell[] GlobalMapCells;
    public SerializableRegion[] GlobalRegions;
    public SerializableSpawner[] Spawns;
}
public class SaveLoadUtility
{
    public static SerializableGameData ConvertToSerializable(MapManager mapManager, GameManager gameManager)
    {
        var gameData = new SerializableGameData();
        gameData.Difficulty = new DifficultySerializable
        {
            Difficulty = UserDifficulty.CurrentLevel
        };

        gameData.Skills = new SkillsSerializable
        {
            skill1 = SkillTree.Instance.PassiveSkills[0].CurrentLevel,
            skill2 = SkillTree.Instance.PassiveSkills[1].CurrentLevel,
            skill3 = SkillTree.Instance.PassiveSkills[2].CurrentLevel,
            skill4 = SkillTree.Instance.PassiveSkills[3].CurrentLevel,
            skill5 = SkillTree.Instance.PassiveSkills[4].CurrentLevel,
            skill6 = SkillTree.Instance.PassiveSkills[5].CurrentLevel,
            skill7 = SkillTree.Instance.ActiveSkills[0].CurrentLevel

        };
        gameData.Stats = new GameStats
        {
            CurrentHealth = gameManager.currentHealth,
            Currency = gameManager.currency,
            Experience = gameManager.experience,
            TotalKills = gameManager.totalKills,
            Score = gameManager.score,
            WaveNum = gameManager.waveNum

        };


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
                    TowerIndex = -1,
                    TowerType = -1,
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
                    int towerType = -1;
                    int towerIndex = -1;
                    CellT cell = region.Cells[x, y];
                    if (cell.objectPlacedOnCell != null)
                    {
                        if (cell.objectPlacedOnCell.GetComponent<Turret>() != null)
                        {
                            
                            towerType = 0;

                            string objectName = cell.objectPlacedOnCell.name.Replace("(Clone)", "").Trim();


                            for (int i = 0; i < GameManager.Instance.turretPrefabs.Length; i++)
                            {
                                if (GameManager.Instance.turretPrefabs[i].name == objectName)
                                {
                                    towerIndex = i;
                                    break;
                                }
                            }
                        }
                        else if (cell.objectPlacedOnCell.GetComponent<Sniper>() != null)
                        {
                            
                            towerType = 1;

                            string objectName = cell.objectPlacedOnCell.name.Replace("(Clone)", "").Trim();


                            for (int i = 0; i < GameManager.Instance.sniperPrefabs.Length; i++)
                            {
                                if (GameManager.Instance.sniperPrefabs[i].name == objectName)
                                {
                                    towerIndex = i;
                                    break;
                                }
                            }
                        }
                        else if (cell.objectPlacedOnCell.GetComponent<RapidFire>() != null)
                        {
                           
                            towerType = 2;

                            string objectName = cell.objectPlacedOnCell.name.Replace("(Clone)", "").Trim();


                            for (int i = 0; i < GameManager.Instance.rapidFirePrefabs.Length; i++)
                            {
                                if (GameManager.Instance.rapidFirePrefabs[i].name == objectName)
                                {
                                    towerIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    regionCells.Add(new SerializableCellT
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpenUp = cell.IsOpenUp,
                        IsOpenDown = cell.IsOpenDown,
                        IsOpenLeft = cell.IsOpenLeft,
                        IsOpenRight = cell.IsOpenRight,
                        TowerType = towerType,
                        TowerIndex = towerIndex,
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
        foreach (var spawnerPos in mapManager.spawnerPositions.Keys)
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

    public static void LoadFromSerializable(MapManager mapManager, GameManager gameManager, SerializableGameData gameData)
    {
        if ((gameData.Difficulty != null))
        {
            UserDifficulty.CurrentLevel = gameData.Difficulty.Difficulty;
        }
        if (gameData.Skills != null)
        {
            for (int i = 0; i < gameData.Skills.skill1; i++)
            {
                SkillTree.Instance.PassiveSkills[0].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill2; i++)
            {
                SkillTree.Instance.PassiveSkills[1].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill3; i++)
            {
                SkillTree.Instance.PassiveSkills[2].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill4; i++)
            {
                SkillTree.Instance.PassiveSkills[3].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill5; i++)
            {
                SkillTree.Instance.PassiveSkills[4].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill6; i++)
            {
                SkillTree.Instance.PassiveSkills[5].Upgrade();
            }
            for (int i = 0; i < gameData.Skills.skill7; i++)
            {
                SkillTree.Instance.ActiveSkills[0].Upgrade();
            }

            SkillTree.Instance.ui.UpdateSkillTreeUI(SkillTree.Instance.PassiveSkills, SkillTree.Instance.ActiveSkills);
           
        }
        if (gameData.Stats != null)
        {
            gameManager.currentHealth = gameData.Stats.CurrentHealth;
            gameManager.currency = gameData.Stats.Currency;
            gameManager.experience = gameData.Stats.Experience;
            gameManager.totalKills = gameData.Stats.TotalKills;
            gameManager.score = gameData.Stats.Score;
            gameManager.waveNum = gameData.Stats.WaveNum;
        }
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
                
            };
            mapManager.globalMap[(pair.GlobalX, pair.GlobalY)] = cell;
        }

        // Deserialize globalRegionMap
        mapManager.globalRegionMap.Clear();
        foreach (var serialRegion in gameData.GlobalRegions)
        {
            Region region = new Region(serialRegion.RegionX, serialRegion.RegionY, serialRegion.Width, serialRegion.Height)
            {
                Cells = new CellT[serialRegion.Width, serialRegion.Height]
            };

            
            for (int i = 0; i < serialRegion.Cells.Length; i++)
            {
                var serialCell = serialRegion.Cells[i];

                GameObject objectOnCell = null;
                if (serialCell.TowerType != -1)
                {

                    if (serialCell.TowerType == 0)
                    {
                        Debug.Log(GameManager.Instance.turretPrefabs.Count());
                        Debug.Log(serialCell.TowerIndex);
                        objectOnCell = GameManager.Instance.turretPrefabs[serialCell.TowerIndex];


                    }
                    else if (serialCell.TowerType == 1)
                    {
                        objectOnCell = GameManager.Instance.sniperPrefabs[serialCell.TowerIndex];
                    }
                    else if (serialCell.TowerType == 2)
                    {
                        objectOnCell = GameManager.Instance.rapidFirePrefabs[serialCell.TowerIndex];
                    }

                }
                region.Cells[serialCell.X, serialCell.Y] = new CellT(serialCell.X, serialCell.Y)
                {
                    IsOpenUp = serialCell.IsOpenUp,
                    IsOpenDown = serialCell.IsOpenDown,
                    IsOpenLeft = serialCell.IsOpenLeft,
                    IsOpenRight = serialCell.IsOpenRight,
                    objectPlacedOnCell = objectOnCell
                };
            }

            region.startCells = serialRegion.StartCellsIndices
                .Select(index => region.Cells[serialRegion.Cells[index].X, serialRegion.Cells[index].Y]).ToList();
            region.endCells = serialRegion.EndCellsIndices
                .Select(index => region.Cells[serialRegion.Cells[index].X, serialRegion.Cells[index].Y]).ToList();

            mapManager.globalRegionMap[(region.RegionX, region.RegionY)] = region;
        }

        foreach (var spawnerPair in mapManager.spawnerPositions)
        {
            Spawner spawner = spawnerPair.Value;

            // Check if the spawner's GameObject is not null
            if (spawner != null && spawner.gameObject != null)
            {
                GameObject.Destroy(spawner.gameObject); // Destroy the GameObject
            }
        }

        

        mapManager.spawnerPositions.Clear();
        foreach (var spawn in gameData.Spawns)
        {
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
    public static SaveLoadManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SaveGame()
    {
        SerializableGameData data = SaveLoadUtility.ConvertToSerializable(MapManager.Instance, GameManager.Instance);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFilePath), json);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameCo());

        static IEnumerator LoadGameCo()
        {
            while (GameManager.Instance == null || MapManager.Instance == null)
            {
                yield return null; // Wait for the next frame
            }


            string filePath = Path.Combine(Application.persistentDataPath, SaveFilePath);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SerializableGameData data = JsonUtility.FromJson<SerializableGameData>(json);
                SaveLoadUtility.LoadFromSerializable(MapManager.Instance, GameManager.Instance, data);
                Debug.Log("Game Loaded");
                MapManager.Instance.LoadGame();

            }
            else
            {
                Debug.LogWarning("Save file not found");
            }

        }

    }
}
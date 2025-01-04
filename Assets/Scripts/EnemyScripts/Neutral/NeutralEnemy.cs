using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class NeutralEnemy : MonoBehaviour
{

    // Static variables shared by all instances
    public static float GlobalDamage = 5;
    public static float GlobalHealth = 10;
    public static float GlobalSpeed = 1;

    // Instance-specific variables initialized from static
    public float damage;
    public float health;
    public float speed;

    public float selfDamageMultiplier = 0.9f;
    public float strongDamageMultiplier = 1.2f;
    public float weakDamageMultiplier = 0.8f;

    protected MainTower castle;
    protected GameManager gameManager;
    protected bool isDead;

    public Element element;
    public float levitationHeight = 2f; // Max height to levitate
    public float levitationSpeed = 2f;   // Speed of levitation
    private float startingY;              // Starting Y position

    private bool pathToTargetFound = false;
    public static NeutralEnemy Instance { get; private set; }
    private List<Vector3> pathToTarget;
    private int currentPathIndex;
    private Vector3Int currentCell;
    private bool pathNeedsUpdate;
    private float pathUpdateCooldown = 1f; // How often to recalculate path
    private float lastPathUpdateTime;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (Instance == null) Instance = this;
        gameManager = FindFirstObjectByType<GameManager>();
        castle = gameManager.mainTower;

        // Initialize instance-specific variables from static values
        damage = GlobalDamage;
        health = GlobalHealth;
        speed = GlobalSpeed;

        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)Math.Min(25.0, (5.0+(gameManager.waveNum * 1.5)));
        
        element = Element.Neutral;
        startingY = transform.position.y;

        pathToTarget = new List<Vector3>();
        currentPathIndex = 0;
        pathNeedsUpdate = true;

        Vector3Int currentCell = MapManager.Instance.tilemap.WorldToCell(transform.position);
        Vector3Int nearestWalkable = MapManager.Instance.FindNearestWalkableTile(currentCell);
        if (!MapManager.Instance.globalMap.TryGetValue((nearestWalkable.x, nearestWalkable.y), out CellT nearestWalkableData) || !nearestWalkableData.IsWalkable)
        {
            Debug.LogError($"Starting position {nearestWalkable} is not walkable.");
            return;
        }
        //transform.position = MapManager.Instance.tilemap.GetCellCenterWorld(nearestWalkable);
        Vector3 targetPosition = MapManager.Instance.tilemap.GetCellCenterWorld(nearestWalkable);
        // Apply a Y offset to place the mob correctly above the ground
        float yOffset = GetComponent<Collider>().bounds.extents.y; // Adjust this value based on the mob's height
        transform.position = new Vector3(targetPosition.x, targetPosition.y + yOffset, targetPosition.z);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Time.time - lastPathUpdateTime > pathUpdateCooldown)
        {
            pathNeedsUpdate = true;
        }

        if (pathNeedsUpdate)
        {
            UpdatePath();
        }

        if (pathToTarget.Count > 0 && currentPathIndex < pathToTarget.Count)
        {
            Vector3 targetPosition = pathToTarget[currentPathIndex];
            Vector3 direction = targetPosition - transform.position;

            // Rotate to face the direction of movement
            Vector3 horizontalDirection = new Vector3(direction.x, 90, direction.z); // Ignore Y axis
            if (horizontalDirection != Vector3.zero) // Avoid errors if direction is zero
            {
                Quaternion rotationOffset = Quaternion.Euler(0, 0, 180); // Adjust this value as needed
                Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation * rotationOffset, Time.deltaTime * 10f); // Smooth rotation
            }

            // Move toward the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            //// Apply levitation effect
            float currentHeight = Mathf.Abs(Mathf.Sin(Time.time * levitationSpeed)) * levitationHeight;
            transform.position = new Vector3(transform.position.x, startingY + currentHeight, transform.position.z);

            // If close enough to the target, move to the next point
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                currentPathIndex++;
            }
        }
    }


    public virtual void TakeDamage(float damage, Element element)
    {
        if (isDead) return;
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            Debug.Log("NeutralEnemy is dead. Calling EnemyKilled.");
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }

    public static void ScaleStatsForWave(int waveNumber)
    {
        // Base scaling for wave number
        float waveDamage = 5 + (waveNumber * 0.5f);
        float waveHealth = 10 + (waveNumber * 1.1f);
        float waveSpeed = 1 + (waveNumber * 0.2f);

        // Apply difficulty multipliers
        if (UserDifficulty.CurrentLevel == DifficultyLevel.Hard)
        {
            GlobalDamage = waveDamage * 3.0f;
            GlobalHealth = waveHealth * 3.0f;
            GlobalSpeed = waveSpeed * 1.5f;
        }
        else if (UserDifficulty.CurrentLevel == DifficultyLevel.Medium)
        {
            GlobalDamage = waveDamage * 2.0f;
            GlobalHealth = waveHealth * 2.0f;
            GlobalSpeed = waveSpeed * 1.25f;
        }
        else // Easy difficulty
        {
            GlobalDamage = waveDamage;
            GlobalHealth = waveHealth;
            GlobalSpeed = waveSpeed;
        }

        Debug.Log($"Stats scaled for wave {waveNumber} and difficulty {UserDifficulty.CurrentLevel}: " +
                  $"Damage={GlobalDamage}, Health={GlobalHealth}, Speed={GlobalSpeed}");
    }

    private void UpdatePath()
    {
        if (pathToTargetFound)
        {
            Debug.Log("Path to target already found. Skipping update.");
            return;
        }

        Vector3Int targetCell = new Vector3Int(5, 5, 0);
        if (!MapManager.Instance.globalMap.TryGetValue((targetCell.x, targetCell.y), out CellT targetCellData) || !targetCellData.IsWalkable)
        {
            Debug.LogError($"Target cell {targetCell} is not walkable.");
            return;
        }
        if (MapManager.Instance == null || MapManager.Instance.tilemap == null)
        {
            Debug.LogError("MapManager.Instance or its tilemap is null");
            return;
        }
        currentCell = MapManager.Instance.tilemap.WorldToCell(transform.position);
        Debug.Log($"Updating path from {currentCell} to {targetCell}");
        List<Vector3Int> path = FindPath(currentCell, targetCell);

        if (path != null && path.Count > 0)
        {
            pathToTarget.Clear();
            foreach (Vector3Int cell in path)
            {
                pathToTarget.Add(MapManager.Instance.tilemap.GetCellCenterWorld(cell));
            }
            currentPathIndex = 0;
            pathToTargetFound = true; // Mark the path as found
            Debug.Log("Path successfully updated.");
        }
        else
        {
            Debug.LogWarning($"No path found from {currentCell} to {targetCell}");
        }

        pathNeedsUpdate = false;
        lastPathUpdateTime = Time.time;
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        Debug.Log($"Finding path from {start} to {target}");
        var openSet = new List<PathNode>();
        var closedSet = new HashSet<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, float> { [start] = 0 };
        var fScore = new Dictionary<Vector3Int, float> { [start] = Mathf.Abs(start.x - target.x) + Mathf.Abs(start.y - target.y) };

        openSet.Add(new PathNode(start, 0, fScore[start]));

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(node => node.FScore).First();
            Debug.Log($"Processing node {current.Position}");

            if (current.Position == target)
            {
                Debug.Log($"Path found from {start} to {target}");
                return ReconstructPath(cameFrom, current.Position);
            }

            openSet.Remove(current);
            closedSet.Add(current.Position);

            foreach (var neighbor in GetNeighbors(current.Position))
            {
                Debug.Log($"Processing neighbor: {neighbor}");
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current.Position] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    Debug.Log($"Updating neighbor {neighbor}, GScore: {tentativeGScore}");
                    cameFrom[neighbor] = current.Position;
                    gScore[neighbor] = tentativeGScore;
                    float h = Mathf.Abs(neighbor.x - target.x) + Mathf.Abs(neighbor.y - target.y);
                    fScore[neighbor] = gScore[neighbor] + h;

                    if (!openSet.Any(n => n.Position == neighbor))
                    {
                        openSet.Add(new PathNode(neighbor, gScore[neighbor], h));
                    }
                }
            }
        }
        Debug.LogWarning($"No path found from {start} to {target}");
        return null;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        var neighbors = new List<Vector3Int>();
        var directions = new[]
        {
            Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down
        };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = position + dir;
            if (MapManager.Instance.globalMap.TryGetValue((neighborPos.x, neighborPos.y), out CellT cell) && cell.IsWalkable)
            {
                // Ensure connectivity between cells
                if ((dir == Vector3Int.right && cell.IsWalkable) ||
                    (dir == Vector3Int.left && cell.IsWalkable) ||
                    (dir == Vector3Int.up && cell.IsWalkable) ||
                    (dir == Vector3Int.down && cell.IsWalkable))
                {
                    neighbors.Add(neighborPos);
                }
            }
        }
        Debug.Log($"Neighbors of {position}: {string.Join(", ", neighbors)}");
        return neighbors;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
    private void OnDrawGizmos()
    {
        if (pathToTarget != null && pathToTarget.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < pathToTarget.Count - 1; i++)
            {
                Gizmos.DrawLine(pathToTarget[i], pathToTarget[i + 1]);
            }

            foreach (var point in pathToTarget)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
    }
}

public class PathNode
{
    public Vector3Int Position { get; private set; }
    public float GScore { get; private set; }
    public float HScore { get; private set; }
    public float FScore => GScore + HScore;

    public PathNode(Vector3Int position, float gScore, float hScore)
    {
        Position = position;
        GScore = gScore;
        HScore = hScore;
    }
}
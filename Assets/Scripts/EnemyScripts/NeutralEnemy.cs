using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NeutralEnemy : MonoBehaviour
{
    public static NeutralEnemy Instance { get; private set; }
    private List<Vector3> pathToTarget;
    private int currentPathIndex;
    private Vector3Int currentCell;
    private bool pathNeedsUpdate;
    private float pathUpdateCooldown = 1f; // How often to recalculate path
    private float lastPathUpdateTime;
    public float damage = 10;
    public float health = 20;
    public float speed = 5;
    public float selfDamageMultiplier = 0.9f;
    public float strongDamageMultiplier = 1.2f;
    public float weakDamageMultiplier = 0.8f;
    protected MainTower castle;
    protected GameManager gameManager;
    protected bool isDead;
    public Element element;
    public float levitationHeight = 5f; // Max height to levitate
    public float levitationSpeed = 5f;   // Speed of levitation
    private float startingY;              // Starting Y position

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of NeutralEnemy detected! Destroying duplicate.");
            Destroy(this.gameObject);
            return;
        }
        gameManager = FindFirstObjectByType<GameManager>();
        castle = gameManager.mainTower;
        health = (int)(8 + (gameManager.waveNum * 1.1));
        speed = (float)(5 + (gameManager.waveNum * 1.5));
        element = Element.Neutral;
        startingY = transform.position.y; // Set starting Y to initial Y
        pathToTarget = new List<Vector3>();
        currentPathIndex = 0;
        pathNeedsUpdate = true;
        lastPathUpdateTime = 0f;

        // Move enemy to nearest walkable tile
        Vector3Int currentCell = MapManager.Instance.tilemap.WorldToCell(transform.position);
        Vector3Int nearestWalkable = MapManager.Instance.FindNearestWalkableTile(currentCell);
        transform.position = MapManager.Instance.tilemap.GetCellCenterWorld(nearestWalkable);
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
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Apply levitation effect
            float currentHeight = Mathf.Abs(Mathf.Sin(Time.time * levitationSpeed)) * levitationHeight;
            transform.position = new Vector3(transform.position.x, startingY + currentHeight, transform.position.z);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
        // Calculate smooth levitation effect with sine wave (shifted to stay above the plane)

        // Apply the levitation effect to the Y position
    }

    public virtual void TakeDamage(float damage, Element element)
    {
        if (isDead) return;

        health -= damage * 1.0f;

        if (health <= 0)
        {
            isDead = true;
            gameManager.EnemyKilled();
            Destroy(gameObject);
        }
    }

    private void UpdatePath()
    {
        if (MapManager.Instance == null || MapManager.Instance.tilemap == null)
        {
            Debug.LogError("MapManager.Instance or its tilemap is null");
            return;
        }

        Vector3Int targetCell = MapManager.Instance.tilemap.WorldToCell(castle.transform.position);
        currentCell = MapManager.Instance.tilemap.WorldToCell(transform.position);

        List<Vector3Int> path = FindPath(currentCell, targetCell);

        Debug.Log($"Trying to find path from {currentCell} to {targetCell}");

        if (path != null)
        {
            pathToTarget.Clear();
            foreach (Vector3Int cell in path)
            {
                pathToTarget.Add(MapManager.Instance.tilemap.GetCellCenterWorld(cell));
            }
            currentPathIndex = 0;
        }
        else
        {
            Debug.LogWarning($"Path could not be found from {currentCell} to {targetCell}");
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
        var fScore = new Dictionary<Vector3Int, float> { [start] = Vector3.Distance(start, target) };

        openSet.Add(new PathNode(start, 0, Vector3.Distance(start, target)));

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(node => node.FScore).First();

            if (current.Position == target)
            {
                Debug.Log($"Path found from {start} to {target}");
                return ReconstructPath(cameFrom, current.Position);
            }

            openSet.Remove(current);
            closedSet.Add(current.Position);
            Debug.Log($"Processing node {current.Position}");

            foreach (var neighbor in GetNeighbors(current.Position))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current.Position] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current.Position;
                    gScore[neighbor] = tentativeGScore;
                    float h = Vector3.Distance(neighbor, target);
                    fScore[neighbor] = gScore[neighbor] + h;

                    if (!openSet.Any(n => n.Position == neighbor))
                    {
                        openSet.Add(new PathNode(neighbor, gScore[neighbor], h));
                    }
                }
            }
        }
        Debug.LogWarning("No path found from {start} to {target}");
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
                if ((dir == Vector3Int.right && cell.IsOpenLeft) ||
                    (dir == Vector3Int.left && cell.IsOpenRight) ||
                    (dir == Vector3Int.up && cell.IsOpenDown) ||
                    (dir == Vector3Int.down && cell.IsOpenUp))
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
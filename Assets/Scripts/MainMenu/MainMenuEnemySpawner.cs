using UnityEngine;
using System.Collections;

public class MainMenuEnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectPrefab;
    public int totalObjectsToSpawn = 0;
    public float spawnInterval = 1f;
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Movement Settings")]
    public GameObject mainTower; // Reference to MainTower GameObject
    public float moveSpeed = 5f;
    public float objectLifetime = 3f;

    private int objectsSpawned = 0;
    private bool isSpawning = false;

    private void OnEnable()
    {
        StartSpawning();
    }

    private void Awake()
    {
        isSpawning = false;
        objectsSpawned = 0;

        if (mainTower == null)
        {
            mainTower = GameObject.Find("MainTower");
            if (mainTower == null)
            {
                Debug.LogError("MainTower GameObject not found! Please assign it in the inspector or ensure it exists in the scene.");
            }
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StopAllCoroutines();
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (isSpawning && (totalObjectsToSpawn == 0 || objectsSpawned < totalObjectsToSpawn))
        {
            SpawnObject();
            objectsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private void SpawnObject()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("No prefab assigned to spawn!");
            return;
        }

        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, transform.rotation);

        var mover = spawnedObject.AddComponent<ObjectMover>();
        mover.Initialize(mainTower, moveSpeed, objectLifetime);
    }
}
public class ObjectMover : MonoBehaviour
{
    private GameObject target;
    private float speed;
    private float lifetime;

    private float originalY; // Store the original height
    public float levitationHeight = 2f; // Maximum levitation height above the original position
    public float levitationSpeed = 2f; // Speed of levitation
    public float rotationSpeed = 100f; // Speed of rotation around its axis

    public void Initialize(GameObject targetObject, float moveSpeed, float objectLifetime)
    {
        target = targetObject;
        speed = moveSpeed;
        lifetime = objectLifetime;

        // Store the original Y position
        originalY = transform.position.y;

        StartCoroutine(DestroyAfterLifetime());
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null. Destroying object.");
            Destroy(gameObject);
            return;
        }

        // Calculate levitation effect
        float levitationOffset = Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;

        // Move towards the target while applying levitation
        Vector3 targetPosition = new Vector3(
            target.transform.position.x,
            originalY + Mathf.Abs(levitationOffset), // Ensure it doesn't go below the original height
            target.transform.position.z
        );

        // Update position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Apply rotation around the object's axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching this one is the mainTower
        if (other.gameObject == target)
        {
            Debug.Log("Object reached the MainTower and will be destroyed.");
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class MainMenuEnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The prefab to spawn and push")]
    public GameObject objectPrefab;

    [Tooltip("How many objects to spawn in total (0 for infinite)")]
    public int totalObjectsToSpawn = 0;

    [Tooltip("Time between each spawn in seconds")]
    public float spawnInterval = 1f;

    [Tooltip("Position offset from spawner where objects will appear")]
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Force Settings")]
    [Tooltip("Initial force to push the object forward")]
    public float initialForce = 500f;

    [Tooltip("Whether to apply continuous force")]
    public bool continuousForce = false;

    [Tooltip("Amount of continuous force applied per second")]
    public float continuousForceAmount = 200f;

    [Tooltip("How long each object should exist before being destroyed (in seconds)")]
    public float objectLifetime = 3f;

    private int objectsSpawned = 0;
    private bool isSpawning = false;

    private void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
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
            Debug.LogError("No prefab assigned to spawn! Please assign a prefab in the inspector.");
            return;
        }

        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, transform.rotation);

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
            Debug.LogWarning("Added Rigidbody to spawned object as none was found.");
        }

        // Initial push is now separate from continuous force
        rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);

        if (continuousForce)
        {
            // Add a separate component to handle continuous force
            ObjectForceApplier forceApplier = spawnedObject.AddComponent<ObjectForceApplier>();
            forceApplier.Initialize(continuousForceAmount, transform.forward);
        }

        // Start destruction coroutine
        StartCoroutine(DestroyAfterDelay(spawnedObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(objectLifetime);

        if (obj != null)
        {
            Destroy(obj);
        }
    }
}

// Separate component to handle continuous force
public class ObjectForceApplier : MonoBehaviour
{
    private float forceAmount;
    private Vector3 forceDirection;
    private Rigidbody rb;

    public void Initialize(float force, Vector3 direction)
    {
        forceAmount = force;
        forceDirection = direction;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Apply force in FixedUpdate for consistent physics
        if (rb != null)
        {
            rb.AddForce(forceDirection * forceAmount * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
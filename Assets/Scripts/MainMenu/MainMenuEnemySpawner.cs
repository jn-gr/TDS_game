using UnityEngine;
using System.Collections;

public class MainMenuEnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectPrefab;
    public int totalObjectsToSpawn = 0;
    public float spawnInterval = 1f;
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Force Settings")]
    public float initialForce = 500f;
    public Vector3 forceDirection = Vector3.forward;
    public bool continuousForce = false;
    public float continuousForceAmount = 200f;
    public float objectLifetime = 3f;

    private int objectsSpawned = 0;
    private bool isSpawning = false;

    private void OnEnable()
    {
        Debug.Log("OnEnable called, starting spawning...");
        StartSpawning();
    }

    private void Awake()
    {
        Debug.Log("Awake called, resetting variables.");
        isSpawning = false;
        objectsSpawned = 0;
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StopAllCoroutines();
            Debug.Log("Starting spawning process.");
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        Debug.Log("Stopping spawning process.");
        isSpawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("SpawnRoutine started.");

        while (isSpawning && (totalObjectsToSpawn == 0 || objectsSpawned < totalObjectsToSpawn))
        {
            Debug.Log($"Spawning object {objectsSpawned + 1}/{totalObjectsToSpawn}.");
            SpawnObject();
            objectsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("SpawnRoutine stopped.");
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
        Debug.Log($"Spawned object {spawnedObject.name} at {spawnPosition}.");

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
            Debug.LogWarning("Added Rigidbody to spawned object.");
        }

        rb.AddForce(forceDirection.normalized * initialForce, ForceMode.Impulse);

        if (continuousForce)
        {
            var forceApplier = spawnedObject.AddComponent<ObjectForceApplier>();
            forceApplier.Initialize(continuousForceAmount, forceDirection.normalized);
        }

        StartCoroutine(DestroyAfterDelay(spawnedObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(objectLifetime);
        Debug.Log($"Destroying object {obj.name} after {objectLifetime} seconds.");
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
        forceDirection = direction.normalized;
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

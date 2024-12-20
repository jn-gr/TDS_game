using UnityEngine;

public class WaveRotation : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager
    public int wavesPer360 = 6; // Number of waves required for a full 360-degree rotation
    public float rotationDuration = 1f; // Duration of the smooth rotation (in seconds)

    private float degreesPerWave; // Degrees to rotate per wave
    private Quaternion targetRotation; // The target rotation to reach
    private float rotationStartTime; // When the smooth rotation starts
    private bool isRotating = false; // Whether a rotation is currently happening

    void Start()
    {
        // Find the GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene!");
                return;
            }
        }

        // Calculate degrees per wave
        degreesPerWave = 360f / wavesPer360;

        // Subscribe to the WaveStarted event
        gameManager.WaveStarted += OnWaveStarted;

        // Initialize target rotation to the current rotation
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isRotating)
        {
            // Smoothly interpolate rotation towards the target
            float t = (Time.time - rotationStartTime) / rotationDuration;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            // Stop rotating if we've reached the target
            if (t >= 1f)
            {
                isRotating = false;
            }
        }
    }

    private void OnWaveStarted()
    {
        // Calculate the new target rotation
        float increment = degreesPerWave;
        targetRotation = transform.rotation * Quaternion.Euler(increment, 0, 0);

        // Start the rotation
        rotationStartTime = Time.time;
        isRotating = true;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the WaveStarted event to avoid memory leaks
        if (gameManager != null)
        {
            gameManager.WaveStarted -= OnWaveStarted;
        }
    }
}

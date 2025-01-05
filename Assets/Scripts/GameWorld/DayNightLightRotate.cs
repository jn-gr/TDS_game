using UnityEngine;

public class DayNightLightRotate : MonoBehaviour
{
    public int wavesPerFullRotation = 6; // Number of waves to complete a full 360° rotation
    public float rotationDuration = 1f; // Duration of the rotation in seconds

    private GameManager gameManager;
    private float anglePerWave;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private float rotationProgress = 0f;

    private void Start()
    {
        // Find the GameManager in the scene and subscribe to the WaveStarted event
        gameManager = FindFirstObjectByType<GameManager>();
        anglePerWave = 360f / wavesPerFullRotation;
        targetRotation = transform.rotation;

        gameManager.WaveStarted += OnWaveStarted;
    }

    private void OnWaveStarted()
    {
        if (gameManager.waveNum == 1)
        {
            return;
        }

        if (!isRotating)
        {
            StartCoroutine(SmoothRotate());
        }
    }

    private System.Collections.IEnumerator SmoothRotate()
    {
        isRotating = true;
        rotationProgress = 0f;
        Quaternion startRotation = transform.rotation;
        targetRotation *= Quaternion.Euler(-anglePerWave, 0, 0);

        while (rotationProgress < 1f)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationProgress);
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure final rotation is precise
        isRotating = false;
    }
}

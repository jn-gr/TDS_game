using UnityEngine;

public class LoadingScreenRotator : MonoBehaviour
{
    // Adjustable rotation speed in degrees per second
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second.")]
    public float rotationSpeed = 30f;

    // Axis of rotation
    [Tooltip("Axis of rotation (default is the Y-axis).")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate the object around the specified axis
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}

using UnityEngine;
using System.Collections;

public class MainMenuCameraMover : MonoBehaviour
{
    // Public variable to set the target position in the Inspector
    public Vector3 targetPosition;

    // Public variable to set the target rotation (as a quaternion) in the Inspector
    public Quaternion targetRotation;

    // Speed of movement
    public float moveSpeed = 5f;

    // Speed of rotation (degrees per second)
    public float rotationSpeed = 10f;

    // Public method that starts the smooth movement and rotation
    public void MoveToTarget()
    {
        StartCoroutine(SmoothMoveAndRotate());
    }

    private IEnumerator SmoothMoveAndRotate()
    {
        // Smooth move to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Move the camera smoothly using Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Smoothly rotate the camera towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null; // Wait until the next frame
        }

        // Ensure the camera reaches the exact target position and rotation
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}

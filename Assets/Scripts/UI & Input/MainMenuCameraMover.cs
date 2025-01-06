using UnityEngine;
using System.Collections;

public class MainMenuCameraMover : MonoBehaviour
{ 
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public void MoveToTarget()
    {
        StartCoroutine(SmoothMoveAndRotate());
    }

    private IEnumerator SmoothMoveAndRotate()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
           
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null; 
        }

        // makes sure the camera reaches the exact target position and rotation
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}

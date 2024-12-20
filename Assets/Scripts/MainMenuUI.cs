using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera (assign via Inspector)
    public Transform[] targetPositions; // Array of target positions and rotations
    public float moveSpeed = 5f; // Speed of camera movement
    public float rotateSpeed = 100f; // Speed of camera rotation

    private Transform currentTarget; // The current target position and rotation
    private bool isMoving = false;

    public void PlayGame()
    {
        SceneLoader.NextSceneName = "Main";
        SceneManager.LoadScene("Loading Screen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MoveCameraTo(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < targetPositions.Length)
        {
            currentTarget = targetPositions[targetIndex];
            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving && mainCamera != null && currentTarget != null)
        {
            // Move the camera towards the target position
            mainCamera.transform.position = Vector3.MoveTowards(
                mainCamera.transform.position,
                currentTarget.position,
                moveSpeed * Time.deltaTime
            );

            // Rotate the camera towards the target rotation
            mainCamera.transform.rotation = Quaternion.RotateTowards(
                mainCamera.transform.rotation,
                currentTarget.rotation,
                rotateSpeed * Time.deltaTime
            );

            // Check if the camera has reached the target position and rotation
            if (Vector3.Distance(mainCamera.transform.position, currentTarget.position) < 0.01f &&
                Quaternion.Angle(mainCamera.transform.rotation, currentTarget.rotation) < 0.1f)
            {
                isMoving = false; // Stop moving the camera
            }
        }
    }
}

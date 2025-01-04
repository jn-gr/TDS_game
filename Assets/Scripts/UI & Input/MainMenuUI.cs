using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    private Transform currentTarget; // The current target position and rotation
    private bool isMoving = false;
    private float soundEffectVolume;
    private float musicVolume;

    [Header("Camera Movement Variables")]
    public Camera mainCamera; // Reference to the main camera (assign via Inspector)
    public Transform[] targetPositions; // Array of target positions and rotations
    public float moveSpeed = 5f; // Speed of camera movement
    public float rotateSpeed = 100f; // Speed of camera rotation

    [Header("Settings Volume Variables")]
    public Image soundEffectButton;
    public Sprite soundEffectMute;
    public Sprite soundEffectUnmute;
    public Image musicButton;
    public Sprite musicMute;
    public Sprite musicUnmute;

    public void PlayGameEasy()
    {
        UserDifficulty.CurrentLevel = DifficultyLevel.Easy;
        SceneLoader.NextSceneName = "Main";
        SceneManager.LoadScene("Loading Screen");
    }

    public void PlayGameMedium()
    {
        UserDifficulty.CurrentLevel = DifficultyLevel.Medium;
        SceneLoader.NextSceneName = "Main";
        SceneManager.LoadScene("Loading Screen");
    }
    public void PlayGameHard()
    {
        UserDifficulty.CurrentLevel = DifficultyLevel.Hard;
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
        // Moves the camera if the camera is told to move.
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

    public void LoadGame()
    {
        Debug.Log("Enter Load Game Logic Here");
    }

    private void Start()
    {
        // Gets volume variables from PlayerPrefs
        soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);

        // Changes volume sprites based on if they're 1 or 0.
        if (soundEffectVolume == 1)
        {
            soundEffectButton.sprite = soundEffectUnmute;
        }
        else
        {
            soundEffectButton.sprite = soundEffectMute;
        }

        if (musicVolume == 1)
        {
            musicButton.sprite = musicUnmute;
        }
        else
        {
            musicButton.sprite = musicMute;
        }
    }

    public void SoundEffectToggle()
    {
        if (soundEffectVolume == 1.0f)
        {
            PlayerPrefs.SetFloat("SoundEffectVolume", 0.0f);
            soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume");
            soundEffectButton.sprite = soundEffectMute;
        }
        else
        {
            PlayerPrefs.SetFloat("SoundEffectVolume", 1.0f);
            soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume");
            soundEffectButton.sprite = soundEffectUnmute;
        }
    }

    public void MusicToggle()
    {
        if (musicVolume == 1.0f)
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.0f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicButton.sprite = musicMute;
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicButton.sprite = musicUnmute;
        }
    }
}
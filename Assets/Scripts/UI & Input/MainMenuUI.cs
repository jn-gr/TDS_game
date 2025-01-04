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

    [Header("Audio Variables")]
    public AudioSource bgmAudioSource; // Reference to the AudioSource for BGM
    public AudioClip bgmClip;          // The AudioClip for the BGM

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
        // Initialize PlayerPrefs with default values if not already set
        if (!PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            PlayerPrefs.SetInt("SoundEffectVolume", 1);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetInt("MusicVolume", 1);
        }
        PlayerPrefs.Save();

        // Update volume sprites based on PlayerPrefs values
        UpdateSoundEffectSprite();
        UpdateMusicSprite();

        // Play or stop BGM based on the MusicVolume
        UpdateBGM();
    }

    public void SoundEffectToggle()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            PlayerPrefs.SetInt("SoundEffectVolume", 0);
            PlayerPrefs.Save();
            soundEffectButton.sprite = soundEffectUnmute;
        }
        else
        {
            PlayerPrefs.SetInt("SoundEffectVolume", 1);
            PlayerPrefs.Save();
            soundEffectButton.sprite = soundEffectMute;

            // Play click sound only when unmuting
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);
        }
    }

    public void MusicToggle()
    {
        if (PlayerPrefs.GetInt("MusicVolume") == 1)
        {
            PlayerPrefs.SetInt("MusicVolume", 0);
            PlayerPrefs.Save();
            musicButton.sprite = musicMute;

            // Stop the BGM
            UpdateBGM();
        }
        else
        {
            PlayerPrefs.SetInt("MusicVolume", 1);
            PlayerPrefs.Save();
            musicButton.sprite = musicUnmute;

            // Play the BGM
            UpdateBGM();

            // Play click sound only if sound effects are enabled
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.UiClick, 0.5f);
            }
        }
    }

    private void UpdateBGM()
    {
        if (PlayerPrefs.GetInt("MusicVolume") == 1)
        {
            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.clip = bgmClip;
                bgmAudioSource.loop = true; // Enable looping
                bgmAudioSource.volume = 0.5f; // Set desired volume
                bgmAudioSource.Play(); // Start playing
            }
        }
        else
        {
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop(); // Stop playing the BGM
            }
        }
    }

    private void UpdateSoundEffectSprite()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            soundEffectButton.sprite = soundEffectMute;
        }
        else
        {
            soundEffectButton.sprite = soundEffectUnmute;
        }
    }

    private void UpdateMusicSprite()
    {
        if (PlayerPrefs.GetInt("MusicVolume") == 1)
        {
            musicButton.sprite = musicMute;
        }
        else
        {
            musicButton.sprite = musicUnmute;
        }
    }
}

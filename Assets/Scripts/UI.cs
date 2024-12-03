using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private GameManager gameManager;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI totalKillsText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI experienceText;

    public Button startWaveButton;
    public GameObject gameOverOverlay;

    public Sprite startWaveSprite; 
    public Sprite waveStartedSprite;

    public GameObject enemiesLeftIcon;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        healthText.text = "Health: " + gameManager.currentHealth;
        goldText.text = "Gold: " + gameManager.currency;
        enemiesLeftText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // Currently checks all info on UI per frame. Gold, health, wave progress, etc.
        // Should be optimised to every time a value changes.
        healthText.text = (gameManager.currentHealth).ToString();
        goldText.text = (gameManager.currency).ToString();
        totalKillsText.text = (gameManager.totalKills).ToString();
        scoreText.text = (gameManager.score).ToString();
        experienceText.text = (gameManager.experience).ToString();

        if (gameManager.waveStarted)
        {
            startWaveButton.image.sprite = waveStartedSprite;
            startWaveButton.interactable = false;
            enemiesLeftIcon.SetActive(true);
            enemiesLeftText.text = (gameManager.enemiesAlive).ToString();
        }
        else
        {
            startWaveButton.image.sprite = startWaveSprite;
            startWaveButton.interactable = true;
            enemiesLeftIcon.SetActive(false);
        }

        if (gameManager.currentHealth <= 0)
        {
            gameOverOverlay.SetActive(true);
        }
    }

    // Button connected to start wave button. Starts wave 1.
    public void StartWave()
    {
        gameManager.StartWave();
        //Debug.Log("button clicked");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

}

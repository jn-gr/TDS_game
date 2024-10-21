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

    public GameObject startWaveButton;
    public GameObject gameOverOverlay;

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
        // Should be optimised to every time value changes.
        healthText.text = "Health: " + gameManager.currentHealth;
        goldText.text = "Gold: " + gameManager.currency;
        totalKillsText.text = "Total kills: " + gameManager.totalKills;
        scoreText.text = "Score: " + gameManager.score;
        experienceText.text = "Experience: " + gameManager.experience;

        if (gameManager.waveStarted)
        {
            startWaveButton.SetActive(false);
            enemiesLeftText.text = "Enemies left in wave: " + (gameManager.enemiesAlive);
        }
        else
        {
            startWaveButton.SetActive(true);
            enemiesLeftText.text = "";
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

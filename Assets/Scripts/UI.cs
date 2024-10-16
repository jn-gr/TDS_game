using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    private GameManager gameManager;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI enemiesLeftText;

    public GameObject startWaveButton;

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
        // Currently checks all info on UI per frame. Should be optimised every time value changes. Maybe should be changed to that instead.
        healthText.text = "Health: " + gameManager.currentHealth;
        goldText.text = "Gold: " + gameManager.currency;

        if (gameManager.waveStarted)
        {
            startWaveButton.SetActive(false);
            enemiesLeftText.text = "Enemies left in wave: " + (gameManager.enemiesInThisWave - gameManager.enemiesDiedThisWave);
        }
        else
        {
            startWaveButton.SetActive(true);
            enemiesLeftText.text = "";
        }
    }

    // Button connected to start wave button. Starts wave 1.
    public void StartWave()
    {
        gameManager.StartWaveOne();
        Debug.Log("button clicked");
    }
}

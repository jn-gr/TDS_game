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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        healthText.text = "Health: " + gameManager.currentHealth;
        goldText.text = "Gold: " + gameManager.currency;
        enemiesLeftText = "";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

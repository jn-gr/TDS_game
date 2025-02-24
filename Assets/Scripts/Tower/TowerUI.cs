using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public TowerManager towerManager;
     // Reference to the UI canvas
    public Button upgradeButton; // Upgrade button
    public Button[] elementButtons;
    

    public Tower activeTower; // Currently selected turret's data

    public void Start()
    {
        towerManager = TowerManager.Instance;
    }
    private void Update()
    {
        if(activeTower != null)
        {
            ShowPanelForTurret(activeTower);
        }
    }
    public void ShowPanelForTurret(Tower tower)
    {
        activeTower = tower;
        gameObject.SetActive(true);
        // if element is picked, dont show element upgrade buttons
        if (activeTower.element != Element.Neutral)
        {
            foreach (Button button in elementButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Button button in elementButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
        // same for tier, if its max level remove the button
        if (activeTower.currentTier == 2)
        {
            upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            upgradeButton.gameObject.SetActive(true);
        }
        UpdatePositionPanel();
    }
    private void UpdatePositionPanel()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(activeTower.transform.position);
        gameObject.GetComponent<RectTransform>().position = screenPosition;
    }

    public void HidePanel()
    {
       
        gameObject.SetActive(false);
        activeTower = null;
    }
    public void UpgradeTower()
    {
        activeTower =activeTower.UpgradeTower();
        towerManager.selectedTower = activeTower;
    }


    public void FireUpgrade()
    {
        activeTower = activeTower.FireUpgrade();
        towerManager.selectedTower = activeTower;
    }
    public void WaterUpgrade()
    {
        activeTower = activeTower.WaterUpgrade();
        towerManager.selectedTower = activeTower;
    }
    public void AirUpgrade()
    {
        activeTower = activeTower.AirUpgrade();
        towerManager.selectedTower = activeTower;
    }
    public void SellTower()
    {
        activeTower.SellTower();
        towerManager.selectedTower = null;
    }
}


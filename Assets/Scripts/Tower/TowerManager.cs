using UnityEngine;
using UnityEngineInternal;

public class TowerManager : MonoBehaviour
{
    public Tower selectedTower; // Currently selected turret
    public TowerUI towerUI; // Reference to the TowerUI script (assigned in Inspector)
    public LayerMask towerLayer;
    public static TowerManager Instance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {           
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;
      
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider is BoxCollider)
                {
                    Tower tower = hit.collider.GetComponent<Tower>();
                    if (tower != null)
                    {
                        if (tower.placed) // this makes sure that we can only select towers that have been placed
                        {
                            SelectTurret(tower);
                            return;
                        }
                    }                  
                }
            }            
            // If no turret is clicked, deselect the current turret
            DeselectTurret();
        }
    }

    private void SelectTurret(Tower turret)
    {
        // Deselect the currently selected turret if it exists
        if (selectedTower != null)
        {
            DeselectTurret();
        }

        // Set the new selected turret and update the UI
        selectedTower = turret;
        towerUI.ShowPanelForTurret(selectedTower.GetComponent<Tower>());
    }

    private void DeselectTurret()
    {
        
        if (selectedTower != null)
        {
            selectedTower = null;
            towerUI.HidePanel();
        }
    }
}

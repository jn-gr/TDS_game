using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeTowerUI : MonoBehaviour
{
    public GameObject panelPrefab;
    private GameObject activePanel;
    public Collider colliderForMouseToClick;

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == colliderForMouseToClick)
                {
                    TogglePanel();
                    return;
                }
            }
        }
    }

    void TogglePanel()
    {
        if (activePanel == null)
        {
            activePanel = Instantiate(panelPrefab, Object.FindFirstObjectByType<Canvas>().transform);
            PositionPanel();
        }
        else
        {
            Destroy(activePanel);
            activePanel = null;
        }
    }

    void PositionPanel()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        RectTransform rectTransform = activePanel.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class TurretInteraction : MonoBehaviour
//{
    
//    public Canvas canvas;
//    [SerializeField] private BoxCollider colliderForMouseToClick;
//    public TurretUI turretUI;
//    public RectTransform uiRectTransform;
//    private void Start()
//    {
//        colliderForMouseToClick = transform.GetComponent<BoxCollider>();
//        turretUI = canvas.GetComponentInChildren<TurretUI>();
//        uiRectTransform = turretUI.gameObject.GetComponent<RectTransform>();
        

//    }
//    void Update()
//    {
//        if (canvas.gameObject.activeSelf)
//        {
//            //Debug.Log("active");
//            updatePositionPanel();
//        }
        
//        //if (EventSystem.current.IsPointerOverGameObject())
//        //{
            
//        //    return;
//        //}
        

//        // Check for mouse click
//        if (Input.GetMouseButtonDown(0))
//        {
//            // make canvas inactive if we click outside the ui
//            if (!IsPointerInsideUI() && canvas.gameObject.activeSelf){
//                canvas.gameObject.SetActive(false);
//                return;
//            }
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit[] hits = Physics.RaycastAll(ray);

//            foreach (RaycastHit hit in hits)
//            {
//                if (hit.collider == colliderForMouseToClick)
//                {
//                    TogglePanel();
//                    return;
//                }
//            }
//        }

//        // Check if the panel should be closed
//        //if (turretUI.gameObject != null && !IsMouseOverUI())
//        //{
//        //    ClosePanelIfMouseLeaves();
//        //}
//    }

//    void TogglePanel()
//    {
//        if (canvas.gameObject.activeSelf)
//        {
//            Debug.Log("click to set false");
//            canvas.gameObject.SetActive(false);
//            //activePanel = Instantiate(panelPrefab, Object.FindFirstObjectByType<Canvas>().transform);
            
//        }
//        else
//        {
//            Debug.Log("click to set true");
            
//            canvas.gameObject.SetActive(true);
//            updatePositionPanel();
//            //Destroy(activePanel);
//            //activePanel = null;
//        }
//    }

//    void updatePositionPanel()
//    {
//        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        
//        uiRectTransform.position = screenPosition;
        
//    }
//    bool IsPointerInsideUI()
//    {
       
//        Vector3 mousePosition = Input.mousePosition;

//        // Check if the mouse is inside the panel's bounds
//        if (RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform, mousePosition, Camera.main))
//        {
//            Debug.Log(uiRectTransform.position);
//            return true;
//        }
//        return false;
//    }

//    bool IsMouseOverUI()
//    {
//        // Check if the mouse is over any UI element
//        return EventSystem.current.IsPointerOverGameObject();
//    }
//}


using UnityEngine;
using UnityEngine.EventSystems;

public class TurretInteraction : MonoBehaviour
{
    public Canvas canvas;
    public BoxCollider colliderForMouseToClick;
    public TurretUI turretUI;
    private RectTransform uiRectTransform;

    void Start()
    {
        
        turretUI = canvas.GetComponentInChildren<TurretUI>();
        colliderForMouseToClick = GetComponent<BoxCollider>();
        
        uiRectTransform = turretUI.gameObject.GetComponent<RectTransform>();

        // Initially hide the UI
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (canvas.gameObject.activeSelf)
        {
            //Debug.Log("active");
            UpdatePositionPanel();
        }
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // If UI is active and we clicked outside any UI element, hide it
            if (canvas.gameObject.activeSelf && !IsClickInsidePanel())
            {
                Debug.Log("clicked outside");
                canvas.gameObject.SetActive(false);
                return;
            }

            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == colliderForMouseToClick)
                {
                    ShowPanel();
                    return;
                }
            }
        }
    }

    void ShowPanel()
    {
        if (!canvas.gameObject.activeSelf)
        {
            Debug.Log("click to set true");
            canvas.gameObject.SetActive(true);
            UpdatePositionPanel();
        }
        
    }
    void UpdatePositionPanel()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        uiRectTransform.position = screenPosition;

    }
    bool IsClickInsidePanel()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Checks if the mousePosition is inside the given RectTransform (our UI panel)
        return RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform, mousePosition, null);
    }

    void OnDrawGizmos()
    {
        Color gizmoColor = Color.green;
        RectTransform rectTransform = uiRectTransform;
        if (rectTransform != null)
        {
            // Get the corners of the RectTransform in world space
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Draw lines between the corners to form the rectangle
            Gizmos.color = gizmoColor;
            for (int i = 0; i < 4; i++)
            {
                Vector3 start = corners[i];
                Vector3 end = corners[(i + 1) % 4]; // Loop back to the first corner
                Gizmos.DrawLine(start, end);
            }
        }
    }
}

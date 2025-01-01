//using UnityEngine;
//using UnityEngine.EventSystems;

//public class TowerInteraction : MonoBehaviour
//{
//    public Canvas canvas;
//    public BoxCollider colliderForMouseToClick;
//    public TowerUI towerUI;
//    private RectTransform uiRectTransform;

//    public LayerMask towerLayer;

//    void Start()
//    {
        
//        towerUI = canvas.GetComponentInChildren<TowerUI>();
//        colliderForMouseToClick = GetComponent<BoxCollider>();
        
//        uiRectTransform = towerUI.gameObject.GetComponent<RectTransform>();

//        // Initially hide the UI
//        canvas.gameObject.SetActive(false);
//    }

//    void Update()
//    {
//        if (canvas.gameObject.activeSelf)
//        {
//            //Debug.Log("active");
//            UpdatePositionPanel();
//        }
//        // Check for left mouse click
//        if (Input.GetMouseButtonDown(0))
//        {
//            if (EventSystem.current.IsPointerOverGameObject())
//            {
//                // Prevents interaction with towers if the click is on a UI element
//                return;
//            }
//            // If UI is active and we clicked outside any UI element, hide it
//            if (canvas.gameObject.activeSelf && !IsClickInsidePanel())
//            {
//                Debug.Log("clicked outside");
//                canvas.gameObject.SetActive(false);
//                return;
//            }

            
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit[] hits = Physics.RaycastAll(ray);

//            foreach (RaycastHit hit in hits)
//            {
//                if (hit.collider == colliderForMouseToClick)
//                {
//                    ShowPanel();
//                    return;
//                }
//            }
//            //RaycastHit hit;

//            //// Check if the raycast hits something
//            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerLayer))
//            //{
//            //    Debug.Log("Raycast hit: " + hit.collider.name);
//            //    if (hit.collider == colliderForMouseToClick)
//            //    {
//            //        ShowPanel();
//            //    }
//            //}
//            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
//        }
//    }

//    public void ShowPanel()
//    {
//        if (!canvas.gameObject.activeSelf)
//        {          
//            canvas.gameObject.SetActive(true);
//            UpdatePositionPanel();
//        }
        
//    }
//    public void HidePanel()
//    {

//    }
//    void UpdatePositionPanel()
//    {
//        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
//        uiRectTransform.position = screenPosition;
//    }
//    bool IsClickInsidePanel()
//    {
//        Vector3 mousePosition = Input.mousePosition;

//        // Checks if the mousePosition is inside the given RectTransform (our UI panel)
//        return RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform, mousePosition, null);
//    }
//}


using UnityEngine;

public class TowerInteraction : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        // Initially hide the UI
        canvas.gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        canvas.gameObject.SetActive(true);
        UpdatePositionPanel();
    }

    public void HidePanel()
    {
        canvas.gameObject.SetActive(false);
    }

    private void UpdatePositionPanel()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        canvas.GetComponent<RectTransform>().position = screenPosition;
    }
}

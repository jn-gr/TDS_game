using UnityEngine;
using UnityEngine.EventSystems;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Find or add a CanvasGroup for controlling visibility
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0; // Start hidden
        canvasGroup.blocksRaycasts = true; // Block raycasts to detect hover
        canvasGroup.interactable = false; // Prevent interaction with the UI itself
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canvasGroup.alpha = 1; // Make visible
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = 0; // Hide visually
    }

    private void Update()
    {
        // During hover, allow clicks to pass through by temporarily disabling raycast blocking
        if (canvasGroup.alpha > 0)
        {
            canvasGroup.blocksRaycasts = false; // Pass clicks to the underlying objects
        }
        else
        {
            canvasGroup.blocksRaycasts = true; // Detect hover when not visible
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normalSprite;
    public Sprite hoverSprite;
    private Image buttonImage;
    private Button button;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        buttonImage.sprite = normalSprite; // Set the default sprite on start
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable) // Check if the button is interactable
        {
            buttonImage.sprite = hoverSprite; // Change sprite on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null && button.interactable) // Check if the button is interactable
        {
            buttonImage.sprite = normalSprite; // Revert back on exit
        }
    }

    public void MakeSpriteNormalAgain()
    {
        buttonImage.sprite = normalSprite;
    }
}

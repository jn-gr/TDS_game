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
        buttonImage.sprite = normalSprite; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable) 
        {
            buttonImage.sprite = hoverSprite; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null && button.interactable) 
        {
            buttonImage.sprite = normalSprite; 
        }
    }

    public void MakeSpriteNormalAgain()
    {
        buttonImage.sprite = normalSprite;
    }
}

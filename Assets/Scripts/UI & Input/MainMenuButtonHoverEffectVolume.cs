using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButtonHoverEffectVolume : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private bool pointerIsOver = false;

    [Tooltip("If false, checks music PlayerPrefs. If true, checks sound effect PlayerPrefs.")]
    public bool isForSoundEffects;

    public Sprite muteSprite;
    public Sprite muteSpriteHover;
    public Sprite unmuteSprite;
    public Sprite unmuteSpriteHover;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        UpdateButtonSprite(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsOver = true;
        UpdateButtonSprite(true); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsOver = false;
        UpdateButtonSprite(false);
    }

    private void Update()
    {
        if (pointerIsOver)
        {
            UpdateButtonSprite(true); 
        }
    }

    private void UpdateButtonSprite(bool isHover)
    {
        string prefsKey = isForSoundEffects ? "SoundEffectVolume" : "MusicVolume";

        if (isHover)
        {
            buttonImage.sprite = (PlayerPrefs.GetInt(prefsKey, 1) == 1) ?  muteSpriteHover : unmuteSpriteHover;
        }
        else
        {
            buttonImage.sprite = (PlayerPrefs.GetInt(prefsKey, 1) == 1) ?  muteSprite : unmuteSprite;
        }
    }
}

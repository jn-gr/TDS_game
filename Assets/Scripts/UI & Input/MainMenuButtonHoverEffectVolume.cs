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
            buttonImage.sprite = (PlayerPrefs.GetFloat(prefsKey, 1.0f) == 1.0f) ? unmuteSpriteHover : muteSpriteHover;
        }
        else
        {
            buttonImage.sprite = (PlayerPrefs.GetFloat(prefsKey, 1.0f) == 1.0f) ? unmuteSprite : muteSprite;
        }
    }
}

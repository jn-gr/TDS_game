using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ToastPanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Coroutine activeAnimation;
    private TextMeshProUGUI messageText;

    [Header("Animation Settings")]
    public float fadeInDuration = 0.5f;
    public float displayDuration = 2.0f;
    public float fadeOutDuration = 0.5f;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        messageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowMessage(string message)
    {
        if (activeAnimation != null)
            StopCoroutine(activeAnimation);

        activeAnimation = StartCoroutine(AnimateMessage(message));
    }

    IEnumerator AnimateMessage(string message)
    {
        // Update text
        messageText.text = message;

        // Fade in
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;

        activeAnimation = null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    private GameManager gameManager;
    private bool isPaused = false;
    private PostProcessVolume blurryCameraEffect;
    private Transform[] allUI;
    private PlayerInput playerInput;
    private InputAction pauseAction;
    private bool isPauseOnCooldown = false;

    [Header("Canvas Game Object")]
    public GameObject canvas;

    [Header("UI Text and Icons")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI totalKillsText;
    public TextMeshProUGUI experienceText;
    public GameObject enemiesLeftIcon;
    public TextMeshProUGUI waveCounter;

    [Header("Hotbar")]
    public Button startWaveButton;
    public Sprite startWaveSprite; 
    public Sprite waveStartedSprite;

    [Header("Game Over")]
    public GameObject gameOverOverlay;
    public float gameOverFadeDuration = 1.0f;

    [Header("Pause Menu")]
    public GameObject pausePanel;
    public float pauseFadeDuration = 0.3f;
    public float pauseCooldownDuration = 0.5f;

    //[Header("Skill Tree Script w/ Logic")]
    //public SkillTree skillTree;

    [Header("Skill Tree")]
    public GameObject activeSkillTreePanel;
    public GameObject passiveSkillTreePanel;
    public float skillTreeFadeDuration = 0.3f;
    public TextMeshProUGUI activeSkillLevelText;
    public TextMeshProUGUI passiveSkill1LevelText;
    public TextMeshProUGUI passiveSkill2LevelText;
    public TextMeshProUGUI passiveSkill3LevelText;
    public TextMeshProUGUI passiveSkill4LevelText;
    public TextMeshProUGUI passiveSkill5LevelText;
    public TextMeshProUGUI passiveSkill6LevelText;

    [Header("Active Skill Button")]
    public Button activeSkillButton;

    [Header("Encyclopedia")]
    public GameObject encyclopediaPanel;
    public float encyclopediaFadeDuration = 0.3f;

    [Header("Game Won")]
    public GameObject gameWonOverlay;
    public float gameWonFadeDuration = 1.0f;

    private void Awake()
    {
        playerInput = Camera.main.GetComponent<PlayerInput>();
        InputActionMap actionMap = playerInput.actions.FindActionMap("RTS Camera");
        pauseAction = actionMap.FindAction("Pause");
    }
    private void OnEnable()
    {
        pauseAction.performed += ctx => TryTogglePause();
        pauseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.performed -= ctx => TryTogglePause();
        pauseAction.Disable();
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        blurryCameraEffect = GetComponent<PostProcessVolume>();
        allUI = canvas.GetComponentsInChildren<Transform>(true).ToArray();
        gameManager.WaveEnded += EndOfWave;
        gameManager.LastWaveCompleted += EndOfLastWave;
    }

    void Update()
    {
        // Checks all info on UI per frame. Gold, health, wave progress, etc.
        healthText.text = (gameManager.currentHealth).ToString();
        goldText.text = (gameManager.currency).ToString();
        totalKillsText.text = (gameManager.totalKills).ToString();
        experienceText.text = (gameManager.experience).ToString();
        waveCounter.text = "Wave: \n" + gameManager.waveNum + WaveNumSlashRemaining();

        if (gameManager.waveStarted)
        {
            enemiesLeftIcon.SetActive(true);
            enemiesLeftText.text = (gameManager.enemiesAlive).ToString();
        }
        else
        {
            enemiesLeftIcon.SetActive(false);
        }

        if (gameManager.currentHealth <= 0)
        {
            GameOverScreen();
        }

        //if (skillTree.isActiveSkillAvailable)
        //{
        //    activeSkillButton.interactable = true;
        //} else
        //{
        //    activeSkillButton.interactable = false;
        //}
    }

    private string WaveNumSlashRemaining()
    {
        if (gameManager.waveNum <= gameManager.GetLastWaveNumber())
        {
            return "/" + gameManager.GetLastWaveNumber();
        }
        else
        {
            return "";
        }
    }

    // Button connected to start wave button. Starts waves.
    public void StartWave()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        gameManager.StartWave();
        startWaveButton.image.sprite = waveStartedSprite;
        startWaveButton.interactable = false;
    }

    public void EndOfWave()
    {
        startWaveButton.image.sprite = startWaveSprite;
        startWaveButton.interactable = true;
    }

    public void BackToMainMenu()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        SceneLoader.NextSceneName = "Main Menu";
        SceneManager.LoadScene("Loading Screen");
    }

    public void SaveAndBackToMainMenu()
    {
        Debug.Log("Put Save And Go Home Logic Here");

        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        SceneLoader.NextSceneName = "Main Menu";
        SceneManager.LoadScene("Loading Screen");
    }

    enum Fade
    {
        In,
        Out
    }

    enum Blurry
    {
        Yes,
        No
    }

    private IEnumerator FadeCanvasGroup(Fade fadeAction, GameObject uiPanel, float fadeDuration, Blurry blurryAction)
    {
        CanvasGroup canvasGroup = uiPanel.GetComponent<CanvasGroup>();
        float startAlpha;
        float endAlpha;

        if (blurryAction == Blurry.Yes)
        {
            blurryCameraEffect.enabled = true;
        }

        if (blurryAction == Blurry.No)
        {
            blurryCameraEffect.enabled = false;
        }

        if (fadeAction == Fade.In)
        {
            uiPanel.gameObject.SetActive(true);
            startAlpha = 0.0f;
            endAlpha = 1.0f;
        }
        else
        {
            startAlpha = 1.0f;
            endAlpha = 0.0f;
        }

        float elapsedTime = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (fadeAction == Fade.Out)
        {
            uiPanel.gameObject.SetActive(false);
        }
    }

    private void TryTogglePause()
    {
        if (!isPauseOnCooldown)
        {
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.UiClick, 0.5f);

            }
            TogglePause();
            StartCoroutine(PauseCooldown());
        }
    }

    private IEnumerator PauseCooldown()
    {
        isPauseOnCooldown = true; // Start cooldown
        yield return new WaitForSecondsRealtime(pauseCooldownDuration); // Wait for cooldown to complete
        isPauseOnCooldown = false; // End cooldown
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.UiClick, 0.5f);

            }
            StartCoroutine(FadeCanvasGroup(Fade.In, pausePanel, pauseFadeDuration, Blurry.Yes));
            Time.timeScale = 0;
        }
        else
        {
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.UiClick, 0.5f);

            }
            StartCoroutine(FadeCanvasGroup(Fade.Out, pausePanel, pauseFadeDuration, Blurry.No));
            Time.timeScale = 1;
        }

    }

    private void GameOverScreen()
    {
        foreach (Transform transform in allUI) {
            if (transform.CompareTag("UI")) {
                transform.gameObject.SetActive(false);
            }
        }
        StartCoroutine(FadeCanvasGroup(Fade.In, gameOverOverlay, gameOverFadeDuration, Blurry.Yes));
    }

    public void SkillTreeOpen()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        StartCoroutine(FadeCanvasGroup(Fade.In, activeSkillTreePanel, skillTreeFadeDuration, Blurry.Yes));
    }

    public void SkillTreeClose()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        StartCoroutine(FadeCanvasGroup(Fade.Out, activeSkillTreePanel, skillTreeFadeDuration, Blurry.No));
        StartCoroutine(FadeCanvasGroup(Fade.Out, passiveSkillTreePanel, skillTreeFadeDuration, Blurry.No));
    }

    public void PassiveSkillTreeOpen()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        CanvasGroup canvasGroup = passiveSkillTreePanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
    }

    public void ToggleEncyclopedia()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
            {
                SoundManager.PlaySound(SoundType.UiClick, 0.5f);

            }
            StartCoroutine(FadeCanvasGroup(Fade.In, encyclopediaPanel, encyclopediaFadeDuration, Blurry.Yes));
            Time.timeScale = 0;
        }
        else
        {
            StartCoroutine(FadeCanvasGroup(Fade.Out, encyclopediaPanel, encyclopediaFadeDuration, Blurry.No));
            Time.timeScale = 1;
        }

    }

    public void RestartLevel()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        SceneLoader.NextSceneName = "Main";
        SceneManager.LoadScene("Loading Screen");
    }

    public void ActiveSkillButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Active Skill Button Clicked");
    }

    public void Skill1ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 1 Button Clicked");
    }

    public void Skill2ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 2 Button Clicked");
    }

    public void Skill3ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 3 Button Clicked");
    }

    public void Skill4ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 4 Button Clicked");
    }

    public void Skill5ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 5 Button Clicked");
    }

    public void Skill6ButtonClicked()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        Debug.Log("Passive Skill 6 Button Clicked");
    }

    public void EndOfLastWave()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        isPaused = true;

        StartCoroutine(FadeCanvasGroup(Fade.In, gameWonOverlay, pauseFadeDuration, Blurry.Yes));
        Time.timeScale = 0;
    }

    public void ContinueToFreeplay()
    {
        if (PlayerPrefs.GetInt("SoundEffectVolume") == 1)
        {
            SoundManager.PlaySound(SoundType.UiClick, 0.5f);

        }
        isPaused = false;

        StartCoroutine(FadeCanvasGroup(Fade.Out, gameWonOverlay, pauseFadeDuration, Blurry.No));
        Time.timeScale = 1;
    }

}

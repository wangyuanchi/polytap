using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class UIManagerScript : MonoBehaviour
{
    [Header("General")]
    public float beatMapStartTime;
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private GameObject PostProcessing;
    private static bool firstAttempt = true;

    [Header("Audio")]
    [SerializeField] private GameObject levelMusic;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioMixer audioMixer;
    private float audioCompletedDuration;

    [Header("Progress")]
    [SerializeField] private TMP_Text progressText;
    private float progressPercentage;
    private Coroutine UpdateProgressPercentageCoroutine;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private RectTransform heartMask;
    private Coroutine animateHeartMaskCoroutine;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject normalModeProgressBar;
    [SerializeField] private GameObject hardModeProgressBar;

    [Header("Game Over")]
    [SerializeField] private GameObject levelCompleteOverlay;
    [SerializeField] private GameObject judgementLines;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseActionReference;

    private void OnEnable()
    {
        pauseActionReference.action.Enable();
        pauseActionReference.action.performed += onPause;
    }

    private void OnDisable()
    {
        pauseActionReference.action.performed -= onPause;
        pauseActionReference.action.Disable();
    }

    private void onPause(InputAction.CallbackContext context)
    {
        if (pauseUI.activeSelf)
        { ResumeScene(); }
        else
        { PauseScene(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Do not load fade in transition for subsequent attempts for better flow
        if (firstAttempt)
        { 
            sceneTransition.GetComponent<SceneTransitionScript>().SceneFadeIn(); 
            firstAttempt = false;
        }
        LoadAudioVolume();
        SetTotalAttempts();
        SetDifficulty();
        SetProgressBar();
        UpdateProgressPercentageCoroutine = StartCoroutine(UpdateProgressPercentage());
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    // Increasing total number of attempts every time the scene is loaded
    private void SetTotalAttempts()
    {
        string key;

        key = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

    private void SetDifficulty()
    {
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            health = 3;
            heartMask.sizeDelta = new Vector2(350f, 100f);
        }
        else
        {
            health = 1;
            heartMask.sizeDelta = new Vector2(110f, 100f);
        }
    }

    // Load and set progress bars
    private void SetProgressBar()
    {
        string levelName = SceneManager.GetActiveScene().name;
        float normalModeHighScore = PlayerPrefs.GetFloat($"{levelName}-N-HS");
        float hardModeHighScore = PlayerPrefs.GetFloat($"{levelName}-H-HS");

        // Set progress bar fill
        normalModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = normalModeHighScore / 100;
        hardModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = hardModeHighScore / 100;

        // Set progress text
        normalModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = normalModeHighScore.ToString() + "%";
        hardModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = hardModeHighScore.ToString() + "%";
    }

    private IEnumerator UpdateProgressPercentage()
    {
        float beatMapEndTime = levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime;

        while (progressPercentage < 100f)
        {
            audioCompletedDuration = Time.time - beatMapStartTime;
            progressPercentage = (float)Math.Round(audioCompletedDuration / beatMapEndTime * 100f, 2);
            progressText.text = $"{progressPercentage}%";

            if (progressPercentage >= 100f)
            {
                // Make sure progressPercentage is at exactly 100
                progressPercentage = 100f;
                progressText.text = $"{progressPercentage}%";
            }

            yield return null;
        }

        // Level complete
        StartCoroutine(GameOver(true));
    }

    public void TakeDamage()
    {
        health--;

        bool enableDamageVignette = PlayerPrefs.GetString("Damage Vignette") == "true" ? true : false;
        if (enableDamageVignette)
        {
            PostProcessing.GetComponent<VignetteScript>().DamageVignette(health);
        }

        // Prevent coroutine clashing if health decreases faster than animation
        if (animateHeartMaskCoroutine != null) 
        {
            StopCoroutine(animateHeartMaskCoroutine);
        }

        if (health == 2)
        {
            animateHeartMaskCoroutine = StartCoroutine(AnimateHeartMask(new Vector2(230f, 100f)));
        }
        else if (health == 1)
        {
            animateHeartMaskCoroutine = StartCoroutine(AnimateHeartMask(new Vector2(110f, 100f)));
        }
        else if (health == 0)
        {
            animateHeartMaskCoroutine = StartCoroutine(AnimateHeartMask(new Vector2(0f, 100f)));
            StopCoroutine(UpdateProgressPercentageCoroutine);
            StartCoroutine(GameOver(false));
        }
    }

    private IEnumerator AnimateHeartMask(Vector2 targetSize)
    {
        float currentTime = 0f;
        float animationTime = 0.3f;
        Vector2 currentSize = heartMask.sizeDelta;

        AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        while (currentTime < animationTime) 
        {
            float lerpFactor = slideEase.Evaluate(currentTime / animationTime);
            heartMask.sizeDelta = Vector2.Lerp(currentSize, targetSize, lerpFactor);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    // When game over happens, the progress is stopped and the audio is paused, but the beatmap still plays for aesthetics
    private IEnumerator GameOver(bool levelComplete)
    {
        SetHighScore();
        SetProgressBar();
        levelMusic.GetComponent<LevelMusicScript>().StopMusic();

        if (levelComplete) 
        {
            pauseActionReference.action.Disable();
            Instantiate(levelCompleteOverlay, transform.position, transform.rotation);
        }
        else
        {
            PlaySFX(gameOverSFX);
            judgementLines.GetComponent<JudgementLinesScript>().GameOver();
            // Wait for 2 seconds before restarting the game
            yield return new WaitForSeconds(2f);
            RestartScene();
        }
    }

    // Setting of high score in player prefs
    private void SetHighScore()
    {
        string key;
        
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            key = SceneManager.GetActiveScene().name + "-N-HS";
        }
        else
        {
            key = SceneManager.GetActiveScene().name + "-H-HS";
        }

        float highScore = PlayerPrefs.GetFloat(key);
        if (highScore < progressPercentage)
        {
            PlayerPrefs.SetFloat(key, progressPercentage);
        }
    }

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }

    private void PauseScene()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        levelMusic.GetComponent<LevelMusicScript>().PauseMusic();
    }

    public void ResumeScene()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
        levelMusic.GetComponent<LevelMusicScript>().ResumeMusic();
    }

    // Restart scene but without the transition
    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void TransitionToScene(string levelName)
    {
        // Reset first attempt so that transition is loaded in the future
        firstAttempt = true;
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }
}

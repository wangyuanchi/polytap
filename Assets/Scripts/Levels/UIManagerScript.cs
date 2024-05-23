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
using Unity.Properties;

public class UIManagerScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private GameObject PostProcessing;
    private static bool firstAttempt = true;

    [Header("Audio")]
    [SerializeField] private GameObject levelMusic;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Progress")]
    [SerializeField] private TMP_Text currentProgressText;
    public float progressPercentage;
    private Coroutine UpdateProgressPercentageCoroutine;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private RectTransform heartMask;
    private Coroutine animateHeartMaskCoroutine;

    [Header("Practice UI")]
    [SerializeField] private GameObject practiceManager;
    [SerializeField] private Button forwardsButton;
    [SerializeField] private Button backwardsButton;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject normalModeProgressBar;
    [SerializeField] private GameObject hardModeProgressBar;

    [Header("Attempts UI")]
    [SerializeField] private GameObject attemptsUI;
    [SerializeField] private TMP_Text attemptsText;

    [Header("Game Over")]
    [SerializeField] private GameObject levelCompleteOverlay;
    [SerializeField] private GameObject newBestOverlay;
    [SerializeField] private GameObject judgementLines;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseActionReference;
    [SerializeField] private GameObject logicManager;

    [Header("Particles")]
    [SerializeField] private ParticleSystem ambientParticles;

    [Header("Background")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite[] backgroundSprites;
    private string[] levelArray = { "L1", "L2" };

    [Header("Accuracy")]
    [SerializeField] private GameObject accuracyUI;
    [SerializeField] private TMP_Text accuracyText;
    private Coroutine displayAccuracyCoroutine;

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
        // Grabs the current level, then references the scriptable object for the background
        string level = StaticInformation.level;
        LevelDataScriptableObject scriptableObjectInstance = (LevelDataScriptableObject)Resources.Load<ScriptableObject>($"LevelData\\{level}");
        background.sprite = scriptableObjectInstance.background;
        LoadAudioVolume();
        LoadDifficulty();
        LoadParticles();
        SetTotalAttempts();
        SetProgressBar();
        UpdateProgressPercentageCoroutine = StartCoroutine(UpdateProgressPercentage());
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    public void LoadDifficulty()
    {
        // If a new checkpoint is set using forward/backward buttons right after a note passes judgement line,
        // loses health and animation is still playing, health may not be full afterwards
        // Hence, stop the animation first.
        if (animateHeartMaskCoroutine != null)
        {
            StopCoroutine(animateHeartMaskCoroutine);
        }

        if (PlayerPrefs.GetString("Mode") == "N")
        {
            health = 3;
            heartMask.sizeDelta = new Vector2(350f, 100f);
            PostProcessing.GetComponent<VignetteScript>().SetVignette(health);
        }
        else
        {
            health = 1;
            heartMask.sizeDelta = new Vector2(110f, 100f);
            PostProcessing.GetComponent<VignetteScript>().SetVignette(health);
        }
    }

    private void LoadParticles()
    {
        if (PlayerPrefs.GetString("Particles") == "true")
        { ambientParticles.Play(); }
    }

    // Increasing total number of attempts every time the scene is loaded
    private void SetTotalAttempts()
    {
        // [PRACTICE MODE] Do not increase attempts count or show attempts text if in practice mode
        if (PracticeManagerScript.practiceMode == true)
        {
            attemptsUI.SetActive(false);
            return;
        }

        string key;

        key = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
        int totalAttempts = PlayerPrefs.GetInt(key) + 1; // Includes the current attempt
        PlayerPrefs.SetInt(key, totalAttempts);

        // Set attemps text
        if (PlayerPrefs.GetString("Attempts") == "true")
        {
            attemptsUI.SetActive(true);
            attemptsText.text = $"Attempt {totalAttempts}";
        }
        else
        {
            attemptsUI.SetActive(false);
        }
    }

    // Load and set progress bars
    private void SetProgressBar()
    {
        string levelName = SceneManager.GetActiveScene().name;
        float normalModeHighScore = PlayerPrefs.GetFloat($"{levelName}-N-HS");
        float hardModeHighScore = PlayerPrefs.GetFloat($"{levelName}-H-HS");
        int normalModeAttempts = PlayerPrefs.GetInt($"{levelName}-N-TA");
        int hardModeAttempts = PlayerPrefs.GetInt($"{levelName}-H-TA");

        // Set progress bar fill
        normalModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = normalModeHighScore / 100;
        hardModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = hardModeHighScore / 100;

        // Set progress text
        normalModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{normalModeHighScore}% ({normalModeAttempts})";
        hardModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{hardModeHighScore}% ({hardModeAttempts})";
    }

    private IEnumerator UpdateProgressPercentage()
    {
        float beatMapEndTime = levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime;

        while (progressPercentage < 100f)
        {
            float checkpointTimeStamp = PracticeManagerScript.checkpointTimeStamp;
            float checkpointPercentage = (float)Math.Round(checkpointTimeStamp / beatMapEndTime * 100f, 2);

            float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
            progressPercentage = (float)Math.Round(currentTimeStamp / beatMapEndTime * 100f, 2);

            // [PRACTICE MODE] Include checkpoint percentage if in practice mode
            currentProgressText.text = PracticeManagerScript.practiceMode ?
                $"{checkpointPercentage}% - \n{progressPercentage}%" : $"{progressPercentage}%";

            if (progressPercentage >= 100f)
            {
                // Make sure progressPercentage is at exactly 100
                progressPercentage = 100f;

                // [PRACTICE MODE] Include checkpoint percentage if in practice mode
                currentProgressText.text = PracticeManagerScript.practiceMode ? 
                    $"{checkpointPercentage}% - \n{progressPercentage}%" : $"{progressPercentage}%";
            }

            yield return null;
        }

        // Level complete
        StartCoroutine(GameOver(true));
    }

    public void TakeDamage()
    {
        health--;
        PostProcessing.GetComponent<VignetteScript>().SetVignette(health);

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
        bool newHighScore = SetHighScore();
        if (newHighScore)
        {
            SetProgressBar();
            if (!levelComplete)
            { 
                Instantiate(newBestOverlay, transform.position, transform.rotation); 
            }
        }
        
        // Allows music to continue at level complete screen
        if (!levelComplete) 
        { 
            levelMusic.GetComponent<LevelMusicScript>().StopMusic();
        }

        logicManager.GetComponent<LogicManagerScript>().DisableShapeInputs();

        // [PRACTICE MODE] Disable forward/backward button pressing 
        practiceManager.GetComponent<PracticeManagerScript>().DisablePracticeButtons();

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
    private bool SetHighScore()
    {
        // [PRACTICE MODE] Do not set any high scores if in practice mode
        if (PracticeManagerScript.practiceMode == true) return false;

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
            return true;
        }

        return false;
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
        StopCoroutine(UpdateProgressPercentageCoroutine); // Fix bug where the checkpoint percentage would blink out before scene restarts
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void TransitionToScene(string levelName)
    {
        StopCoroutine(UpdateProgressPercentageCoroutine); // Fix bug where the checkpoint percentage would blink out before scene restarts
        // Reset first attempt so that transition is loaded in the future
        firstAttempt = true;
        // [PRACTICE MODE] Reset so that practice and checkpoint is not loaded in the future
        PracticeManagerScript.practiceMode = false;
        PracticeManagerScript.checkpointTimeStamp = 0f;
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }

    // [PRACTICE MODE]
    public void TogglePracticeMode()
    {
        PracticeManagerScript.practiceMode = !PracticeManagerScript.practiceMode;
        PracticeManagerScript.checkpointTimeStamp = 0f; // Checkpoint should be 0f regardless of entering or exiting practice mode
        RestartScene();
    }

    // [PRACTICE MODE] For pressing the reset button in pause UI or level complete UI,
    // because putting this in the normal RestartScene() function will conflict with the normal restarting of scene after game over
    public void PracticeCheckpointReset()
    {
        PracticeManagerScript.checkpointTimeStamp = 0f;
    }

    public void UpdateAccuracyText(float accuracy, float expectedWindow, bool bypass)
    {
        // Do not show accuracy for unexpected inputs, with exception to noteSquareEnd
        if (!bypass)
        {
            // expectedWindow is smaller for a note that requires higher accuracy
            if (Math.Abs(accuracy) > expectedWindow) 
            {
                return;
            }
        }

        // Convert from seconds to milliseconds
        accuracy = (float)Math.Round(accuracy * 1000, 0);

        if (accuracy > 0)
        {
            accuracyText.text = $"+{accuracy}ms";
            accuracyText.color = Color.green;
        }
        else
        {
            accuracyText.text = $"{accuracy}ms"; // Don't need "-" because it is already included
            accuracyText.color = Color.red;
        }

        // If this function is ever called, "Accuracy" player preference must be set to true, hence, can set accuracyUI to be active
        accuracyUI.SetActive(true);

        if (displayAccuracyCoroutine != null)
        {
            StopCoroutine(displayAccuracyCoroutine);
        }
        displayAccuracyCoroutine = StartCoroutine(DisplayAccuracyText());
    }

    // Displays the accuracy text if there is a constant input, and stops displaying if no input is detected for waitTime
    private IEnumerator DisplayAccuracyText()
    {
        float waitTime = 1f;
        yield return new WaitForSeconds(waitTime);
        accuracyUI.SetActive(false);
    }
}

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

public class UIManagerScript : MonoBehaviour
{
    [Header("General")]
    public float beatMapStartTime;
    [SerializeField] private GameObject sceneTransition;

    [Header("Audio")]
    [SerializeField] private GameObject levelMusic;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Health and Progress")]
    [SerializeField] private int health;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite HeartEmpty;
    [SerializeField] private Sprite HeartFull;
    [SerializeField] private TMP_Text progressText;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject normalModeProgressBar;
    [SerializeField] private GameObject hardModeProgressBar;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject levelCompleteOverlay;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseActionReference;

    private float audioCompletedDuration;
    private float progressPercentage;

    private Coroutine UpdateProgressPercentageCoroutine;

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
        LoadAudioVolume();
        SetDifficulty();
        SetProgressBar();
        UpdateProgressPercentageCoroutine = StartCoroutine(UpdateProgressPercentage());
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    private void SetDifficulty()
    {
        if (PlayerPrefs.GetString("Hard Mode") == "false")
        {
            health = 3;
        }
        else
        {
            health = 1;
            hearts[1].sprite = HeartEmpty;
            hearts[2].sprite = HeartEmpty;
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
        float musicDuration = levelMusic.GetComponent<LevelMusicScript>().GetMusicLength();

        while (progressPercentage < 100f)
        {
            audioCompletedDuration = Time.time - beatMapStartTime;
            progressPercentage = (float)Math.Round(audioCompletedDuration / musicDuration * 100f, 2);
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

    public void DecreaseHealth()
    {
        health--;

        // Change sprite of hearts based on health
        for (int heart = 0; heart < hearts.Count(); heart++)
        {
            if (heart < health)
            {
                hearts[heart].sprite = HeartFull;
            }
            else
            {
                hearts[heart].sprite = HeartEmpty;
            }
        }

        // End the game if no health is left
        if (health == 0)
        {
            StopCoroutine(UpdateProgressPercentageCoroutine);
            StartCoroutine(GameOver(false));
        }
    }

    // When game over happens, the progress is stopped and the audio is paused, but the beatmap still plays for aesthetics
    private IEnumerator GameOver(bool levelComplete)
    {
        SetHighScore();
        SetProgressBar();

        if (levelComplete) 
        {
            Instantiate(levelCompleteOverlay, transform.position, transform.rotation);
        }
        else
        {
            gameOverText.text = "Game Over!" + Environment.NewLine + $"Progress: {progressPercentage}%";
            gameOverUI.SetActive(true);
        }

        levelMusic.GetComponent<LevelMusicScript>().StopMusic();

        // Pause for 3 seconds before restarting the scene
        if (!levelComplete)
        {
            yield return new WaitForSeconds(3);
            RestartScene();
        }
    }

    // Setting of high score in player prefs
    private void SetHighScore()
    {
        string key;
        
        if (PlayerPrefs.GetString("Hard Mode") == "false")
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

        // Prevent clash where audio resumes if user pauses then unpauses after the game has ended
        if (!gameOverUI.activeSelf)
        { levelMusic.GetComponent<LevelMusicScript>().ResumeMusic(); }
    }

    public void RestartScene()
    {
        TransitionToScene(SceneManager.GetActiveScene().name);
    }    

    public void TransitionToScene(string levelName)
    {
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }
}

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
    public int health;
    public Image[] hearts;
    public Sprite HeartEmpty;
    public Sprite HeartFull;

    public GameObject AudioManager;
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public TMP_Text gameOverText;
    public GameObject restartSoonText;
    public TMP_Text progressText;

    public AudioMixer audioMixer;
    public Slider musicSlider;

    public float beatMapStartTime;

    private float audioTotalDuration;
    private float audioCompletedDuration;
    private float progressPercentage;

    [SerializeField]
    private InputActionReference pauseActionReference;

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
        setDifficulty();

        // Load and set the music volume
        musicSlider.value = PlayerPrefs.GetFloat("Music Volume");
        SetMusicVolume();

        audioTotalDuration = AudioManager.GetComponent<AudioManagerScript>().musicClip.length;
    }

    // Update is called once per frame
    void Update()
    {
        // Do not increase progress if game is over
        if (!gameOverUI.activeSelf)
        {
            updateProgressPercentage();

            if (progressPercentage < 100f)
            {
                progressText.text = $"{progressPercentage}%";
            }
            else
            {
                // Make sure progressPercentage is at exactly 100
                progressPercentage = 100f;
                progressText.text = $"{progressPercentage}%";
                StartCoroutine(GameOver(true));
            }
        }
    }

    void setDifficulty()
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

    void updateProgressPercentage()
    {
        audioCompletedDuration = Time.time - beatMapStartTime;
        progressPercentage = (float) Math.Round(audioCompletedDuration / audioTotalDuration * 100f, 2);
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
            StartCoroutine(GameOver(false));
        }
    }

    // When game over happens, the progress is stopped and the audio is paused, but the beatmap still plays for aesthetics
    IEnumerator GameOver(bool levelComplete)
    {
        if (levelComplete)
        {
            gameOverText.text = "Level Complete!";
            restartSoonText.SetActive(false);
        }
        else
        {
            gameOverText.text = "Game Over!" + Environment.NewLine + $"Progress: {progressPercentage}%";
        }

        AudioManager.GetComponent<AudioManagerScript>().StopMusic();
        gameOverUI.SetActive(true);

        SetHighScore();

        // Pause for 3 seconds before restarting the scene
        if (!levelComplete)
        {
            yield return new WaitForSeconds(3);
            RestartScene();
        }
    }

    // Setting of high score in player prefs
    void SetHighScore()
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

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PauseScene()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        AudioManager.GetComponent<AudioManagerScript>().PauseMusic();
    }

    public void ResumeScene()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;

        // Prevent clash where audio resumes if user pauses then unpauses after the game has ended
        if (!gameOverUI.activeSelf)
        { 
            AudioManager.GetComponent<AudioManagerScript>().ResumeMusic();
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneName); 
    }

    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 25);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteScript : MonoBehaviour
{ 
    [Header("Overlay")]
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI levelInfoText;
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioClip levelCompleteNSFX;
    [SerializeField] private AudioClip levelCompleteHSFX;
    [SerializeField] private AudioClip levelCompleteASFX;

    private GameObject UIManager;

    void Awake()
    {
        UIManager = GameObject.Find("UIManager");

        SetLevelCompleteText();
        SetLevelInfoText();
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            PlaySFX(levelCompleteNSFX);
            if (!PracticeManagerScript.practiceMode)
            {
                animator.SetTrigger("LevelCompleteN");
            }
            else
            {
                animator.SetTrigger("PracticeCompleteN");
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "H")
        {
            PlaySFX(levelCompleteHSFX);
            if (!PracticeManagerScript.practiceMode)
            {
                animator.SetTrigger("LevelCompleteH");
            }
            else
            {
                animator.SetTrigger("PracticeCompleteH");
            }
        }
        else
        {
            PlaySFX(levelCompleteASFX);
            if (!PracticeManagerScript.practiceMode)
            {
                animator.SetTrigger("LevelCompleteA");
            }
            else
            {
                animator.SetTrigger("PracticeCompleteA");
            }
        }
    }

    private void SetLevelCompleteText()
    {
        if (!PracticeManagerScript.practiceMode)
        {
            levelCompleteText.text = "Level Complete!";
        }
        else
        {
            levelCompleteText.text = "Practice Complete!";
        }
    }

    private void SetLevelInfoText()
    {
        if (!PracticeManagerScript.practiceMode)
        {
            if (PlayerPrefs.GetString("Mode") == "A")
            {
                float currentAccuracy = UIManager.GetComponent<UIManagerScript>().currentAccuracy;
                string bestAccuracyKey = StaticInformation.level + "-" + PlayerPrefs.GetString("Mode") + "-HS";
                float bestAccuracy = PlayerPrefs.GetFloat(bestAccuracyKey);
                levelInfoText.fontSize = 60;
                levelInfoText.text = $"Accuracy: {currentAccuracy}% \t Best: {bestAccuracy}%";
            }
            else
            { 
                string mode = PlayerPrefs.GetString("Mode") == "N" ? "Normal" : "Hard";
                string attemptsKey = StaticInformation.level + "-" + PlayerPrefs.GetString("Mode") + "-TA";
                string attempts = PlayerPrefs.GetInt(attemptsKey).ToString();
                levelInfoText.fontSize = 60;
                levelInfoText.text = $"Mode: {mode} \t Attempts: {attempts}";
            }
        }
        else
        {
            levelInfoText.fontSize = 50;
            levelInfoText.text = "Try to beat the level without checkpoints!";
        }
    }

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }

    public void RestartScene()
    {
        UIManager.GetComponent<UIManagerScript>().RestartScene();
    }

    public void TransitionToScene(string levelName)
    {
        UIManager.GetComponent<UIManagerScript>().TransitionToScene(levelName);
    }

    // [PRACTICE MODE] For pressing the reset button in pause UI or level complete UI,
    // because putting this in the normal RestartScene() function will conflict with the normal restarting of scene after game over
    public void PracticeCheckpointReset()
    {
        PracticeManagerScript.checkpointTimeStamp = 0f;
    }

    public void ExitPracticeMode()
    {
        // [PRACTICE MODE] Reset so that practice and checkpoint is not loaded in the future
        PracticeManagerScript.practiceMode = false;
        PracticeManagerScript.checkpointTimeStamp = 0f;
        RestartScene();
    }
}

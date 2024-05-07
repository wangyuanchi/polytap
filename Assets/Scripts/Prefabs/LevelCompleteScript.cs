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

    private GameObject UIManager;

    void Awake()
    {
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
        else
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

        UIManager = GameObject.Find("UIManager");
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
            string mode = PlayerPrefs.GetString("Mode") == "N" ? "Normal" : "Hard";
            string attemptsKey = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
            string attempts = PlayerPrefs.GetInt(attemptsKey).ToString();
            levelInfoText.fontSize = 65;
            levelInfoText.text = $"Mode: {mode} \t Attempts: {attempts}";
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

    public void ExitPracticeMode()
    {
        // [PRACTICE MODE] Reset so that practice and checkpoint is not loaded in the future
        PracticeManagerScript.practiceMode = false;
        PracticeManagerScript.checkpointTimeStamp = 0f;
        RestartScene();
    }
}

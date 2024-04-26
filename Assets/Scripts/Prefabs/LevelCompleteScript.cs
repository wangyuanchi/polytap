using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteScript : MonoBehaviour
{ 
    [Header("Overlay")]
    [SerializeField] private TextMeshProUGUI levelInfoText;
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioClip levelCompleteNSFX;
    [SerializeField] private AudioClip levelCompleteHSFX;

    void Awake()
    {
        SetLevelInfoText();
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            PlaySFX(levelCompleteNSFX);
            animator.SetTrigger("LevelCompleteN");
        }
        else
        {
            PlaySFX(levelCompleteHSFX);
            animator.SetTrigger("LevelCompleteH");
        }
    }

    private void SetLevelInfoText()
    {
        string mode = PlayerPrefs.GetString("Mode") == "N" ? "Normal" : "Hard";
        string attemptsKey = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
        string attempts = PlayerPrefs.GetInt(attemptsKey).ToString();
        levelInfoText.text = $"Mode: {mode} \t Attempts: {attempts}";
    }

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }

    public void RestartScene()
    {
        TransitionToScene(SceneManager.GetActiveScene().name);
    }

    public void TransitionToScene(string levelName)
    {
        GameObject sceneTransition = GameObject.FindGameObjectWithTag("SceneTransition");
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }
}

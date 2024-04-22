using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI levelInfoText;

    void Awake()
    {
        SetLevelInfoText();
    }

    private void SetLevelInfoText()
    {
        string mode = PlayerPrefs.GetString("Mode") == "N" ? "Normal" : "Hard";
        string attemptsKey = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
        string attempts = PlayerPrefs.GetInt(attemptsKey).ToString();
        levelInfoText.text = $"Mode: {mode} \t Attempts: {attempts}";
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

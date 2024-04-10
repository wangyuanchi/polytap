using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void QuitGame()
    { Application.Quit(); }

    // Called once per lifetime
    void Awake()
    {
        // DO NOT UNCOMMENT THIS LINE
        // PlayerPrefs.DeleteAll();

        // Setting of player preferences
        PlayerPrefs.SetFloat("Music Volume", PlayerPrefs.GetFloat("Music Volume", 0.5f));
        PlayerPrefs.SetString("Hard Mode", PlayerPrefs.GetString("Hard Mode", "false"));

        PlayerPrefs.SetFloat("L1-N-HS", PlayerPrefs.GetFloat("L1-N-HS", 0f));
        PlayerPrefs.SetFloat("L1-H-HS", PlayerPrefs.GetFloat("L1-H-HS", 0f));
    }

    public void TransitionToScene(string levelName)
    {
        SceneTransitionScript.instance.TransitionToScene(levelName);
    }
}

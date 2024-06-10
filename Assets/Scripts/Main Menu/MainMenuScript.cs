using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private AudioMixer audioMixer;

    // Called once per lifetime
    void Awake()
    {
        SetAllPreferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneTransition.GetComponent<SceneTransitionScript>().SceneFadeIn();
        LoadAudioVolume();
    }

    // Setting of player preferences
    private void SetAllPreferences()
    {
        // General
        PlayerPrefs.SetInt("Note Speed", PlayerPrefs.GetInt("Note Speed", 5)); // // Units: arbitrary
        PlayerPrefs.SetFloat("Practice Skip Duration", PlayerPrefs.GetFloat("Practice Skip Duration", 5f)); // Units: seconds
        PlayerPrefs.SetInt("Global Offset", PlayerPrefs.GetInt("Global Offset", 0)); // Units: milliseconds
        PlayerPrefs.SetString("Vignette", PlayerPrefs.GetString("Vignette", "true"));
        PlayerPrefs.SetString("Particles", PlayerPrefs.GetString("Particles", "true"));
        PlayerPrefs.SetString("Accuracy", PlayerPrefs.GetString("Accuracy", "false"));
        PlayerPrefs.SetString("Attempts", PlayerPrefs.GetString("Attempts", "true"));
        PlayerPrefs.SetString("Logs", PlayerPrefs.GetString("Logs", "false"));
        PlayerPrefs.SetFloat("Music Volume", PlayerPrefs.GetFloat("Music Volume", 0.5f));
        PlayerPrefs.SetFloat("SFX Volume", PlayerPrefs.GetFloat("SFX Volume", 0.5f));
        PlayerPrefs.SetString("Lobby Music", PlayerPrefs.GetString("Lobby Music", "true"));
        PlayerPrefs.SetString("Mode", PlayerPrefs.GetString("Mode", "N"));

        // L1
        PlayerPrefs.SetFloat("L1-N-HS", PlayerPrefs.GetFloat("L1-N-HS", 0f));
        PlayerPrefs.SetFloat("L1-H-HS", PlayerPrefs.GetFloat("L1-H-HS", 0f));
        PlayerPrefs.SetInt("L1-N-TA", PlayerPrefs.GetInt("L1-N-TA", 0));
        PlayerPrefs.SetInt("L1-H-TA", PlayerPrefs.GetInt("L1-H-TA", 0));

        // L2
        PlayerPrefs.SetFloat("L2-N-HS", PlayerPrefs.GetFloat("L2-N-HS", 0f));
        PlayerPrefs.SetFloat("L2-H-HS", PlayerPrefs.GetFloat("L2-H-HS", 0f));
        PlayerPrefs.SetInt("L2-N-TA", PlayerPrefs.GetInt("L2-N-TA", 0));
        PlayerPrefs.SetInt("L2-H-TA", PlayerPrefs.GetInt("L2-H-TA", 0));

        //L3
        PlayerPrefs.SetFloat("L3-N-HS", PlayerPrefs.GetFloat("L3-N-HS", 0f));
        PlayerPrefs.SetFloat("L3-H-HS", PlayerPrefs.GetFloat("L3-H-HS", 0f));
        PlayerPrefs.SetInt("L3-N-TA", PlayerPrefs.GetInt("L3-N-TA", 0));
        PlayerPrefs.SetInt("L3-H-TA", PlayerPrefs.GetInt("L3-H-TA", 0));

        // L4
        PlayerPrefs.SetFloat("L4-N-HS", PlayerPrefs.GetFloat("L4-N-HS", 0f));
        PlayerPrefs.SetFloat("L4-H-HS", PlayerPrefs.GetFloat("L4-H-HS", 0f));
        PlayerPrefs.SetInt("L4-N-TA", PlayerPrefs.GetInt("L4-N-TA", 0));
        PlayerPrefs.SetInt("L4-H-TA", PlayerPrefs.GetInt("L4-H-TA", 0));

        // L5
        PlayerPrefs.SetFloat("L5-N-HS", PlayerPrefs.GetFloat("L5-N-HS", 0f));
        PlayerPrefs.SetFloat("L5-H-HS", PlayerPrefs.GetFloat("L5-H-HS", 0f));
        PlayerPrefs.SetInt("L5-N-TA", PlayerPrefs.GetInt("L5-N-TA", 0));
        PlayerPrefs.SetInt("L5-H-TA", PlayerPrefs.GetInt("L5-H-TA", 0));

        // L6
        PlayerPrefs.SetFloat("L6-N-HS", PlayerPrefs.GetFloat("L6-N-HS", 0f));
        PlayerPrefs.SetFloat("L6-H-HS", PlayerPrefs.GetFloat("L6-H-HS", 0f));
        PlayerPrefs.SetInt("L6-N-TA", PlayerPrefs.GetInt("L6-N-TA", 0));
        PlayerPrefs.SetInt("L6-H-TA", PlayerPrefs.GetInt("L6-H-TA", 0));
    }

    public void ResetAllPreferences()
    {
        Debug.Log("Preferences Reset!");
        PlayerPrefs.DeleteAll();
        SetAllPreferences();
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    public void TransitionToScene(string levelName)
    {
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }

    public void QuitGame()
    { Application.Quit(); }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorManagerScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Levels")]
    [SerializeField] private GameObject allLevelsGameObject;
    [SerializeField] private AudioClip levelStartSFX;

    // Start is called before the first frame update
    void Start()
    {
        sceneTransition.GetComponent<SceneTransitionScript>().SceneFadeIn();
        LoadAudioVolume();

        foreach (Transform level in allLevelsGameObject.transform)
        {
            // Remove this after all levels have been made
            if (level.name != "L1")
            {
                continue;
            }
            LoadProgressBars(level);
        }
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    private void LoadProgressBars(Transform level)
    {
        string levelName = level.name;
        float normalModeHighScore = PlayerPrefs.GetFloat($"{levelName}-N-HS");
        float hardModeHighScore = PlayerPrefs.GetFloat($"{levelName}-H-HS");
        int normalModeAttempts = PlayerPrefs.GetInt($"{levelName}-N-TA");
        int hardModeAttempts = PlayerPrefs.GetInt($"{levelName}-H-TA");

        GameObject levelGameObject = level.gameObject;
        GameObject normalModeProgressBar = levelGameObject.transform.Find("NormalModeProgressBar").gameObject;
        GameObject hardModeProgressBar = levelGameObject.transform.Find("HardModeProgressBar").gameObject;

        // Set progress bar fill
        normalModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = normalModeHighScore / 100;
        hardModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = hardModeHighScore / 100;

        // Set progress text and total attempts
        normalModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{normalModeHighScore}% ({normalModeAttempts})";
        hardModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{hardModeHighScore}% ({hardModeAttempts})";
    }

    public void TransitionToScene(string levelName)
    {
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
        // Sets the level in a static script so that the managers can reference it for the scriptable objects
        StaticInformation.level = levelName;

        // If not going back to main menu, fade music out and destroy music object
        if (levelName != "Main Menu")
        {
            LobbyMusicScript.instance.MusicFadeOutAndDestroy();
            PlaySFX(levelStartSFX);
        }   
    }    

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsManagerScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Navigation")]
    [SerializeField] private Button NextButton;
    [SerializeField] private Button PrevButton;
    [SerializeField] private GameObject GameplayInterface;
    [SerializeField] private GameObject EffectsAndStatsInterface;
    [SerializeField] private GameObject KeybindsInterface;
    [SerializeField] private GameObject AudioInterface;
    [SerializeField] private int totalPage;
    [SerializeField] private int currentPage;

    // Start is called before the first frame update
    void Start()
    {
        sceneTransition.GetComponent<SceneTransitionScript>().SceneFadeIn();
        LoadAudioVolume();

        // Make sure correct page at scene start
        ShowPage(1);
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

    public void Next()
    {
        currentPage++;
        ShowPage(currentPage);
    }

    public void Prev()
    {
        currentPage--;
        ShowPage(currentPage);
    }

    private void ShowPage(int currentPage)
    {
        // Prev and Next Buttons
        NextButton.interactable = true;
        PrevButton.interactable = true;

        // Interfaces
        GameplayInterface.SetActive(false);
        EffectsAndStatsInterface.SetActive(false);
        KeybindsInterface.SetActive(false);
        AudioInterface.SetActive(false);

        if (currentPage == 1)
        {
            GameplayInterface.SetActive(true);
            PrevButton.interactable = false;
        }
        else if (currentPage == 2)
        {
            EffectsAndStatsInterface.SetActive(true);
        }
        else if (currentPage == 3)
        {
            KeybindsInterface.SetActive(true);
        }
        else if (currentPage == 4)
        {
            AudioInterface.SetActive(true);
            NextButton.interactable= false;
        }
    }
}

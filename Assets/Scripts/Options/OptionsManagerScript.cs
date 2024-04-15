using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManagerScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Navigation")]
    [SerializeField] private GameObject NextButton;
    [SerializeField] private GameObject PrevButton;
    [SerializeField] private GameObject GameplayInterface;
    [SerializeField] private GameObject KeybindsInterface;
    [SerializeField] private GameObject AudioInterface;
    [SerializeField] private int totalPage;
    [SerializeField] private int currentPage;

    // Start is called before the first frame update
    void Start()
    {
        LoadAudioVolume();
        PrevButton.SetActive(false);
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

        if (currentPage != 1)
        {
            PrevButton.SetActive(true);
        }

        if (currentPage == totalPage)
        {
            NextButton.SetActive(false);
        }
    }

    public void Prev()
    {
        currentPage--;
        ShowPage(currentPage);

        if (currentPage == 1)
        {
            PrevButton.SetActive(false);
        }

        if (currentPage != totalPage)
        {
            NextButton.SetActive(true);
        }
    }

    private void ShowPage(int currentPage)
    {
        GameplayInterface.SetActive(false);
        KeybindsInterface.SetActive(false);
        AudioInterface.SetActive(false);

        if (currentPage == 1)
        {
            GameplayInterface.SetActive(true);
        }
        else if (currentPage == 2)
        {
            KeybindsInterface.SetActive(true);
        }
        else if (currentPage == 3)
        {
            AudioInterface.SetActive(true);
        }
    }
}

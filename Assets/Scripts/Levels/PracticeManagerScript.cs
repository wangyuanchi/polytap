using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeManagerScript : MonoBehaviour
{
    [Header("Practice Mode")]
    public static bool practiceMode = false;
    public float practiceTimeSkipped = 0f; // Linked to amount of time skipped in beatmap and music
                                           // Defaulted to start at 0.00%  

    [Header("Managers")]
    [SerializeField] private GameObject UIManager;
    [SerializeField] private GameObject levelMusic;
    [SerializeField] private GameObject notesManager;

    [Header("Pause UI")]
    [SerializeField] private Sprite practiceEnabled;
    [SerializeField] private Sprite practiceDisabled;
    [SerializeField] private GameObject practiceButton;

    [Header("Practice UI")]
    [SerializeField] private GameObject practiceUI;
    [SerializeField] private Button forwardsButton;
    [SerializeField] private Button backwardsButton;

    void Start()
    {
        if (practiceMode)   
        {
            practiceButton.GetComponent<Image>().sprite = practiceDisabled;
        }
        else
        {
            practiceButton.GetComponent<Image>().sprite = practiceEnabled;
        }
        LoadPracticeUI();
        TimeOutOfRangeCheck();  
    }

    private void LoadPracticeUI()
    {
        // Activate buttons because they are disabled during game over or level complete
        forwardsButton.interactable = true;
        backwardsButton.interactable = true;

        if (practiceMode == false)
        {
            practiceUI.SetActive(false);
        }
        else
        {
            practiceUI.SetActive(true);
        }
    }

    private void TimeOutOfRangeCheck()
    {
        if (practiceTimeSkipped + 5f > levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime)
        {
            forwardsButton.interactable = false;
        }
        else
        {
            forwardsButton.interactable = true;
        }

        if (practiceTimeSkipped - 5f < 0)
        {
            backwardsButton.interactable = false;
        }
        else
        {
            backwardsButton.interactable = true;
        }
    }

    public void TogglePracticeMode()
    {
        practiceMode = !practiceMode;
        UIManager.GetComponent<UIManagerScript>().RestartScene();
    }

    public void OnPracticeTimeSkippedForward()
    {
        practiceTimeSkipped += 5f;
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(practiceTimeSkipped);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(practiceTimeSkipped);
        TimeOutOfRangeCheck();
    }

    public void OnPracticeTimeSkippedBackwards()
    {
        practiceTimeSkipped -= 5f;
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(practiceTimeSkipped);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(practiceTimeSkipped);
        TimeOutOfRangeCheck();
    }
}

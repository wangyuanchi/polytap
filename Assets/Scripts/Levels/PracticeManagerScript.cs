using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeManagerScript : MonoBehaviour
{
    [Header("Practice Mode")]
    public static bool practiceMode = false;
    public static float checkpointTimeStamp = 0f; // Defaulted to start at 0f
                                                  // Linked to amount of time skipped in beatmap and music

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

    private Coroutine TOORCCoroutine;

    void Start()
    {
        if (practiceMode)   
        {
            practiceButton.GetComponent<Image>().sprite = practiceDisabled;
            PracticeCheckpointLoad(); // Plays the beatmap and music at default 0.00% initially
            TOORCCoroutine = StartCoroutine(TimeOutOfRangeCheck()); 
        }
        else
        {
            practiceButton.GetComponent<Image>().sprite = practiceEnabled;
        }
        LoadPracticeUI(practiceMode);
    }

    private void LoadPracticeUI(bool isPracticeMode)
    {
        // (Re)activate buttons because they are disabled during game over or level complete
        forwardsButton.interactable = isPracticeMode;
        backwardsButton.interactable = isPracticeMode;
        practiceUI.SetActive(isPracticeMode);
    }

    private IEnumerator TimeOutOfRangeCheck()
    {
        // Make sure that buttons are disabled the instant they would go out of range
        while (true)
        {
            // Break if the level is complete
            if (levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp() >= levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime)
            {
                break;
            }

            // Check for forwards overflow
            if (checkpointTimeStamp + 5f > levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime)
            {
                forwardsButton.interactable = false;
            }
            else
            {
                forwardsButton.interactable = true;
            }

            // Check for backwards overflow
            if (checkpointTimeStamp - 5f < 0)
            {
                backwardsButton.interactable = false;
            }
            else
            {
                backwardsButton.interactable = true;
            }

            yield return null;           
        }
    }

    private void PracticeCheckpointLoad()
    {
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(checkpointTimeStamp);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(checkpointTimeStamp);
    }

    public void OnPracticeTimeSkippedForward()
    {
        checkpointTimeStamp += 5f;
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(checkpointTimeStamp);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(checkpointTimeStamp);
    }

    public void OnPracticeTimeSkippedBackwards()
    {
        checkpointTimeStamp -= 5f;
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(checkpointTimeStamp);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(checkpointTimeStamp);
    }

    public void DisablePracticeButtons()
    {
        forwardsButton.interactable = false;
        backwardsButton.interactable = false;

        // Prevents while loop triggering interactable after it is supposed to be false during game over
        if (TOORCCoroutine != null)
        {
            StopCoroutine(TOORCCoroutine);
        }
    }
}

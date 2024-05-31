using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class PracticeManagerScript : MonoBehaviour
{
    [Header("Practice Mode")]
    public static bool practiceMode = false;
    public static float checkpointTimeStamp = 0f; // Defaulted to start at 0f
                                                  // Linked to amount of time skipped in beatmap and music
    private float skipDuration;

    [Header("Managers")]
    [SerializeField] private GameObject UIManager;
    [SerializeField] private GameObject levelMusic;
    [SerializeField] private GameObject notesManager;
    [SerializeField] private GameObject logsUI;

    [Header("Pause UI")]
    [SerializeField] private Sprite practiceEnabled;
    [SerializeField] private Sprite practiceDisabled;
    [SerializeField] private GameObject practiceButton;

    [Header("Practice UI")]
    [SerializeField] private GameObject practiceUI;
    [SerializeField] private Button forwardsButton;
    [SerializeField] private Button backwardsButton;
    [SerializeField] private Button checkpointButton;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference forwardsActionReference;
    [SerializeField] private InputActionReference backwardsActionReference;
    [SerializeField] private InputActionReference checkpointActionReference;

    private Coroutine TOORCCoroutine;

    private void OnEnable()
    {
        // Prevents buttons working during normal playtime
        if (practiceMode)
        {
            forwardsActionReference.action.Enable();
            backwardsActionReference.action.Enable();
            checkpointActionReference.action.Enable();

            forwardsActionReference.action.performed += OnForwards;
            backwardsActionReference.action.performed += OnBackwards;
            checkpointActionReference.action.performed += OnCheckpoint;
        }
    }

    private void OnDisable()
    {
        forwardsActionReference.action.performed -= OnForwards;
        backwardsActionReference.action.performed -= OnBackwards;
        checkpointActionReference.action.performed -= OnCheckpoint;

        forwardsActionReference.action.Disable();
        backwardsActionReference.action.Disable();
        checkpointActionReference.action.Disable();
    }

    // Manually simulate the hovering and clicking of button is included
    private void OnForwards(InputAction.CallbackContext context)
    {
        CheckpointTimestampSkipForward();
        // Button animations because they are not called on keybind press,
        // Triggers cannot be set one after another because disabled needs to be instant to signal to the player
        if (checkpointTimeStamp + skipDuration > levelMusic.GetComponent<LevelMusicScript>().beatMapEndTime)
        {
            forwardsButton.GetComponent<Animator>().SetTrigger("Disabled");
        }
        else
        {
            forwardsButton.GetComponent<Animator>().SetTrigger("Full");
        }
    }

    private void OnBackwards(InputAction.CallbackContext context)
    {
        CheckpointTimestampSkipBackwards();
        if (checkpointTimeStamp - skipDuration < 0)
        {
            backwardsButton.GetComponent<Animator>().SetTrigger("Disabled");
        }
        else
        {
            backwardsButton.GetComponent<Animator>().SetTrigger("Full");
        }
    }

    private void OnCheckpoint(InputAction.CallbackContext context)
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
        SaveCheckpoint(currentTimeStamp);
        checkpointButton.GetComponent<Animator>().SetTrigger("Full");
    }

    void Start()
    {
        skipDuration = PlayerPrefs.GetFloat("Practice Skip Duration");
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

    private void PracticeCheckpointLoad()
    {
        levelMusic.GetComponent<LevelMusicScript>().SkipToTime(checkpointTimeStamp);
        notesManager.GetComponent<NotesManagerScript>().SkipToTime(checkpointTimeStamp);

        // Clear out the logs from previous attempt
        logsUI.GetComponent<LogsScript>().ClearLogs();
    }

    private IEnumerator TimeOutOfRangeCheck()
    {
        // Make sure that buttons are disabled the instant they would go out of range
        while (true)
        {
            // Break if the level is complete
            if (levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp() >= levelMusic.GetComponent<LevelMusicScript>().GetBeatMapEndTime())
            {
                break;
            }

            // Check for forwards overflow
            if (checkpointTimeStamp + skipDuration >= levelMusic.GetComponent<LevelMusicScript>().GetBeatMapEndTime())
            {
                forwardsActionReference.action.Disable();
                forwardsButton.interactable = false;
            }
            else
            {
                forwardsActionReference.action.Enable();
                forwardsButton.interactable = true;
            }

            // Check for backwards overflow
            if (checkpointTimeStamp - skipDuration < 0)
            {
                backwardsActionReference.action.Disable();
                backwardsButton.interactable = false;
            }
            else
            {
                backwardsActionReference.action.Enable();
                backwardsButton.interactable = true;
            }

            yield return null;           
        }
    }

    private void LoadPracticeUI(bool isPracticeMode)
    {
        // (Re)activate buttons because they are disabled during game over or level complete
        forwardsButton.interactable = isPracticeMode;
        backwardsButton.interactable = isPracticeMode;
        checkpointButton.interactable = isPracticeMode;
        practiceUI.SetActive(isPracticeMode);
    }

    // Automatically saves as new checkpoint
    public void CheckpointTimestampSkipForward()
    {
        SaveCheckpoint(checkpointTimeStamp + skipDuration);
        PracticeCheckpointLoad();
    }

    // Automatically saves as new checkpoint
    public void CheckpointTimestampSkipBackwards()
    {
        SaveCheckpoint(checkpointTimeStamp - skipDuration);
        PracticeCheckpointLoad();
    }

    public void SetCurrentTimeAsCheckpoint()
    {
        SaveCheckpoint(levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp());
    }

    private void SaveCheckpoint(float timestamp)
    {
        checkpointTimeStamp = timestamp;
    }

    public void DisablePracticeButtons()
    {
        // Prevents while loop triggering interactable after it is supposed to be false during game over
        if (TOORCCoroutine != null)
        {
            StopCoroutine(TOORCCoroutine);
        }

        // Disable inputactions and buttons
        forwardsActionReference.action.Disable();
        backwardsActionReference.action.Disable();
        checkpointActionReference.action.Disable();
        forwardsButton.interactable = false;
        backwardsButton.interactable = false;
        checkpointButton.interactable = false;
    }
}

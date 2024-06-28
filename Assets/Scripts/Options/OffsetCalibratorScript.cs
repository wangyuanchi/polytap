using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OffsetCalibratorScript : MonoBehaviour
{
    [Header("Calibration")]
    [SerializeField] private GameObject calibrationNote;
    [SerializeField] private TMP_Text accuracyText;
    private GameObject newCalibrationNote;

    [Header("Hidden UI")]
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject explanationButton;

    [Header("Audio Source")]
    [SerializeField] private AudioSource calibrationMetronome;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference calibrationActionReference;

    [Header("Sliders")]
    [SerializeField] private GameObject inputOffsetSlider;

    // The time in seconds between 4 notes of 100bpm
    private float interval = (60f / 100f) * 4f;

    // The "perfect" timing where an input would result in +0ms offset
    // Can be constantly updated and referred directly because only 1 note is present at a time
    private float optimalTiming;

    // A list of user inputs that is within the accepted range of offsets
    private List<float> inputTimings = new List<float>(); 

    private void OnEnable()
    {
        calibrationActionReference.action.Enable();
        calibrationActionReference.action.performed += OnInput;

        StartCoroutine(StartCalibration());
    }

    private void OnDisable()
    {
        calibrationActionReference.action.performed -= OnInput;
        calibrationActionReference.action.Disable();
    }

    private IEnumerator StartCalibration()
    {
        calibrationMetronome.Play();

        // Without this buffer, the metronome is expected to be synced with the note, but it is always late
        // Adding this buffer delays the spawning of the note (does not affect optimalTiming has it would also be shifted)
        // Now, +0.25 is the baseline for music offset, furthur customizable by the slider
        float musicOffset = PlayerPrefs.GetInt("Music Offset") / 1000f;
        yield return new WaitForSeconds(0.25f + musicOffset); 

        float timer = 0;
        int totalNotesSpawned = 0;

        while (totalNotesSpawned < 10)
        {
            timer += Time.deltaTime;
                
            if (timer >= interval)
            {
                // Spawn the note at x = 500, and it will move 1000 units to x = -500 (opposite)
                newCalibrationNote = Instantiate(calibrationNote, new Vector3(500, 0, 0), Quaternion.identity);

                // Sets the parent of the note to be this game object, but maintain its current position
                newCalibrationNote.transform.SetParent(gameObject.transform, false);

                // Only after setting positions then start moving the object
                newCalibrationNote.GetComponent<CalibrationNoteScript>().StartMove();

                optimalTiming = Time.time + (60f / 100f) * 2f;

                // Enable input because it has been disabled previously in OnInput
                calibrationActionReference.action.Enable();

                timer = 0;
                totalNotesSpawned++;
            }

            yield return null;
        }

        // For last bar of 4 beats to complete, then stop metronome
        yield return new WaitForSeconds(interval);
        calibrationMetronome.Stop();

        // inputOffset default to 0 if no inputs have been made, prevent out of bounds error
        int inputOffset = 0;
        if (inputTimings.Any())
        { inputOffset = -(int)inputTimings.Average(); }

        // For player to know that input offset has been updated
        if (inputOffset >= 0) { accuracyText.text = $"Your input offset has been set to +{inputOffset}ms."; }
        else { accuracyText.text = $"Your input offset has been set to {inputOffset}ms."; }

        // Update preferences and slider
        PlayerPrefs.SetInt("Input Offset", inputOffset);
        inputOffsetSlider.GetComponent<InputOffsetScript>().LoadInputOffset();

        yield return new WaitForSeconds(3);

        ShowCalibrationUI(false);
    }

    private void OnInput(InputAction.CallbackContext context)
    {
        float inputTiming = Time.time;
        float accuracy = Mathf.Round((optimalTiming - inputTiming) * 1000); // in milliseconds

        // Do not process inputs above the accepted offset range
        if (Mathf.Abs(accuracy) > 200f) return;

        // Prevent double inputs if an input has already been accepted
        calibrationActionReference.action.Disable();

        // Add all accepted inputs into a list
        inputTimings.Add(accuracy);

        // Display accuracy text
        if (accuracy > 0) { accuracyText.text = $"+{accuracy}ms"; }
        else { accuracyText.text = $"{accuracy}ms"; }
    }

    public void ShowCalibrationUI(bool show) 
    {
        // Reset accuracy text
        accuracyText.text = "+0ms";

        if (show)
        {
            LobbyMusicScript.instance.StopLobbyMusic();
        }
        else
        {
            LobbyMusicScript.instance.PlayLobbyMusic();
        }

        // Destroy the note if the user leaves the calibration prematurely to prevent the note being there on during a new calibration session
        if (newCalibrationNote != null) { Destroy(newCalibrationNote); }

        gameObject.SetActive(show);
        prevButton.SetActive(!show);
        nextButton.SetActive(!show);
        backButton.SetActive(!show);
        explanationButton.SetActive(!show);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{
    public Queue<GameObject> circleObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> squareObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> triangleObjectsQueue = new Queue<GameObject>();
    private Queue<GameObject> missedNoteObjectsQueue = new Queue<GameObject>();
    public Queue<Dictionary<string, float>> circleTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> squareTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> triangleTimingsQueue = new Queue<Dictionary<string, float>>();

    [Header("Managers")]
    [SerializeField] private GameObject UIManager;
    [SerializeField] private GameObject levelMusic;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference circleActionReference;
    [SerializeField] private InputActionReference squareActionReference;
    [SerializeField] private InputActionReference triangleActionReference;

    [Header("Note Square")]
    private bool initialSquareInput = false;

    [Header("Particles")]
    [SerializeField] private GameObject inputParticles;

    private void OnEnable()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();

        circleActionReference.action.performed += OnCircle;
        squareActionReference.action.started += OnSquareHold;
        squareActionReference.action.canceled += OnSquareRelease;
        triangleActionReference.action.performed += OnTriangle;
    }

    private void OnDisable()
    {
        circleActionReference.action.performed -= OnCircle;
        squareActionReference.action.started -= OnSquareHold;
        squareActionReference.action.canceled -= OnSquareRelease;
        triangleActionReference.action.performed -= OnTriangle;

        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    public void EnableShapeInputs()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();
    }

    public void DisableShapeInputs()
    {
        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    // Circle -> Single Tap
    private void OnCircle(InputAction.CallbackContext context)
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
        if (circleTimingsQueue.Count > 0) 
        { CheckInputCircle(currentTimeStamp); }
    }

    // Square -> Hold and Release
    private void OnSquareHold(InputAction.CallbackContext context)
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
        if (squareTimingsQueue.Count > 0)
        { CheckInputSquareInitial(currentTimeStamp); }
    }
    private void OnSquareRelease(InputAction.CallbackContext context)
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
        // initialSquareInput is only true if the first input is correct after checkInputSquareInitial is performed
        if (initialSquareInput && squareTimingsQueue.Count > 0) 
        { 
            CheckInputSquareFinal(currentTimeStamp);
            initialSquareInput = false;
        }
    }

    // Triangle -> Double Tap
    private void OnTriangle(InputAction.CallbackContext context)
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();
        if (triangleTimingsQueue.Count > 0) 
        { CheckInputTriangle(Time.time - currentTimeStamp); }
    }

    // For missed note, inputCorrect is false even though there was no input
    private void ProcessInput(bool inputCorrect, string inputDetails)
    {
        if (!inputCorrect)
        { UIManager.GetComponent<UIManagerScript>().TakeDamage(); }
        Debug.Log(inputDetails);
    }

    private void DequeueNote(Queue<GameObject> noteObjectsQueue, Queue<Dictionary<string, float>> noteTimingsQueue, bool destroyNote)
    {
        GameObject note = noteObjectsQueue.Dequeue();
        noteTimingsQueue.Dequeue();
        if (destroyNote)
        { Destroy(note); }
        else
        { missedNoteObjectsQueue.Enqueue(note); }
    }

    // Updates the text displaying the accuracy of each input
    private void UpdateAccuracyText(float accuracy, float expectedWindow, bool bypass)
    {
        if (PlayerPrefs.GetString("Accuracy") == "true")
        {
            UIManager.GetComponent<UIManagerScript>().UpdateAccuracyText(accuracy, expectedWindow, bypass);
        }
    }

    // expectedWindow = 100logx, base 10
    // Only outputs intended result using milliseconds! 
    private float expectedWindowCalculation(float accuracyWindow)
    {
        // Convert from seconds to milliseconds
        accuracyWindow = accuracyWindow * 1000;
        float expectedWindow = 100 * (float)Math.Log10(accuracyWindow);
        // Convert from milliseconds back to seconds
        return expectedWindow / 1000f;
    }

    private void CheckInputCircle(float inputTimeStamp)
    {
        // The accuracy window is the window of timing which an input would be considered correct
        // Example: C-100 note has a accuracy window of +-100ms, where the total window range is 200ms
        float accuracyWindow = circleTimingsQueue.Peek()["accuracyWindow"];
        // The expected window is where the user is expected to provide an input before the note is missed,
        // needs to scale accordingly to accuracy window (which is a subset of the expected window)
        float expectedWindow = expectedWindowCalculation(accuracyWindow);

        float requiredTimeStamp = circleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        
        if (timeFromPerfect <= accuracyWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false); 
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false);
            ProcessInput(false, "Wrong Input: Too Early/Late [Circle]");
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
        else
        {
            // Accuracy text will not reflect accuracy of missed notes
            // If the input is unexpected (way too early), do nothing
            return;
        }
    }

    private void CheckInputSquareInitial(float inputTimeStamp)
    {
        float accuracyWindow = squareTimingsQueue.Peek()["accuracyWindow"];
        float expectedWindow = expectedWindowCalculation(accuracyWindow);
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);

        // If the first input is correct, destroy noteSquareStart only and wait for second input
        if (timeFromPerfect <= accuracyWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false);
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareStart();
            initialSquareInput = true; // Give a bool flag for the second input such that
                                       // the second input is checked only if the first input is correct
        }
        // If the first input is wrong, process the input and destroy the whole note 
        else if (timeFromPerfect <= expectedWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false);
            ProcessInput(false, "Wrong Input: Too Early/Late [Square (Start)]");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
        else { return; }
    }

    // This ignores accuracy of the first input
    private void CheckInputSquareFinal(float inputTimeStamp)
    {
        float accuracyWindow = squareTimingsQueue.Peek()["accuracyWindow"];
        float expectedWindow = expectedWindowCalculation(accuracyWindow);
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);

        // Expected input regardless of how accurate it is, hence, bypass expected window check
        // However, if the note is missed, no accuracy text will be displayed
        UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, true); 

        if (timeFromPerfect <= accuracyWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Square (End)]");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
        else { return; }
    }
    private void CheckInputTriangle(float inputTimeStamp)
    {
        float accuracyWindow = triangleTimingsQueue.Peek()["accuracyWindow"];
        float expectedWindow = expectedWindowCalculation(accuracyWindow);
        float requiredTimeStamp = triangleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);

        if (timeFromPerfect <= accuracyWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false);
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, expectedWindow, false);
            ProcessInput(false, "Wrong Input: Too Early/Late [Triangle]");
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
        else { return; }
    }

    // Update is called once per frame
    void Update()
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();

        // Missed note checks is based on the fact that a non-missed note would have already been dequeued,
        // and since it is not, it must be missed
        if (circleTimingsQueue.Count > 0 && currentTimeStamp > circleTimingsQueue.Peek()["timeStamp"] + expectedWindowCalculation(circleTimingsQueue.Peek()["accuracyWindow"]))
        {
            DequeueNote(circleObjectsQueue, circleTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Circle]");
        }
        // Only a missed note if the first input has not yet be detected, because the note is not dequeued upon first input check
        if (squareTimingsQueue.Count > 0 && !initialSquareInput && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + expectedWindowCalculation(squareTimingsQueue.Peek()["accuracyWindow"]))
        {
            // Prevent 2 notes being shown and causing confusion
            // Only the first note will pass the judgement line and signify the loss of 1 health
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareEnd(); 
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (Start)]");
        }
        if (squareTimingsQueue.Count > 0 && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"] + expectedWindowCalculation(squareTimingsQueue.Peek()["accuracyWindow"]))
        {
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (End)]");
            // Invalidate onSquareRelease because note has already been missed 
            initialSquareInput = false;
        }
        if (triangleTimingsQueue.Count > 0 && currentTimeStamp > triangleTimingsQueue.Peek()["timeStamp"] + expectedWindowCalculation(triangleTimingsQueue.Peek()["accuracyWindow"]))
        {
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Triangle]");
        }
    }

    // Destroy all note game objects, clear note and timing queues
    public void ResetBeatMap()
    {
        DisableShapeInputs();

        // Destroy all note game objects and clear its queue
        while (circleObjectsQueue.Count > 0) 
        {
            GameObject note = circleObjectsQueue.Dequeue();
            Destroy(note);
        }

        while (squareObjectsQueue.Count > 0)
        {
            GameObject note = squareObjectsQueue.Dequeue();
            Destroy(note);
        }

        while (triangleObjectsQueue.Count > 0)
        {
            GameObject note = triangleObjectsQueue.Dequeue();
            Destroy(note);
        }

        while (missedNoteObjectsQueue.Count > 0)
        {
            GameObject missedNote = missedNoteObjectsQueue.Dequeue();
            Destroy(missedNote);
        }

        // Clear note timing queues
        circleTimingsQueue.Clear();
        squareTimingsQueue.Clear();
        triangleTimingsQueue.Clear();

        EnableShapeInputs(); 
    }
}

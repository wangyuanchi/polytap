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

    [Header("Timings")]
    [SerializeField] private float bufferWindow; // The buffer window is a subset of the expected window
    public float expectedWindow; // The expected window is where the user is expected to provide an input before the note is missed

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
    private void UpdateAccuracyText(float accuracy, bool bypass)
    {
        if (PlayerPrefs.GetString("Accuracy") == "true")
        {
            UIManager.GetComponent<UIManagerScript>().UpdateAccuracyText((float)Math.Round(accuracy * 1000, 0), bypass);
        }
    }

    private void CheckInputCircle(float inputTimeStamp)
    {
        float requiredTimeStamp = circleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, false);
        
        if (timeFromPerfect <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Circle]");
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
    }

    private void CheckInputSquareInitial(float inputTimeStamp)
    {
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, false);

        // If the first input is correct, destroy noteSquareStart only and wait for second input
        if (timeFromPerfect <= bufferWindow)
        {
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareStart();
            initialSquareInput = true; // Give a bool flag for the second input such that
                                       // the second input is checked only if the first input is correct
        }
        // If the first input is wrong, process the input and destroy the whole note 
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Square (Start)]");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
    }

    // This ignores accuracy of the first input
    private void CheckInputSquareFinal(float inputTimeStamp)
    {
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, true); // Expected input regardless of how accurate it is, hence, bypass expected window check

        if (timeFromPerfect <= bufferWindow)
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
    }
    private void CheckInputTriangle(float inputTimeStamp)
    {
        float requiredTimeStamp = triangleTimingsQueue.Peek()["timeStamp"];
        float timeFromPerfect = Math.Abs(requiredTimeStamp - inputTimeStamp);
        UpdateAccuracyText(requiredTimeStamp - inputTimeStamp, false);

        if (timeFromPerfect <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            inputParticles.GetComponent<InputParticlesScript>().SpawnParticles(timeFromPerfect);
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
        else if (timeFromPerfect <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late [Triangle]");
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp();

        // Missed note checks is based on the fact that a non-missed note would have already been dequeued,
        // and since it is not, it must be missed
        if (circleTimingsQueue.Count > 0 && currentTimeStamp > circleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            DequeueNote(circleObjectsQueue, circleTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Circle]");
        }
        // Only a missed note if the first input has not yet be detected, because the note is not dequeued upon first input check
        if (squareTimingsQueue.Count > 0 && !initialSquareInput && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            // Prevent 2 notes being shown and causing confusion
            // Only the first note will pass the judgement line and signify the loss of 1 health
            squareObjectsQueue.Peek().GetComponent<NoteSquareScript>().DestroyNoteSquareEnd(); 
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (Start)]");
        }
        if (squareTimingsQueue.Count > 0 && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"] + expectedWindow)
        {
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            ProcessInput(false, "Missed Note! [Square (End)]");
        }
        if (triangleTimingsQueue.Count > 0 && currentTimeStamp > triangleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
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

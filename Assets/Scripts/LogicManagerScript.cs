using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{
    public float beatMapStartTime;
    public Queue<GameObject> circleObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> squareObjectsQueue = new Queue<GameObject>();
    public Queue<GameObject> triangleObjectsQueue = new Queue<GameObject>();
    public Queue<Dictionary<string, float>> circleTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> squareTimingsQueue = new Queue<Dictionary<string, float>>();
    public Queue<Dictionary<string, float>> triangleTimingsQueue = new Queue<Dictionary<string, float>>();
    public GameObject UIManager;

    [SerializeField]
    private InputActionReference circleActionReference;
    [SerializeField]
    private InputActionReference squareActionReference;
    [SerializeField]
    private InputActionReference triangleActionReference;
    private float inputDuration = 0;

    private float bufferWindow = 0.15f; // The buffer window is a subset of the expected window
    private float expectedWindow = 0.5f; // The expected window is where the user is expected to provide an input before the note is missed
    
    private void OnEnable()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();
    }

    private void OnDisable()
    {
        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Circle -> Single Tap
        circleActionReference.action.performed += context =>
        {
            if (circleTimingsQueue.Count > 0) { checkInputCircle(Time.time - beatMapStartTime); }
        };

        // Square -> Hold and Release
        squareActionReference.action.started += context =>
        {
            inputDuration = Time.time;
        };
        squareActionReference.action.canceled += context =>
        {
            float inputEnd = Time.time;
            inputDuration = inputEnd - inputDuration;
            if (squareTimingsQueue.Count > 0) { checkInputSquare(inputEnd - inputDuration - beatMapStartTime, inputDuration); }
        };

        triangleActionReference.action.performed += context =>
        {
            if (triangleTimingsQueue.Count > 0) { checkInputTriangle(Time.time - beatMapStartTime); }
        };
    }

    void checkInputCircle(float inputTimeStamp)
    {
        float requiredTimeStamp = circleTimingsQueue.Peek()["timeStamp"];
        if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
        else if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late");
            DequeueNote(circleObjectsQueue, circleTimingsQueue, true);
        }
    }

    void checkInputSquare(float inputTimeStamp, float inputDuration)
    {
        float requiredTimeStamp = squareTimingsQueue.Peek()["timeStamp"];
        if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= bufferWindow)
        {
            if (Math.Abs(squareTimingsQueue.Peek()["duration"] - inputDuration) <= bufferWindow)
            {
                ProcessInput(true, "Correct Input!");
            }
            else
            {
                ProcessInput(false, "Wrong Input: Too Early/Late Release");
            }
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
        else if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late");
            DequeueNote(squareObjectsQueue, squareTimingsQueue, true);
        }
    }

    void checkInputTriangle(float inputTimeStamp)
    {
        float requiredTimeStamp = triangleTimingsQueue.Peek()["timeStamp"];
        if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= bufferWindow)
        {
            ProcessInput(true, "Correct Input!");
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
        else if (Math.Abs(requiredTimeStamp - inputTimeStamp) <= expectedWindow)
        {
            ProcessInput(false, "Wrong Input: Too Early/Late");
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentTimeStamp = Time.time - beatMapStartTime;
        if (circleTimingsQueue.Count > 0 && currentTimeStamp > circleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            DequeueNote(circleObjectsQueue, circleTimingsQueue, false);
            UIManager.GetComponent<UIManagerScript>().DecreaseHealth();
            Debug.Log("Missed Note!");
        }
        if (squareTimingsQueue.Count > 0 && currentTimeStamp > squareTimingsQueue.Peek()["timeStamp"] + squareTimingsQueue.Peek()["duration"] + expectedWindow)
        {
            DequeueNote(squareObjectsQueue, squareTimingsQueue, false);
            UIManager.GetComponent<UIManagerScript>().DecreaseHealth();
            Debug.Log("Missed Note!");
        }
        if (triangleTimingsQueue.Count > 0 && currentTimeStamp > triangleTimingsQueue.Peek()["timeStamp"] + expectedWindow)
        {
            DequeueNote(triangleObjectsQueue, triangleTimingsQueue, false);
            UIManager.GetComponent<UIManagerScript>().DecreaseHealth();
            Debug.Log("Missed Note!");
        }
    }

    void ProcessInput(bool inputCorrect, string inputDetails)
    {
        Debug.Log(inputDetails);

        if (!inputCorrect)
        { UIManager.GetComponent<UIManagerScript>().DecreaseHealth(); }
    }

    void DequeueNote(Queue<GameObject> noteObjectsQueue, Queue<Dictionary<string, float>> noteTimingsQueue, bool destroyNote)
    {
        GameObject note = noteObjectsQueue.Dequeue();
        noteTimingsQueue.Dequeue();
        if (destroyNote)
        { Destroy(note); }
    }
}

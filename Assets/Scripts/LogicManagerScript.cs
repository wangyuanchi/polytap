using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{

    public float beatMapStartTime;
    public Queue<GameObject> noteObjectsQueue = new Queue<GameObject>();
    public Queue<Dictionary<string, float>> noteTimingsQueue = new Queue<Dictionary<string, float>>();  

    private float inputStart;
    private float inputEnd;
    private bool input = false;
    private bool waitForDuration = false;
    private float inputDuration = 0;
    private float bufferWindow = 0.15f; // The buffer window is a subset of the expected window
    private float expectedWindow = 0.5f; // The expected window is where the user is expected to provide an input before the note is missed

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float currentTimeStamp = Time.time - beatMapStartTime;

        // If there are notes in the queue
        if (noteObjectsQueue.Count > 0 && noteTimingsQueue.Count > 0) { 

            if (input)
            {
                // If (initial) input is expected and correct
                if (Math.Abs(noteTimingsQueue.Peek()["timeStamp"] - currentTimeStamp) <= bufferWindow || waitForDuration)
                {
                    // If tap required
                    if (noteTimingsQueue.Peek()["type"] == 0f)
                    {
                        Debug.Log("Correct Input");
                        noteTimingsQueue.Dequeue();
                        Destroy(noteObjectsQueue.Dequeue());

                    }
                    // If hold required
                    else if (noteTimingsQueue.Peek()["type"] == 1f)
                    {
                        waitForDuration = true; // This is a bypass variable so that this if-else statement would be reached on the next frame
                        if (inputDuration != 0 && Math.Abs(inputDuration - noteTimingsQueue.Peek()["duration"]) <= bufferWindow)
                        {
                            Debug.Log("Correct Input");
                            noteTimingsQueue.Dequeue();
                            Destroy(noteObjectsQueue.Dequeue());
                            waitForDuration = false;
                        }
                        else if (inputDuration != 0 && noteTimingsQueue.Peek()["duration"] - inputDuration > bufferWindow)
                        {
                            Debug.Log("Wrong Input: Release Too Early");
                            noteTimingsQueue.Dequeue();
                            Destroy(noteObjectsQueue.Dequeue());
                            waitForDuration = false;
                        }
                        else if (inputDuration == 0 && currentTimeStamp > noteTimingsQueue.Peek()["timeStamp"] + noteTimingsQueue.Peek()["duration"] + bufferWindow)
                        {
                            Debug.Log("Wrong Input: Release Too Late");
                            noteTimingsQueue.Dequeue();
                            Destroy(noteObjectsQueue.Dequeue());
                            waitForDuration = false;
                        }

                    }
                }

                // If (initial) input is expected and too early
                if (noteTimingsQueue.Peek()["timeStamp"] - currentTimeStamp <= expectedWindow && noteTimingsQueue.Peek()["timeStamp"] - currentTimeStamp > bufferWindow )
                {
                    Debug.Log("Wrong Input: Tap Too Early");
                    noteTimingsQueue.Dequeue();
                    Destroy(noteObjectsQueue.Dequeue());
                }
            }

            // Note: Inputs that are unexpected will not be considered wrong
            // If note is completely missed without input (If input is too late, will simply be considered an unexpected input)
            if (currentTimeStamp - noteTimingsQueue.Peek()["timeStamp"] > bufferWindow && !waitForDuration)
            {
                Debug.Log("Missed Note!");
                noteTimingsQueue.Dequeue();
                Destroy(noteObjectsQueue.Dequeue());
            }

            if (!waitForDuration)
            {
                input = false;
                inputDuration = 0;
            }
        }
    }

    public void Tap(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            inputStart = Time.time;
            input = true;
        }
        if (context.canceled)
        {
            inputEnd = Time.time;
            inputDuration = inputEnd - inputStart;
        }

    }


}

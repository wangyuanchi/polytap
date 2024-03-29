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
    public GameObject UIManager;

    private float inputStart;
    private float inputEnd;
    private bool input = false;
    private float inputDuration = 0;
    private bool waitingForInputDuration = false; // This is a bypass variable so that if-else statements would be reached on the next frame

    private float bufferWindow = 0.15f; // The buffer window is a subset of the expected window
    private float expectedWindow = 0.5f; // The expected window is where the user is expected to provide an input before the note is missed

    // Update is called once per frame
    void Update()
    {
        // If there are notes in the queue
        if (noteObjectsQueue.Count > 0)
        {
            float currentTimeStamp = Time.time - beatMapStartTime;
            float currentTimeToNextNoteTiming = noteTimingsQueue.Peek()["timeStamp"] - currentTimeStamp;

            // If input is detected
            if (input)
            {
                // If input is expected and too early
                if (Math.Abs(currentTimeToNextNoteTiming) <= expectedWindow && currentTimeToNextNoteTiming > bufferWindow)
                {
                    DequeueNote("wrong", "Wrong Input: Tap Too Early");
                }

                // If input is expected and correct
                else if (Math.Abs(currentTimeToNextNoteTiming) <= bufferWindow || waitingForInputDuration)
                {
                    // If tap required
                    if (noteTimingsQueue.Peek()["type"] == 0f)
                    {
                        DequeueNote("correct", "Correct Input");
                    }

                    // If hold required
                    else if (noteTimingsQueue.Peek()["type"] == 1f)
                    {
                        waitingForInputDuration = true;
                        // If the hold has been released
                        if (inputDuration != 0)
                        {
                            if (Math.Abs(noteTimingsQueue.Peek()["duration"] - inputDuration) <= bufferWindow)
                            {
                                DequeueNote("correct", "Correct Input");
                            }
                            else
                            {
                                DequeueNote("wrong", "Wrong Input: Release Too Early");
                            }
                            waitingForInputDuration = false;
                        }
                        // If the hold has not been released and the release timing is missed
                        else
                        {
                            if (currentTimeStamp > noteTimingsQueue.Peek()["timeStamp"] + noteTimingsQueue.Peek()["duration"] + bufferWindow)
                            {
                                DequeueNote("wrong", "Wrong Input: Release Too Late");
                                waitingForInputDuration = false;
                            }
                        }
                    }
                }
                // If input is expected and wrong
                else if (-currentTimeToNextNoteTiming > bufferWindow)
                {
                    DequeueNote("wrong", "Wrong Input: Tap Too Late");
                }
            }

            // If note is completely missed without input
            else if (-currentTimeToNextNoteTiming > expectedWindow)
            {
                DequeueNote("missed", "Missed Note");
            }

            if (!waitingForInputDuration)
            {
                ResetInput(); 
            }
        }
    }

    // Called when a correct/wrong input took place or the note is missed
    void DequeueNote(string inputType, string inputDetails)
    {
        Debug.Log(inputDetails);

        GameObject dequeuedNoteObject = noteObjectsQueue.Dequeue();
        noteTimingsQueue.Dequeue();

        // Decrease health if input is wrong/missed
        if (inputType != "correct")
        {
            UIManager.GetComponent<UIManagerScript>().DecreaseHealth();
        }

        // Let noteObject move past the JudgementLine and be destroyed after a while if it is missed, otherwise destroy it instantly
        if (inputType != "missed")
        {
            Destroy(dequeuedNoteObject);    
        }
}

    // Sets input to true, and checks for its duration
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

    void ResetInput()
    {
        input = false;
        inputDuration = 0;
    }
}

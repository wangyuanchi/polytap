using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LogicManagerScript : MonoBehaviour
{

    public List<Dictionary<string, float>> beatMap;
    public float beatMapStartTime;

    private float registeredTapTimestamp;
    private bool newRegisteredTap = false;

    private int currentNote = 0; 
    private float bufferWindow = 0.2f;

    public List<GameObject> notesList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If current note exists
        if (currentNote < beatMap.Count)
        {
            // If the current note has been missed, go to the next one
            if (Time.time - beatMapStartTime > beatMap[currentNote]["timeStamp"] + bufferWindow)
            {
                Debug.Log("Missed Timing");
                currentNote++;
                DeleteNote();

            }

            // If a tap has been registered, check if it is at the correct timing
            if (newRegisteredTap)
            {
                if (Math.Abs(beatMap[currentNote]["timeStamp"] - registeredTapTimestamp) <= bufferWindow)
                {
                    Debug.Log("Correct Timing");
                    DeleteNote();

                }
                else
                {
                    Debug.Log("Wrong Timing");
                    DeleteNote();
                }
                currentNote++;
                newRegisteredTap = false;
            }
        }
        else {
            Debug.Log("Level Complete");
        }
    }

    public void Tap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            registeredTapTimestamp = Time.time - beatMapStartTime;
            newRegisteredTap = true;
        }
    }

    public void DeleteNote()
    {
        GameObject oldestNote = notesList[0];
        Destroy(oldestNote);
        notesList.RemoveAt(0);
    }

}

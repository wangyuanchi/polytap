using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogicManagerScript : MonoBehaviour
{

    public List<float> allTapTimings = new List<float>(); // May not be in ascending order
    public float beatMapStartTime;

    private float registeredTapTiming;
    private bool newRegisteredTap = false;
    private int tapTimingListener = 0; // Index of the current timing being listened for a tap
    private float bufferWindow = 0.5f;
    private bool levelComplete = false;
    

    // Start is called before the first frame update
    void Start()
    {
        allTapTimings.Sort();
    }

    // Update is called once per frame
    void Update()
    {
        // If current tapTimingListener exists in allTapTimings
        if (tapTimingListener < allTapTimings.Count)
        {
            // If the current tap timing being listened has been missed, go to the next one
            if (Time.time > allTapTimings[tapTimingListener] + bufferWindow)
            {
                tapTimingListener++;
                Debug.Log("Missed Timing");
            }

            // If a tap has been registered, check if it is at the correct timing
            if (newRegisteredTap)
            {
                if (Math.Abs(allTapTimings[tapTimingListener] - registeredTapTiming) <= bufferWindow)
                {
                    Debug.Log("Correct Timing");
                    tapTimingListener++;
                }
                else
                {
                    Debug.Log("Wrong Timing");
                }
                newRegisteredTap = false;
            }
        }
        else {
            levelComplete = true;
        }
    }

    public void Tap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            registeredTapTiming = Time.time - beatMapStartTime;
            newRegisteredTap = true;
        }
    }

}

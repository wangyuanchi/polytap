using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCircleScript : MonoBehaviour
{

    public float timeToTap; // The time it takes from spawning to hitting the judgement line

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeToTap));
    }

    // Scale the note to go to (1.5, 1.5, 1.5), where it passes JudgementLineCircle at (1, 1, 1) by time
    IEnumerator ScaleOverTime(float timeToTap)
    {
        float elapsedTime = 0f;
        float timeToExit = timeToTap * 1.5f;

        while (elapsedTime < timeToExit)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), elapsedTime / timeToExit);
            yield return null;
        }

        // Delete missed note
        Destroy(gameObject);
    }

}

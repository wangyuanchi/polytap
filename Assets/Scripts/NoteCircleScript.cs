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

    // Scale the note to go to (3, 3, 3), where it passes JudgementLineCircle at (1, 1, 1) by time
    IEnumerator ScaleOverTime(float timeToTap)
    {
        float elapsedTime = 0f;
        float timeToExit = timeToTap * 3;

        while (elapsedTime < timeToExit)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(3, 3, 3), elapsedTime / timeToExit);
            yield return null;
        }

        // Deletes the object after it goes offscreen
        Destroy(gameObject);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCircleScript : MonoBehaviour
{

    public float timeSpawnToJudgement;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement));
    }

    // Scale the note to go to (1.5, 1.5, 1.5), where it passes JudgementLineCircle at (1, 1, 1) by timeSpawnToJudgement
    IEnumerator ScaleOverTime(float timeSpawnToJudgement)
    {
        float elapsedTime = 0f;
        float timeSpawnToDestroy = timeSpawnToJudgement * 1.5f;

        while (elapsedTime < timeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), elapsedTime / timeSpawnToDestroy);
            yield return null;
        }
    }
}

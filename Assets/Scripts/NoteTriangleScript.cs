using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTriangleScript : MonoBehaviour
{

    public float timeSpawnToJudgement;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement));
    }

    // Scale the note to go to (3, 3, 3), where it passes JudgementLineCircle at (1, 1, 1) by timeSpawnToJudgement
    IEnumerator ScaleOverTime(float timeSpawnToJudgement)
    {
        float elapsedTime = 0f;
        float timeSpawnToDestroy = timeSpawnToJudgement * 3f;

        while (elapsedTime < timeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(3f, 3f, 3f), elapsedTime / timeSpawnToDestroy);
            yield return null;
        }

        Destroy(gameObject);
    }
}

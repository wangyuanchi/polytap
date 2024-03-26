using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSquareScript : MonoBehaviour
{

    public float timeSpawnToJudgement; 
    public float holdDuration;

    public GameObject noteSquareX;
    public GameObject noteSquareY;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, noteSquareX));
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, noteSquareY));
    }

    // Scale the side to go to (1.5, 1.5, 1.5), where it passes JudgementLineSquare at (1, 1, 1) by timeSpawnToJudgement
    IEnumerator ScaleOverTime(float timeSpawnToJudgement, GameObject side)
    {
        float elapsedTime = 0f;
        float timeSpawnToDestroy = timeSpawnToJudgement * 1.5f;

        if (side == noteSquareY)
        { 
            yield return new WaitForSeconds(holdDuration);
        }

        while (elapsedTime < timeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            side.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), elapsedTime / timeSpawnToDestroy);
            yield return null;
        }
    }
}

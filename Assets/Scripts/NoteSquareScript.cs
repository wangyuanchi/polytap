using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSquareScript : MonoBehaviour
{

    public float timeSpawnToJudgement; 
    public float holdDuration;

    public GameObject noteSquareStart;
    public GameObject noteSquareEnd;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, noteSquareStart));
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, noteSquareEnd));
    }

    // Scale the note to go to (1.5, 1.5, 1.5), where it passes JudgementLineSquare at (1, 1, 1) by timeSpawnToJudgement
    IEnumerator ScaleOverTime(float timeSpawnToJudgement, GameObject noteSquareChild)
    {
        float elapsedTime = 0f;
        float timeSpawnToDestroy = timeSpawnToJudgement * 1.5f;

        if (noteSquareChild == noteSquareEnd)
        { 
            yield return new WaitForSeconds(holdDuration);
        }

        while (elapsedTime < timeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), elapsedTime / timeSpawnToDestroy);
            yield return null;
        }

        Destroy(noteSquareChild);
    }
}

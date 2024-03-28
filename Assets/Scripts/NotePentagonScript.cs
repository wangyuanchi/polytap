using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePentagonScript : MonoBehaviour
{

    public float timeSpawnToJudgement;
    public float holdDuration;

    public GameObject notePentagonStart;
    public GameObject notePentagonEnd;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, notePentagonStart));
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, notePentagonEnd));
    }

    // Scale the note to go to (1.5, 1.5, 1.5), where it passes JudgementLinePentagon at (1, 1, 1) by timeSpawnToJudgement
    IEnumerator ScaleOverTime(float timeSpawnToJudgement, GameObject notePentagonChild)
    {
        float elapsedTime = 0f;
        float timeSpawnToDestroy = timeSpawnToJudgement * 1.5f;

        if (notePentagonChild == notePentagonEnd)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        while (elapsedTime < timeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            notePentagonChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), elapsedTime / timeSpawnToDestroy);
            yield return null;
        }

        Destroy(notePentagonChild);
    }
}

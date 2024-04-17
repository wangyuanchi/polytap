using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSquareScript : MonoBehaviour
{

    public float timeSpawnToJudgement; 
    public float holdDuration;
    public float noteSpeedTiming;

    [Header("Child Square Objects")]
    [SerializeField] private GameObject noteSquareStart;
    [SerializeField] private GameObject noteSquareEnd;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, 3f, noteSquareStart));
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, 3f, noteSquareEnd));
    }

    // Scale the note to go to (3, 3, 3), where it passes JudgementLineSquare at (1, 1, 1) by timeSpawnToJudgement
    private IEnumerator ScaleOverTime(float timeSpawnToJudgement, float finalScale, GameObject noteSquareChild)
    {
        // Variables here are catered to noteSquareStart
        float defaultTimeSpawnToDestroy = noteSpeedTiming * finalScale;
        float elapsedTime = noteSpeedTiming - timeSpawnToJudgement;

        if (noteSquareChild == noteSquareEnd)
        {
            // If pre-spawning of noteSquareEnd is not required, wait for required time and set elapsedTime to 0
            if (holdDuration >= elapsedTime)
            {
                yield return new WaitForSeconds(holdDuration - elapsedTime);
                elapsedTime = 0;
            }
            // If pre-spawning of noteSquareEnd is required, set elapsedTime and its position
            else
            {
                elapsedTime = elapsedTime - holdDuration;
                noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), elapsedTime / defaultTimeSpawnToDestroy);
            }
        }

        // Move preSpawned note to starting position if required
        if (noteSquareChild == noteSquareStart && elapsedTime > 0)
        {
            noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), elapsedTime / defaultTimeSpawnToDestroy);
        }

        while (elapsedTime < defaultTimeSpawnToDestroy)
        {
            elapsedTime += Time.deltaTime;
            noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), elapsedTime / defaultTimeSpawnToDestroy);
            yield return null;
        }

        Destroy(noteSquareChild);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSquareScript : MonoBehaviour
{
    public float timeSpawnToJudgement; // This is the time it takes for the note to move from its current position,
                                       // not necessarily at (0, 0, 0) due to prespawns, to the judgement line at (1, 1, 1)
    public float defaultTimeSpawnToJudgement; // This is the time it takes for a note to move from (0, 0, 0) to the judgement line at (1, 1, 1)
    public float holdDuration;

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
        // "Default" means the expected time if or assuming the note is not prespawned
        float defaultTimeSpawnToDestroy = defaultTimeSpawnToJudgement * finalScale;
        float defaultElapsedTime = defaultTimeSpawnToJudgement - timeSpawnToJudgement;

        // Move preSpawned note to starting position if required
        if (noteSquareChild == noteSquareStart && defaultElapsedTime > 0)
        {
            noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), defaultElapsedTime / defaultTimeSpawnToDestroy);
        }

        if (noteSquareChild == noteSquareEnd)
        {
            // If pre-spawning of noteSquareEnd is not required, wait for required time and set defaultElapsedTime 
            if (holdDuration >= defaultElapsedTime)
            {
                yield return new WaitForSeconds(holdDuration - defaultElapsedTime);
                defaultElapsedTime = 0; // Recater the variable to noteSqureEnd, because it was set for noteSquareStart initially
            }
            // If pre-spawning of noteSquareEnd is required, set defaultElapsedTime  and its position
            else
            {
                defaultElapsedTime = defaultElapsedTime - holdDuration; // Recater the variable to noteSqureEnd, because it was set for noteSquareStart initially
                noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), defaultElapsedTime / defaultTimeSpawnToDestroy);
            }
        }

        // Scaling up with time from starting position
        // Check for null because notes can be deleted based on user input or if user misses the note
        while (defaultElapsedTime < defaultTimeSpawnToDestroy && noteSquareChild != null)
        {
            defaultElapsedTime += Time.deltaTime;
            noteSquareChild.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), defaultElapsedTime / defaultTimeSpawnToDestroy);
            yield return null;
        }

        // Destroy game object after it moves off the screen
        Destroy(noteSquareChild);
    }

    public void DestroyNoteSquareStart()
    {
        Destroy(noteSquareStart);
    }
    public void DestroyNoteSquareEnd()
    {
        Destroy(noteSquareEnd);
    }
}

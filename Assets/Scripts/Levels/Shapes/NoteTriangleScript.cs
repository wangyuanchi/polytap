using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTriangleScript : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite noteTriangle10;
    [SerializeField] private Sprite noteTriangle25;
    [SerializeField] private Sprite noteTriangle50;
    [SerializeField] private Sprite noteTriangle75;
    [SerializeField] private Sprite noteTriangle100;

    [Header("Timings")]
    public float timeSpawnToJudgement; // This is the time it takes for the note to move from its current position,
                                       // not necessarily at (0, 0, 0) due to prespawns, to the judgement line at (1, 1, 1)
    public float defaultTimeSpawnToJudgement; // This is the time it takes for a note to move from (0, 0, 0) to the judgement line at (1, 1, 1)

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleOverTime(timeSpawnToJudgement, 6f));
    }

    // Scale the note to go to (6, 6, 6), where it passes JudgementLineTriangle at (1, 1, 1) by timeSpawnToJudgement
    private IEnumerator ScaleOverTime(float timeSpawnToJudgement, float finalScale)
    {
        // "Default" means the expected time if or assuming the note is not prespawned
        float defaultTimeSpawnToDestroy = defaultTimeSpawnToJudgement * finalScale;
        float defaultElapsedTime = defaultTimeSpawnToJudgement - timeSpawnToJudgement;

        // Move preSpawned note to starting position if required
        if (defaultElapsedTime > 0)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), defaultElapsedTime / defaultTimeSpawnToDestroy);
        }

        // Scaling up with time from starting position
        while (defaultElapsedTime < defaultTimeSpawnToDestroy)
        {
            defaultElapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(finalScale, finalScale, finalScale), defaultElapsedTime / defaultTimeSpawnToDestroy);
            yield return null;
        }

        // Destroy game object after it moves off the screen
        Destroy(gameObject);
    }

    public void SetSprite(float accuracyWindow)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (accuracyWindow == 0.01f) { spriteRenderer.sprite = noteTriangle10; }
        else if (accuracyWindow == 0.025f) { spriteRenderer.sprite = noteTriangle25; }
        else if (accuracyWindow == 0.05f) { spriteRenderer.sprite = noteTriangle50; }
        else if (accuracyWindow == 0.075f) { spriteRenderer.sprite = noteTriangle75; }
        else if (accuracyWindow == 0.1f) { spriteRenderer.sprite = noteTriangle100; }
        else { Debug.Log("Invalid Accuracy Window! Sprite set as default [noteTriangleDefault.png]."); }
    }
}

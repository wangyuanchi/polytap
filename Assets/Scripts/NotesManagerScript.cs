using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManagerScript : MonoBehaviour
{
    public GameObject noteCircle;
    public GameObject noteSquare;
    public GameObject noteTriangle;
    public GameObject logicManager;
    public int noteSpeed = 5; // note sprite thickness calibrated for noteSpeed = 3

    // WARNING: timeStamp (near the start) cannot be at a timing earlier than that of timeSpawnToJudgement!
    // typeOfNote: 0f -> Circle, 1f -> Square, 2f -> Triangle
    private List<Dictionary<string, float>> beatMap = new List<Dictionary<string, float>>
    {
        new Dictionary<string, float> { { "typeOfNote", 2f }, { "timeStamp", 3f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 4f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 5f }, { "timeStampRelease", 5.25f } }
    };
    // Referencing the index of beatMap
    private int currentNote = 0;

    // noteSpeed timings { noteSpeed, timeSpawnToJudgement }
    private Dictionary<int, float> noteSpeedTimings = new Dictionary<int, float>
    {
        { 1, 10f },
        { 2, 8f },
        { 3, 6f },
        { 4, 4f },
        { 5, 2f }
    };

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBeatMap(beatMap));

        // Give LogicManagerScript the exact timing the beatmap starts
        logicManager.GetComponent<LogicManagerScript>().beatMapStartTime = Time.time;
    }

    IEnumerator SpawnBeatMap(List<Dictionary<string, float>> beatMap)
    {
        // If the currentNote exists
        while (currentNote < beatMap.Count) {
            float timeUntilSpawn = beatMap[currentNote]["timeStamp"] - noteSpeedTimings[noteSpeed];
            if (currentNote > 0)
            {
                timeUntilSpawn = beatMap[currentNote]["timeStamp"] - beatMap[currentNote - 1]["timeStamp"];
            }
            // Wait for timing to hit spawn time, then spawn the note and go to the next note
            yield return new WaitForSeconds(timeUntilSpawn);
            SpawnNote(beatMap[currentNote]);
            currentNote++;
        }
    }

    // Spawn, update relevant variables in the instance of the new note and give it AND its timing(s) to logicManager 
    // type: 0f -> tap, 1f -> hold
    void SpawnNote(Dictionary<string, float> note)
    {
        // noteCircle
        if (note["typeOfNote"] == 0f) 
        {
            GameObject newNote = Instantiate(noteCircle, transform.position, transform.rotation);
            newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            logicManager.GetComponent<LogicManagerScript>().noteObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().noteTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "type", 0f },
                        { "timeStamp", note["timeStamp"] }
                    }
                );
        }

        // noteSquare
        if (note["typeOfNote"] == 1f) 
        {
            GameObject newNote = Instantiate(noteSquare, transform.position, transform.rotation);
            newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            newNote.GetComponent<NoteSquareScript>().holdDuration = note["timeStampRelease"] - note["timeStamp"];
            logicManager.GetComponent<LogicManagerScript>().noteObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().noteTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "type", 1f },
                        { "timeStamp", note["timeStamp"] },
                        { "duration", note["timeStampRelease"] - note["timeStamp"] }
                    }
                );
        }

        // noteTriangle
        if (note["typeOfNote"] == 2f)
        {
            for (int triangleIndex = 0; triangleIndex < 2; triangleIndex++)
            {    
            GameObject newNote = Instantiate(noteTriangle, transform.position, transform.rotation);
            newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            logicManager.GetComponent<LogicManagerScript>().noteObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().noteTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "type", 0f },
                        { "timeStamp", note["timeStamp"] },
                    }
                );
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManagerScript : MonoBehaviour
{
    public GameObject noteCircle;
    public GameObject noteSquare;
    public GameObject noteTriangle;
    public GameObject notePentagon;

    public GameObject logicManager;
    public GameObject audioManager;
    
    public int noteSpeed = 5; // note sprite thickness calibrated for noteSpeed = 3

    // WARNING: timeStamp (near the start) cannot be at a timing earlier than that of timeSpawnToJudgement!
    // typeOfNote: 0f -> Circle, 1f -> Square, 2f -> Triangle, 3f -> Pentagon
    private List<Dictionary<string, float>> beatMap = new List<Dictionary<string, float>>
    {
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 5.164f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 10.230f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 14.843f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 19.794f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 21.974f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 24.334f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 26.744f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 29.150f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 31.593f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 34.023f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 36.374f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 38.780f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 43.643f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 48.424f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 53.164f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 57.949f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 62.804f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 65.203f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 66.423f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 67.553f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 68.663f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 69.883f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 71.013f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 72.243f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 77.228f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 82.013f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 84.438f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 86.774f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 89.279f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 91.708f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 94.053f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 96.409f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 98.898f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 101.268f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 103.658f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 105.939f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 108.418f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 110.868f } }
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
        audioManager.GetComponent<AudioManagerScript>().PlayAudio();

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
        else if (note["typeOfNote"] == 1f)
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
        else if (note["typeOfNote"] == 2f)
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

        // notePentagon
        else if (note["typeOfNote"] == 3f)
        {
            GameObject newNote = Instantiate(notePentagon, transform.position, transform.rotation);
            newNote.GetComponent<NotePentagonScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            newNote.GetComponent<NotePentagonScript>().holdDuration = note["timeStampRelease"] - note["timeStamp"];
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
    }
}

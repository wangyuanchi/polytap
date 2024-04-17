using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NotesManagerScript : MonoBehaviour
{
    [Header("Note Objects")]
    public int noteSpeed;
    [SerializeField] private GameObject noteCircle;
    [SerializeField] private GameObject noteSquare;
    [SerializeField] private GameObject noteTriangle;

    [Header("Managers")]
    [SerializeField] private GameObject logicManager;
    [SerializeField] private GameObject UIManager;
    
    // WARNING: timeStamp (near the start) cannot be at a timing earlier than that of timeSpawnToJudgement!
    // typeOfNote: 0f -> Circle, 1f -> Square, 2f -> Triangle
    private List<Dictionary<string, float>> beatMap = new List<Dictionary<string, float>>
    {
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 5.277f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 10.151f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 14.931f } },
        new Dictionary<string, float> { { "typeOfNote", 2f }, { "timeStamp", 19.661f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 24.326f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 29.311f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 34.111f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 38.896f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 43.621f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 48.421f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 53.201f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 58.037f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 62.862f }, { "timeStampRelease", 71.182f } },
        new Dictionary<string, float> { { "typeOfNote", 2f }, { "timeStamp", 72.376f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 77.242f }, { "timeStampRelease", 78.372f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 79.602f }, { "timeStampRelease", 80.802f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 82.007f }, { "timeStampRelease", 83.208f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 84.432f }, { "timeStampRelease", 85.563f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 86.792f }, { "timeStampRelease", 87.972f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 89.202f }, { "timeStampRelease", 90.382f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 91.642f }, { "timeStampRelease", 94.032f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 96.407f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 98.782f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 101.411f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 103.841f } },
        new Dictionary<string, float> { { "typeOfNote", 1f }, { "timeStamp", 106.068f }, { "timeStampRelease", 114.232f } }
    };

    // Referencing the index of beatMap
    private int currentNote = 0;

    // noteSpeed timings { noteSpeed, timeSpawnToJudgement }
    private Dictionary<int, float> noteSpeedTimings = new Dictionary<int, float>
    {
        { 1, 10f },
        { 2, 9f },
        { 3, 8f },
        { 4, 7f },
        { 5, 6f },
        { 6, 5f },
        { 7, 4f },
        { 8, 3f },
        { 9, 2f },
        { 10, 1f }
    };

    // Start is called before the first frame update
    void Start()
    {
        noteSpeed = PlayerPrefs.GetInt("Note Speed");
        StartCoroutine(SpawnBeatMap(beatMap));

        // Give LogicManagerScript and UIManagerScript the exact timing the beatmap starts
        logicManager.GetComponent<LogicManagerScript>().beatMapStartTime = Time.time;
        UIManager.GetComponent<UIManagerScript>().beatMapStartTime = Time.time;
    }

    private IEnumerator SpawnBeatMap(List<Dictionary<string, float>> beatMap)
    {
        float beatMapStartTime = Time.time;

        // Pre-spawning notes to cater to noteSpeedTimings being longer than the "timeStamp" of the note
        while (currentNote < beatMap.Count)
        {
            if (beatMap[currentNote]["timeStamp"] - noteSpeedTimings[noteSpeed] >= 0)
            {
                break;
            }

            SpawnNote(beatMap[currentNote], true);
            currentNote++;
        }

        // Spanws notes that are not pre-spawned
        while (currentNote < beatMap.Count) {
            float currentTimeStamp = Time.time - beatMapStartTime;
            if (currentTimeStamp >= beatMap[currentNote]["timeStamp"] - noteSpeedTimings[noteSpeed])
            {
                SpawnNote(beatMap[currentNote], false);
                currentNote++;
            }    
            yield return null;
        }
    }

    // Spawn, update relevant variables in the instance of the new note and give it AND its timing(s) to logicManager 
    private void SpawnNote(Dictionary<string, float> note, bool preSpawn)
    {
        GameObject newNote;

        // noteCircle
        if (note["typeOfNote"] == 0f)
        {
            newNote = Instantiate(noteCircle, transform.position, transform.rotation);
            newNote.GetComponent<NoteCircleScript>().noteSpeedTiming = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = note["timeStamp"];
            }
            else
            {
                newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }

            logicManager.GetComponent<LogicManagerScript>().circleObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().circleTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "timeStamp", note["timeStamp"] }
                    }
                );
        }

        // noteSquare
        else if (note["typeOfNote"] == 1f)
        {
            newNote = Instantiate(noteSquare, transform.position, transform.rotation);
            newNote.GetComponent<NoteSquareScript>().holdDuration = note["timeStampRelease"] - note["timeStamp"];
            newNote.GetComponent<NoteSquareScript>().noteSpeedTiming = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = note["timeStamp"];
            }
            else
            {
                newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }

            logicManager.GetComponent<LogicManagerScript>().squareObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().squareTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "timeStamp", note["timeStamp"] },
                        { "duration", note["timeStampRelease"] - note["timeStamp"] }
                    }
                );
        }

        // noteTriangle
        else if (note["typeOfNote"] == 2f)
        {
            newNote = Instantiate(noteTriangle, transform.position, transform.rotation);
            newNote.GetComponent<NoteTriangleScript>().noteSpeedTiming = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = note["timeStamp"];
            }
            else
            {
                newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }

            logicManager.GetComponent<LogicManagerScript>().triangleObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().triangleTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "timeStamp", note["timeStamp"] },
                    }
                );
        }
    }
}

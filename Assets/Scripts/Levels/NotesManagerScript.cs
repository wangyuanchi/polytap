using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // Referencing the index of beatMap
    private int currentNote = 0;
    private List<Dictionary<string, string>> beatMap;
    private float beatMapStartTime;

    // noteSpeed timings { noteSpeed, timeSpawnToJudgement }
    private Dictionary<int, float> noteSpeedTimings = new Dictionary<int, float>
    {
        { 1, 5.5f },
        { 2, 5f },
        { 3, 4.5f },
        { 4, 4f },
        { 5, 3.5f },
        { 6, 3f },
        { 7, 2.5f },
        { 8, 2f },
        { 9, 1.5f },
        { 10, 1f }
    };

    // Start is called before the first frame update
    void Start()
    {
        string levelName = SceneManager.GetActiveScene().name;
        string filepath = $"Assets\\Resources\\{levelName}.csv";
        beatMap = MarkersProcessingScript.ProcessMarkers(filepath);
        
        noteSpeed = PlayerPrefs.GetInt("Note Speed");
        beatMapStartTime = Time.time;
        StartCoroutine(SpawnBeatMap(beatMap));

        // Give LogicManagerScript and UIManagerScript the exact timing the beatmap starts
        logicManager.GetComponent<LogicManagerScript>().beatMapStartTime = beatMapStartTime;
        UIManager.GetComponent<UIManagerScript>().beatMapStartTime = beatMapStartTime;
    }

    // For checking the correctness of the beatmap
    private string BeatMapToString(List<Dictionary<string, string>> beatMap)
    {
        string beatMapString = "";
        foreach (var dict in beatMap)
        {
            beatMapString += "[New Note] ";
            foreach (var kvp in dict)
            {
                beatMapString += $"Key: {kvp.Key}, Value: {kvp.Value} ";
            }
        }
        return beatMapString; 
    }

    private IEnumerator SpawnBeatMap(List<Dictionary<string, string>> beatMap)
    {
        // Pre-spawning notes to cater to noteSpeedTimings being longer than the "timeStamp" of the note
        while (currentNote < beatMap.Count)
        {
            if (float.Parse(beatMap[currentNote]["timeStamp"]) < noteSpeedTimings[noteSpeed])
            {
                SpawnNote(beatMap[currentNote], true);
                currentNote++;
            }
            else { break; }
        }

        // Spawns notes that are not pre-spawned
        while (currentNote < beatMap.Count) {
            float currentTimeStamp = Time.time - beatMapStartTime;
            if (currentTimeStamp >= float.Parse(beatMap[currentNote]["timeStamp"]) - noteSpeedTimings[noteSpeed])
            {
                SpawnNote(beatMap[currentNote], false);
                currentNote++;
            }    
            yield return null;
        }
    }

    // Spawn, update relevant variables in the instance of the new note and give it AND its timing(s) to logicManager 
    private void SpawnNote(Dictionary<string, string> note, bool preSpawn)
    {
        GameObject newNote;

        if (note["typeOfNote"] == "C")
        {
            newNote = Instantiate(noteCircle, transform.position, transform.rotation);
            newNote.GetComponent<NoteCircleScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = float.Parse(note["timeStamp"]);
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
                        { "timeStamp", float.Parse(note["timeStamp"]) }
                    }
                );
        }

        else if (note["typeOfNote"] == "S")
        {
            newNote = Instantiate(noteSquare, transform.position, transform.rotation);
            newNote.GetComponent<NoteSquareScript>().holdDuration = float.Parse(note["timeStampRelease"]) - float.Parse(note["timeStamp"]);
            newNote.GetComponent<NoteSquareScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = float.Parse(note["timeStamp"]);
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
                        { "timeStamp", float.Parse(note["timeStamp"]) },
                        { "duration", float.Parse(note["timeStampRelease"]) - float.Parse(note["timeStamp"]) }
                    }
                );
        }

        else if (note["typeOfNote"] == "T")
        {
            newNote = Instantiate(noteTriangle, transform.position, transform.rotation);
            newNote.GetComponent<NoteTriangleScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];

            if (preSpawn)
            {
                newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = float.Parse(note["timeStamp"]);
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
                        { "timeStamp", float.Parse(note["timeStamp"]) },
                    }
                );
        }
    }
}

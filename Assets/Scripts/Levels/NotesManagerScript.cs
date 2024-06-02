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
    [SerializeField] private GameObject PracticeManager;
    [SerializeField] private GameObject levelMusic;

    [Header("Managers")]
    // Referencing the index of beatMap
    private int currentNote = 0;
    private List<Dictionary<string, string>> beatMap;
    [SerializeField] private Coroutine beatMapCoroutine;

    // noteSpeed timings { noteSpeed, timeSpawnToJudgement }
    private Dictionary<int, float> noteSpeedTimings = new Dictionary<int, float>
    {
        { 1, 3f },
        { 2, 2.5f },
        { 3, 2f },
        { 4, 1.75f },
        { 5, 1.5f },
        { 6, 1.25f },
        { 7, 1f },
        { 8, 0.75f },
        { 9, 0.5f }
    };

    // Start is called before the first frame update
    void Start()
    {
        string levelName = StaticInformation.level;
        string filepath = $"Assets\\Resources\\BeatMaps\\{levelName}.csv";
        beatMap = MarkersProcessingScript.ProcessMarkers(filepath);
        
        noteSpeed = PlayerPrefs.GetInt("Note Speed");

        // Non-practice mode spawning, practice mode spawning is done in PracticeManagerScript
        if (!PracticeManagerScript.practiceMode)
        {
            beatMapCoroutine = StartCoroutine(SpawnBeatMap(beatMap, 0f));
        }
    }

    // For checking the correctness of the beatmap
    private string BeatMapToString(List<Dictionary<string, string>> beatMap)
    {
        string beatMapString = "[NEW BEATMAP LOADED]\n";
        foreach (var dict in beatMap)
        {
            foreach (var kvp in dict)
            {
                beatMapString += $"{kvp.Key}: {kvp.Value}, ";
            }
            beatMapString += "\n";
        }
        return beatMapString; 
    }

    // [PRACTICE MODE] MAIN METHOD
    public void SkipToTime(float timeSkipped)
    {
        // Stop current running beatmap
        if (beatMapCoroutine != null)
        {
            StopCoroutine(beatMapCoroutine);
        }

        // Destroy all note game objects, clear note and timing queues
        logicManager.GetComponent<LogicManagerScript>().ResetBeatMap();

        // Start a new beatmap 
        currentNote = 0;
        beatMapCoroutine = StartCoroutine(SpawnBeatMap(beatMap, timeSkipped));
        UIManager.GetComponent<UIManagerScript>().LoadDifficulty();
    }

    // [PRACTICE MODE] Using timeSkipped to change values only take place in the spawning of beat map (and prespawning),
    // but not logic calculation in logicManager or anywhere else
    private IEnumerator SpawnBeatMap(List<Dictionary<string, string>> beatMap, float timeSkipped)
    {
        // Initial processing of notes that are not spawned and notes that are pre-spawned
        while (currentNote < beatMap.Count)
        {
            float currentNoteTimeStamp = float.Parse(beatMap[currentNote]["timeStamp"]) - timeSkipped;

            // If the note has a "negative" timeStamp
            if (currentNoteTimeStamp < 0)
            {
                currentNote++;
                continue;
            }

            // Pre-spawning notes to cater to noteSpeedTimings being longer than the "timeStamp" of the note
            if (currentNoteTimeStamp < noteSpeedTimings[noteSpeed])
            {
                SpawnNote(beatMap[currentNote], true, timeSkipped);
                currentNote++;
            }
            else { break; }
        }

        // Spawns notes that are not pre-spawned
        while (currentNote < beatMap.Count) {
            float currentTimeStamp = levelMusic.GetComponent<LevelMusicScript>().getCurrentTimeStamp() - timeSkipped;
            float currentNoteTimeStamp = float.Parse(beatMap[currentNote]["timeStamp"]) - timeSkipped;

            if (currentTimeStamp >= currentNoteTimeStamp - noteSpeedTimings[noteSpeed])
            {
                SpawnNote(beatMap[currentNote], false, timeSkipped);
                currentNote++;
            }    
            yield return null;
        }
    }

    // Spawn, update relevant variables in the instance of the new note and give it AND its timing(s) to logicManager 
    private void SpawnNote(Dictionary<string, string> note, bool preSpawn, float timeSkipped)
    {
        GameObject newNote;

        // Example: If the player is always 10ms late on every note,
        // global offset will be -10ms and every note needs to be detected 10ms later.
        // Hence, the global offset needs to be DEDUCTED from the normal timeStamp in the notesTimingsQueue
        float globalOffset = (float)PlayerPrefs.GetInt("Global Offset") / 1000; // Convert ms to s

        // [PRACTICE MODE] Need to include timeSkipped for prespawns if not it will base off old timestamps
        float noteTimeStamp = float.Parse(note["timeStamp"]) - timeSkipped;

        if (note["typeOfNote"] == "C")
        {
            newNote = Instantiate(noteCircle, transform.position, transform.rotation);

            // Give data to note specific script for spawning of note
            newNote.GetComponent<NoteCircleScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            if (preSpawn)
            {
                newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = noteTimeStamp;
            }
            else
            {
                newNote.GetComponent<NoteCircleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }
            newNote.GetComponent<NoteCircleScript>().SetSprite(float.Parse(note["accuracyWindow"]));

            // Give data to logic manager script for input processing
            logicManager.GetComponent<LogicManagerScript>().circleObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().circleTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "accuracyWindow", float.Parse(note["accuracyWindow"]) },
                        { "timeStamp", float.Parse(note["timeStamp"]) - globalOffset }
                    }
                );
        }

        else if (note["typeOfNote"] == "S")
        {
            newNote = Instantiate(noteSquare, transform.position, transform.rotation);

            // Give data to note specific script for spawning of note
            newNote.GetComponent<NoteSquareScript>().holdDuration = float.Parse(note["timeStampRelease"]) - float.Parse(note["timeStamp"]);
            newNote.GetComponent<NoteSquareScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            if (preSpawn)
            {
                newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = noteTimeStamp;
            }
            else
            {
                newNote.GetComponent<NoteSquareScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }
            newNote.GetComponent<NoteSquareScript>().SetSprite(float.Parse(note["accuracyWindow"]));

            // Give data to logic manager script for input processing
            logicManager.GetComponent<LogicManagerScript>().squareObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().squareTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "accuracyWindow", float.Parse(note["accuracyWindow"]) },
                        { "timeStamp", float.Parse(note["timeStamp"]) - globalOffset },
                        { "duration", float.Parse(note["timeStampRelease"]) - float.Parse(note["timeStamp"]) }
                    }
                );
        }

        else if (note["typeOfNote"] == "T")
        {
            newNote = Instantiate(noteTriangle, transform.position, transform.rotation);

            // Give data to note specific script for spawning of note
            newNote.GetComponent<NoteTriangleScript>().defaultTimeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            if (preSpawn)
            {
                newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = noteTimeStamp;
            }
            else
            {
                newNote.GetComponent<NoteTriangleScript>().timeSpawnToJudgement = noteSpeedTimings[noteSpeed];
            }
            newNote.GetComponent<NoteTriangleScript>().SetSprite(float.Parse(note["accuracyWindow"]));

            // Give data to logic manager script for input processing
            logicManager.GetComponent<LogicManagerScript>().triangleObjectsQueue.Enqueue(newNote);
            logicManager.GetComponent<LogicManagerScript>().triangleTimingsQueue.Enqueue
                (
                    new Dictionary<string, float>
                    {
                        { "accuracyWindow", float.Parse(note["accuracyWindow"]) },
                        { "timeStamp", float.Parse(note["timeStamp"]) - globalOffset },
                    }
                );
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManagerScript : MonoBehaviour
{

    public GameObject noteCircle;
    public GameObject logicManager;
    public int noteSpeed = 3;

    // WARNING: timeStamp (near the start) cannot be at a timing earlier than that of timeToTap
    private List<Dictionary<string, float>> beatMap = new List<Dictionary<string, float>>
    {
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 6f } },
        new Dictionary<string, float> { { "typeOfNote", 0f }, { "timeStamp", 10f } }
    };
    // Referencing the index of beatMap
    private int currentNote = 0;
    // noteSpeed timings { noteSpeed, timeToTap }
    private Dictionary<int, int> noteSpeedTimings = new Dictionary<int, int>
    {
        { 1, 10 },
        { 2, 8 },
        { 3, 6 },
        { 4, 4 },
        { 5, 2 }
    };


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBeatMap(beatMap));

        // Give LogicManagerScript the exact timing the beatmap starts
        logicManager.GetComponent<LogicManagerScript>().beatMapStartTime = Time.time;

        // Give beatMap to LogicManager
        logicManager.GetComponent<LogicManagerScript>().beatMap = beatMap;
    }

    IEnumerator SpawnBeatMap(List<Dictionary<string, float>> beatMap) {
        // If the currentNote exists
        while (currentNote < beatMap.Count) {
            float timeUntilSpawn = beatMap[currentNote]["timeStamp"] - noteSpeedTimings[noteSpeed];
            // Wait for timing to hit spawn time, then spawn the note and go to the next note
            yield return new WaitForSeconds(timeUntilSpawn);
            SpawnNote(beatMap[currentNote]);
            currentNote++;
        }
    }

    // Spawn and update variables in the instance of the new note
    void SpawnNote(Dictionary<string, float> note) {

        // noteCircle
        if (note["typeOfNote"] == 0f) {
            GameObject newNote = Instantiate(noteCircle, transform.position, transform.rotation);
            newNote.GetComponent<NoteCircleScript>().timeToTap = noteSpeedTimings[noteSpeed];
        }
    }

}

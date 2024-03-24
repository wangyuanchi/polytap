using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManagerScript : MonoBehaviour
{

    public GameObject noteCircle;
    public GameObject logicManager;

    // {"typeOfNote", "timeToSpawn", "timeToTap"}
    private List<List<float>> beatMap = new List<List<float>>{
            new List<float> { 0f, 0f, 5f },
            new List<float> { 0f, 3f, 5f }
        };
    // Referencing the index of beatMap
    private int currentNote = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBeatMap(beatMap));

        // Give LogicManagerScript the exact timing the beatmap starts
        logicManager.GetComponent<LogicManagerScript>().beatMapStartTime = Time.time;

        // Give all tapTimings to LogicManager
        foreach (List<float> note in beatMap)
        {
            logicManager.GetComponent<LogicManagerScript>().allTapTimings.Add(note[1] + note[2]);
        }
    }

    IEnumerator SpawnBeatMap(List<List<float>> beatMap) {
        // If the currentNote exists
        while (currentNote < beatMap.Count) { 
            // Wait for timing to hit timeToSpawn, then spawn the note and go to the next note
            yield return new WaitForSeconds(beatMap[currentNote][1]);
            SpawnNote((int)beatMap[currentNote][0]);
            currentNote++;
        }
    }

    void SpawnNote(int noteIndex) {

        // Dictionary for all notes
        Dictionary<int, GameObject> notesDictionary = new Dictionary<int, GameObject>{
            {0, noteCircle}
        };

        // Spawn a new note
        GameObject newNote = Instantiate(notesDictionary[noteIndex], transform.position, transform.rotation);

        // Update variables in the instance of the new note and overall tapTimings
        if (noteIndex == 0) {
            newNote.GetComponent<NoteCircleScript>().timeToTap = beatMap[currentNote][2];
        }
    }

}

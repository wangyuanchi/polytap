using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{

    public GameObject noteCircle;

    // {"typeOfNote", "timeToSpawn", "noteSpeed"}
    private List<List<float>> beatMap = new List<List<float>>{
            new List<float> { 0, 0, 0.5f },
            new List<float> { 0, 3, 0.5f }
        };
    // referencing the index of beatMap
    private int currentNote = 0;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.timeSinceLevelLoad;

        // Spawn a new note if currentNote exists and elapsedTime is larger than timeToSpawn of currentNote
        if (currentNote < beatMap.Count) {
            if (elapsedTime >= beatMap[currentNote][1]) {
                SpawnNote((int)beatMap[currentNote][0]);
                currentNote++;
            }
        }
    }
    void SpawnNote(int noteIndex) {

        // Dictionary for all notes
        Dictionary<int, GameObject> notesDictionary = new Dictionary<int, GameObject>{
            {0, noteCircle}
        };

        // Spawn a new note
        GameObject newNote = Instantiate(notesDictionary[noteIndex], transform.position, transform.rotation);

        // Update variables of the instance of the new note
        if (noteIndex == 0) {
            newNote.GetComponent<NoteCircleScript>().noteSpeed = beatMap[currentNote][2];
        }
    }

}

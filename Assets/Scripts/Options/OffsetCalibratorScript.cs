using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OffsetCalibratorScript : MonoBehaviour
{
    [Header("Notes")]
    [SerializeField] GameObject calibrationNote;

    [Header("Audio")]
    [SerializeField] AudioSource calibrationMetronome;
    [SerializeField] AudioSource lobbyMusic;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference circleActionReference;

    private Queue<GameObject> noteQueue = new Queue<GameObject>();
    private Queue<float> timingQueue = new Queue<float>();

    private float timer = 0;
    private float timeToWait;
    private float totalTime;
    private float average;
    private float counter = 0;
    Coroutine calibration;
    private void OnEnable()
    {
        gameObject.SetActive(true);
        float startTime = Time.time;
        timeToWait = (60f / 100f) * 4f;      //amount of seconds between 4 notes of 100bpm
        timingQueue.Enqueue(timeToWait);
        calibrationMetronome.Play();
        lobbyMusic.Stop();
        calibration = StartCoroutine(startCalibration());
        circleActionReference.action.Enable();
        circleActionReference.action.performed += OnCircle;
    }

    private IEnumerator startCalibration()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (counter >= 10)
            {
                new WaitForSeconds(4);
                average = totalTime * 1000 / 10;
                PlayerPrefs.SetInt("Global Offset", (int)average);
                calibrationMetronome.Stop();
                lobbyMusic.Play();
                circleActionReference.action.performed -= OnCircle;
                circleActionReference.action.Disable();
                gameObject.SetActive(false);
                yield break;
            }
            if (timer > timingQueue.Peek())
            {
                GameObject newNote = Instantiate(calibrationNote, new Vector3(500, 0, 0), new Quaternion(0,0,0,0)) ;
                newNote.transform.SetParent(gameObject.transform, false);
                noteQueue.Enqueue(newNote);
                timingQueue.Dequeue();
                optimalTiming = Time.time + 1;
                timingQueue.Enqueue(timeToWait);
                timer = 0;
                counter++;
            }
            yield return null;
        }
    }
    float optimalTiming;
    private void OnCircle(InputAction.CallbackContext context)
    {
        float timing = Time.time;
        Debug.Log(optimalTiming - timing);
        totalTime += optimalTiming - timing;
        Debug.Log(totalTime);
        if (noteQueue.Count != 0)
        {
            Destroy(noteQueue.Dequeue());
        }
    }
}

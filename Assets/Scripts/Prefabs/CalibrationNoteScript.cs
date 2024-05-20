 using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalibrationNoteScript : MonoBehaviour
{
    public void StartMove()
    {
        // Move note from starting position to new position at 1000 units left
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x - 1000, start.y, start.z);

        // The time in seconds between 4 notes of 100bpm
        float interval = (60f / 100f) * 4f;
        StartCoroutine(MoveToPosition(end, interval));
    }

    private IEnumerator MoveToPosition(Vector3 position, float totalDuration)
    {
        float time = 0f;
        Vector3 initialPos = transform.position;

        while (time < totalDuration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, position, time / totalDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}

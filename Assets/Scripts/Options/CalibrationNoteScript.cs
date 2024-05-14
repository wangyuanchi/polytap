using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalibrationNoteScript : MonoBehaviour
{
    // Start is called before the first frame update
    

    Vector3 start;
    Vector3 end;
    void Start()
    {
        gameObject.SetActive(true);
        start = transform.position;
        end = new Vector3(start.x-1000,start.y,start.z);
        float timeToMove = 2;
        StartCoroutine(MoveToPosition(transform, end, timeToMove));
    }

    private IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        transform.position = position;
        Destroy(gameObject);
    }
}

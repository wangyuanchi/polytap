using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCircleScript : MonoBehaviour
{

    public float noteSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increase the scale of the note according to noteSpeed from ShapesManager
        transform.localScale += Vector3.one * noteSpeed * Time.deltaTime;
    }

}

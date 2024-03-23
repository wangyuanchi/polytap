using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Tap pressed");
        }
    }

}

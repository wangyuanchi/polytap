using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindsScript : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference circleActionReference;
    [SerializeField] private InputActionReference squareActionReference;
    [SerializeField] private InputActionReference triangleActionReference;

    private void OnEnable()
    {
        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
    }

    private void OnDisable()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();
    }
}

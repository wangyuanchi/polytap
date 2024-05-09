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
    [SerializeField] private InputActionReference pauseActionReference;
    [SerializeField] private InputActionReference forwardsActionReference;
    [SerializeField] private InputActionReference backwardsActionReference;
    [SerializeField] private InputActionReference checkpointActionReference;

    private void OnEnable()
    {
        circleActionReference.action.Disable();
        squareActionReference.action.Disable();
        triangleActionReference.action.Disable();
        pauseActionReference.action.Disable();
        forwardsActionReference.action.Disable();
        backwardsActionReference.action.Disable();
        checkpointActionReference.action.Disable();
    }

    private void OnDisable()
    {
        circleActionReference.action.Enable();
        squareActionReference.action.Enable();
        triangleActionReference.action.Enable();
        pauseActionReference.action.Enable();
        forwardsActionReference.action.Enable();
        backwardsActionReference.action.Enable();
        checkpointActionReference.action.Enable();
    }
}

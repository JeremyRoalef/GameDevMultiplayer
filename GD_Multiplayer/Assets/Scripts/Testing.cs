using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour
{
    [SerializeField]
    InputActionReference moveInput;

    private void OnEnable()
    {
        moveInput.action.performed += HandleMovePerformed;
    }

    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }
}

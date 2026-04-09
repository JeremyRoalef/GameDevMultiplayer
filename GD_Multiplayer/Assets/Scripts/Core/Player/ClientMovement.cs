using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientMovement : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField]
    InputActionReference moveBody;

    [SerializeField]
    InputActionReference moveTurret;

    [Header("Prefab References")]
    [SerializeField]
    Transform bodyTransform;

    [SerializeField]
    Transform turretPivotTransform;

    [SerializeField]
    Rigidbody2D clientRb;

    [Header("Settings")]
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float turnRate;


    Vector2 moveInput;

    public override void OnNetworkSpawn()
    {
        //Only owner can move this object
        if (!IsOwner) return;
        
        //Get the new movement when the player performs the movement.
        //Also, when the player cancels all input, send that too to stop the player
        moveBody.action.performed += MoveClient;
        moveBody.action.canceled += MoveClient;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        moveBody.action.performed -= MoveClient;
        moveBody.action.canceled -= MoveClient;
    }

    private void Update()
    {
        //Only the owner can move this object
        if (!IsOwner) return;

        //Calculate the rotation amount based on the movement input
        float zRotation = moveInput.x * -turnRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        //Only the owner can move this object
        if (!IsOwner) return;

        //Set the rigidbody velocity based on the move input
        clientRb.linearVelocity = (Vector2)bodyTransform.up * 
            moveInput.y * 
            moveSpeed;
    }

    private void MoveClient(InputAction.CallbackContext context)
    {
        //Get the movement input
        moveInput = context.ReadValue<Vector2>();
    }
}

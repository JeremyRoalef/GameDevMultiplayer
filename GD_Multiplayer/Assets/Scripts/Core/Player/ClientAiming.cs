using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientAiming : NetworkBehaviour
{
    [SerializeField]
    InputActionReference rotateTurretPolar;

    [SerializeField]
    Transform turretPivotTransform;

    [SerializeField]
    float rotateSpeed;

    /// <summary>
    /// Rotation will happen relative to the mathematical polar coordinate system
    /// In other words. Rotating counter-clockwise is in the positive direction, and
    /// rotating clockwise is in the negative direction.
    /// </summary>
    float rotateDirAsPolar;

    public override void OnNetworkSpawn()
    {
        //Only the owner can rotate this object
        if (!IsOwner) return;

        rotateTurretPolar.action.performed += RotateTurretPivot;
        rotateTurretPolar.action.canceled += RotateTurretPivot;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        rotateTurretPolar.action.performed -= RotateTurretPivot;
        rotateTurretPolar.action.canceled -= RotateTurretPivot;
    }

    private void Update()
    {
        //Only the owner can rotate this object
        if (!IsOwner) return;

        turretPivotTransform.Rotate(
            0f, 
            0f, 
            rotateDirAsPolar * rotateSpeed * Time.deltaTime
            );
    }
    private void RotateTurretPivot(InputAction.CallbackContext context)
    {
        rotateDirAsPolar = context.ReadValue<float>();
    }
}

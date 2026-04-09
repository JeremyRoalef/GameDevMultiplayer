using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientFire : NetworkBehaviour
{
    [Header("Input References")]
    [SerializeField]
    InputActionReference fire;

    [Header("Object References")]
    [SerializeField]
    Transform projectileSpawnPoint;

    [Header("Prefabs")]
    [SerializeField]
    GameObject serverProjectilePrefab;

    [SerializeField]
    GameObject clientProjectilePrefab;

    public override void OnNetworkSpawn()
    {
        //Only the owner of this game object can fire on this object
        if (!IsOwner) return;

        fire.action.performed += FireProjectile;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        fire.action.performed -= FireProjectile;
    }

    private void FireProjectile(InputAction.CallbackContext context)
    {
        //Create the dummy owner projectile. then, tell the server to spawn the projectile for everyone else
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        
        //Spawn the projectile on the server
        SpawnServerProjectileRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void SpawnDummyProjectile(Vector3 position, Vector3 upDir)
    {
        //Instantiate the client prefab
        GameObject projectileObj = Instantiate(clientProjectilePrefab, position, Quaternion.identity);
        projectileObj.transform.up = upDir;
    }

    [Rpc(SendTo.Server)]
    void SpawnServerProjectileRpc(Vector3 position, Vector3 upDir)
    {
        //Instantiate the client prefab
        GameObject projectileObj = Instantiate(serverProjectilePrefab, position, Quaternion.identity);
        projectileObj.transform.up = upDir;

        //Tell clients to spawn a dummy projectile on their end
        SpawnDummyProjectileClientRpc(position, upDir);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SpawnDummyProjectileClientRpc(Vector3 position, Vector3 upDir)
    {
        //If you own the object, then you already spawned the projectile; don't spawn another one
        if (IsOwner) return;

        SpawnDummyProjectile(position, upDir);
    }
}

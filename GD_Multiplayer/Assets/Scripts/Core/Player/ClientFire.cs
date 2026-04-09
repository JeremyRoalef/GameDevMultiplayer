using System;
using System.Collections;
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

    [SerializeField]
    GameObject muzzleFlash;

    [SerializeField]
    Collider2D playerCollider;

    [Header("Prefabs")]
    [SerializeField]
    Projectile serverProjectilePrefab;

    [SerializeField]
    Projectile clientProjectilePrefab;

    [Header("Settings")]
    [SerializeField, Min(0)]
    float projectileSpeed;

    [SerializeField, Min(0)]
    float muzzleFlashDuration;

    [SerializeField, Min(0)]
    float fireRate;

    bool canFire = true;

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
        if (!canFire) return;

        //Handle player fired internally
        StartCoroutine(CreateMuzzleFlash());
        StartCoroutine(BeginFireCooldown());

        //Create the dummy owner projectile. then, tell the server to spawn the projectile for everyone else
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        
        //Spawn the projectile on the server
        SpawnServerProjectileRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void SpawnDummyProjectile(Vector3 position, Vector3 upDir)
    {
        //Instantiate the client prefab
        Projectile projectileObj = Instantiate(clientProjectilePrefab, position, Quaternion.identity);
        projectileObj.transform.up = upDir;
        projectileObj.Initialize(projectileSpeed);
        Physics2D.IgnoreCollision(playerCollider, projectileObj.GetCollider());
    }

    IEnumerator CreateMuzzleFlash()
    {
        //Enable the muzzle flash
        muzzleFlash.SetActive(true);

        //Wait the flash duration before turning off the flash
        yield return new WaitForSeconds(muzzleFlashDuration);

        //Disable the muzzle flash
        muzzleFlash.SetActive(false);
    }

    IEnumerator BeginFireCooldown()
    {
        //Disable firing capablility
        canFire = false;

        //Determine how long you need to wait before you can fire again. Handle zero division
        float waitDuration = fireRate == 0? 0 : 1 / fireRate;

        //Wait before re-enabling can fire property
        yield return new WaitForSeconds(waitDuration);
        
        //Client can fire again
        canFire = true;
    }

    [Rpc(SendTo.Server)]
    void SpawnServerProjectileRpc(Vector3 position, Vector3 upDir)
    {
        //Instantiate the client prefab
        Projectile projectileObj = Instantiate(serverProjectilePrefab, position, Quaternion.identity);
        projectileObj.transform.up = upDir;
        projectileObj.Initialize(projectileSpeed);
        Physics2D.IgnoreCollision(playerCollider, projectileObj.GetCollider());

        //Tell clients to spawn a dummy projectile on their end
        SpawnDummyProjectileClientRpc(position, upDir);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SpawnDummyProjectileClientRpc(Vector3 position, Vector3 upDir)
    {
        //If you own the object, then you already spawned the projectile; don't spawn another one
        if (IsOwner) return;

        SpawnDummyProjectile(position, upDir);

        //Create the muzzle flash on all other clients
        StartCoroutine(CreateMuzzleFlash());
    }
}

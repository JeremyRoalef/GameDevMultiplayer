using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField, Min(0)] //field: used for property serialization
    public int MaxHp { get; private set; } = 100;
    public NetworkVariable<int> CurrentHp = new NetworkVariable<int>();

    public Action<Health> OnDealth;
    bool isDead;

    public override void OnNetworkSpawn()
    {
        //Only the server can modify the client's health
        if (!IsServer) return;

        CurrentHp.Value = MaxHp;
        CurrentHp.OnValueChanged += CheckForDeath;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        CurrentHp.OnValueChanged -= CheckForDeath;
    }

    public void TakeDamage(int amount)
    {
        //If dead, then don't do anything
        if (isDead) return;

        //If damage to be taken is negative, then you actually heal
        if (amount < 0)
        {
            RestoreHp(Mathf.Abs(amount));
            return;
        }

        //Calculate the player's health & modift their health
        int newHealth = CurrentHp.Value - amount;
        UpdateHealth(newHealth);
    }

    public void RestoreHp(int amount)
    {
        //If dead, then don't do anything
        if (isDead) return;

        //If health to restore is negative, then you actually take damage
        if (amount < 0)
        {
            TakeDamage(Mathf.Abs(amount));
            return;
        }

        //Calculate the player's new health
        int newHealth = CurrentHp.Value + amount;
        UpdateHealth(newHealth);
    }

    void UpdateHealth(int newValue)
    {
        //Clamp the object's health between 0 and their max HP
        int clampedHpValue = Mathf.Clamp(newValue, 0, MaxHp);

        //If new health is same as old, then no change was made; don't do anything
        if (clampedHpValue == CurrentHp.Value) return;

        //Set the object's new health value
        CurrentHp.Value = clampedHpValue;
    }

    private void CheckForDeath(int previousValue, int newValue)
    {
        if (isDead) return; //Object is already dead

        if (newValue == 0)
        {
            //Object has died
            OnDealth?.Invoke(this);
            isDead = true;
        }
    }
}

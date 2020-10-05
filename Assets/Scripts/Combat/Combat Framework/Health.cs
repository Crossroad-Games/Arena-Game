using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Rendering;

public class Health : NetworkBehaviour
{
    [SerializeField] CharacterData playerStats;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    [SerializeField] private float health = 0f;

    public static Action OnDeath;
    public event Action OnHealthChanged;

    public bool IsDead => health == 0f;


    public override void OnStartServer()
    {
        health = playerStats.MaxHealth;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);

        health = Mathf.Min(health + value, playerStats.MaxHealth);  
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);

        health = Mathf.Max(health - value, 0);

        if(health == 0)
        {
            OnDeath?.Invoke();

            RpcHandleDeath();
        }
    }

    private void HandleHealthUpdated(float oldValue, float newValue)
    {
        OnHealthChanged?.Invoke();
    }

    [ClientRpc]
    private void RpcHandleDeath()
    {
        gameObject.SetActive(false);
    }
}

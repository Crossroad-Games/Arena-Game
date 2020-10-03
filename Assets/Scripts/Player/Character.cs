using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Character : NetworkBehaviour
{
    [Header("Initial Stats")]
    [SerializeField] private CharacterData playerStats;
    public int currentHealth;



    private void Start()
    {
        currentHealth = playerStats.health; //Sets current health to initial health from character data
    }
}

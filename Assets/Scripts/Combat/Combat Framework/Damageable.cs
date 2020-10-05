using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public void DealDamage(float damageToDeal)
    {
        health.Remove(damageToDeal);
    }
}

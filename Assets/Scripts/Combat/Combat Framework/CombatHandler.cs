using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatHandler : NetworkBehaviour , IKillable, IDamageable<int>
{
    private Character player;


    private void Start()
    {
        player = GetComponent<Character>();
    }


    
    [Command]
    public virtual void CmdKill()
    {
        //TODO: Make player die?
        Debug.Log("Player morra fdp");
    }

    [Command]
    public virtual void CmdTakeDamage(int damageTaken) // tells the server to lower player's health
    {
        if (player.currentHealth > 0)
        {
            player.currentHealth -= damageTaken;
        }
        TargetUpdateHealth(this.GetComponent<NetworkIdentity>().connectionToClient, damageTaken); // uses the connectionToClient for the target
    }

    [TargetRpc]
    public void TargetUpdateHealth(NetworkConnection target, int damageTaken) // send the info back to player that he took damage
    {
        player.currentHealth -= damageTaken;
        if(player.currentHealth < 0)
        {
            CmdKill();
        }
    }
    
}

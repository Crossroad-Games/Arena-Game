using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar]
    private uint ownerId;
    public uint OwnerId => ownerId;

    #region References
    [SerializeField] private Transform AttackPos;
    #endregion

    [ClientCallback]
    private void Update()
    {
        if(hasAuthority)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                //Do basic attack for testing
            }
        }
    }
}

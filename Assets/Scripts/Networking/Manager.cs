using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Manager : NetworkManager
{
    private bool isReady = false;
    [SerializeField] private Vector3[] PlayerPositions;
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        if(networkSceneName == "Arena")
        {
            ClientScene.AddPlayer(conn);// Calls the "Add Player" function
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)// Called whenever a player is to be added to the server
    {
        StartCoroutine(HoldUntilReady(conn));// Wait until the arena is done loading before spawning the player
    }
    public void setReady(bool newReadyState)// Function designed to be called when the arena is finished spawning
    {
        isReady = newReadyState;// Change the current ready state
    }
    IEnumerator HoldUntilReady(NetworkConnection conn)
    {
        while (!isReady)// While the arena is still spawning
            yield return null;
        GameObject Player = Instantiate(Resources.Load("Prefabs/Player/Player")) as GameObject;// Instantiate the object
        Player.transform.position = PlayerPositions[conn.connectionId];// Set its position based on the player
        NetworkServer.AddPlayerForConnection(conn,Player);// Spawn the object 
        yield break;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    // Start is called before the first frame update
    private Material Black;// Variable of type Material that holds [reference] that will be manipulated in the future to change its opacity
    private NetworkManager Manager;// Variable of type NetworkManager that holds [reference] that will be called to change the scene
    void Start()
    {
        Manager = GameObject.Find("Manager").GetComponent<Manager>();// Reference to the script is set
        Black = GameObject.Find("DarkScreen").GetComponent<Renderer>().material;// Reference to the material is set
        StartCoroutine("FadeScreen");// Starts coroutine that will decrease the screen's opacity
    }
    [Command(ignoreAuthority = true)]
    public void CmdStartGame()// Function designed to move the players from the lobby to the game scene
    {
        NetworkServer.SetAllClientsNotReady();// Sets all the clients to be not ready
        Manager.ServerChangeScene("Arena");// Changes the scene where the players are located
    }
    IEnumerator FadeScreen()// Coroutine that runs parallel to the application and is responsible for changing the material's opacity
    {
        float Variable=0;
        Black.SetFloat("Thickness", 0.01f);
        //Color col = Color.black;// Base color
        while(Variable<1)// If the material is not yet transparent
        {
            Variable += Time.deltaTime;// Decrease opacity
            Black.SetFloat("_Fade", Variable);
            yield return null;// Return next frame
        }
        GameObject.Find("DarkScreen").SetActive(false);// Once the material is 100% transparent, deactivate it
    }
}

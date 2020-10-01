using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    private NetworkManager manager;
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        manager.networkAddress = "142.166.17.19";// Connect to this IP
    }

    private void LobbyStart()
    {
        //////***** Animation required *****///////
        transform.parent.Find("Lobby").gameObject.SetActive(true);// Activates the Lobby gameobject
        this.gameObject.SetActive(false);// Deactivates this gameobject
    }
    public void CloseGame()// Function designed to close the game application after an animation
    {
        /////***** Animation required *****//////
        Application.Quit();// Closes the application, only works on build
        Debug.Log("Closed the Application");// Test command to check if button is indeed working during editor mode
    }
    public void JoinGame()// Function designed to connect a client to the host
    {
        if (!NetworkClient.active)// If not connected to the Network
        {
            manager.StartClient();// Start a client
            LobbyStart();// Calls the Lobby Start function in order to transition into another BG
        }
    }
    public void Host()// Function designed to connect a Host (Sever+Client)
    {
        if (!NetworkClient.active)// If not connected to the Network
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)// ???
            {
                 manager.StartHost();// Start a Host
                 LobbyStart();// Calls the Lobby Start function in order to transition into another BG
            }
        }
    }
}

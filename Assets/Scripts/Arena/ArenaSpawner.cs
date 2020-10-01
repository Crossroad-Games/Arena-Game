using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ArenaSpawner : NetworkBehaviour
{
    // Start is called before the first frame update
    private Dictionary<string,GameObject> TileList= new Dictionary<string, GameObject>();// List that holds all the tile types
    [SerializeField] private Vector2 ArenaArea;// X and Z limits of the Arena
    [SerializeField] private float Delay;// Delay between each batch of spawns
    [SerializeField] private Vector3 TileScale= new Vector3(4,1,4);// The size of the Tile
    private Manager NetworkManager;// Reference to the Manager script is made in order to spawn the player once the grid is done spawning
    [Server]
    void Start()
    {
        NetworkManager = GameObject.Find("Manager").GetComponent<Manager>();
        foreach(GameObject Tile in Resources.LoadAll("Prefabs/Tiles"))// Acquires all the tiles from the resource folder "Tiles"
        {
            Debug.Log(Tile.name);
            TileList.Add(Tile.name,Tile);// Add the tile to the List
        }
        StartCoroutine("Spawner");// Start coroutine designed to spawn all the tiles without a huge loading time
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void SpawningMethod(Vector3 Position)// Function designed to easily identify the method of instantiating and spawning the object
    {
        GameObject SpawnedObject;// Object tha just got instantiated and needs to be spawned on the server
        SpawnedObject = Instantiate(TileList["BaseTile"], Position, Quaternion.identity);
        SpawnedObject.transform.localScale = TileScale;// Sets the scale of the tile to be the one on the inspector
        NetworkServer.Spawn(SpawnedObject);
        SpawnedObject.transform.SetParent(this.transform);
    }
    IEnumerator Spawner()
    {
        int Count=0;
        float SpawnHeight=5;// How high do the blocks spawn
        Vector3 Position;// Position each block will be instantiated
        yield return new WaitForSeconds(.1f);// Hold a bit to get all players loaded
        if (ArenaArea.x % 2 == 1 && ArenaArea.y % 2==1)// If the numbers are odd
        {
            if (ArenaArea.x >= ArenaArea.y)// If x is equal than or greater than the y
            {
                while (Count * 2 <= ArenaArea.x)// While the spawner haven't hit the limits
                {
                    for (float X = (ArenaArea.x / 2) - Count; X <= (ArenaArea.x / 2) + Count; X++)// Go through all X positions
                    {
                        if (X == (ArenaArea.x / 2) - Count || X == (ArenaArea.x / 2) + Count)// IF its on the edges
                            for (float Z = (ArenaArea.y / 2) - Count; Z <= (ArenaArea.y / 2) + Count; Z++)// Go through all Z positions
                            {
                                Position = new Vector3(X*TileScale.x, SpawnHeight, Z*TileScale.z);
                                SpawningMethod(Position);// Passes the Vector3 as a parameter to the spawning method
                            }
                        else// If its not on the edges of the batch
                        {
                            Position = new Vector3(X* TileScale.x, SpawnHeight, (((ArenaArea.y / 2) - Count))* TileScale.z);// Spawn one on bottom
                            SpawningMethod(Position);// Passes the Vector3 as a parameter to the spawning method
                            Position = new Vector3(X*TileScale.x, SpawnHeight, ((ArenaArea.y / 2) + Count)* TileScale.z);// Spawn another on top
                            SpawningMethod(Position);// Passes the Vector3 as a parameter to the spawning method
                        }
                    }
                    Count++;// Go to the next batch
                    yield return new WaitForSeconds(Delay);// Delay the next batch so it doesn't clutter the memory
                }
                NetworkManager.setReady(true);
            }
            else
                throw new System.ArgumentException("x can't be less than y");
        }
        else
            throw new System.ArgumentException("Can't be even");


    }
}

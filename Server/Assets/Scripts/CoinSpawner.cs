using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static Dictionary<int, CoinSpawner> spawners = new Dictionary<int, CoinSpawner>(); // Dictionary of item spawners
    private static int nextSpawnerId = 1; // ID of the next item spawner

    public int spawnerId; // ID of the item spawner
    public bool hasItem = false; // Boolean to check if the item spawner has an item

    void Start()
    {
        hasItem = false; // Set the item spawner to not have an item
        spawnerId = nextSpawnerId; // Set the ID of the item spawner
        nextSpawnerId++; // Increment the ID of the next item spawner

        spawners.Add(spawnerId, this); // Add the item spawner to the dictionary

        StartCoroutine(SpawnItem()); // Start the coroutine to spawn an item
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider is a player
        {
            Player player = other.GetComponent<Player>();
            if (player.AttemptPickUp())
            {
                ItemPickedUp(spawnerId, player.id); // Call the item picked up function
            }
            
        }
    }
    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(3); // Wait for 10 seconds

        hasItem = true; // Set the item spawner to have an item
        ServerSend.CoinSpawned(spawnerId); // Send the item spawn packet to clients
    }

    private void ItemPickedUp(int spawnerId, int byPlayer)
    {
        hasItem = false; // Set the item spawner to not have an item
        ServerSend.coinPickedUp(spawnerId, byPlayer); // Send the item picked up packet to clients
        StartCoroutine(SpawnItem()); // Start the coroutine to spawn an item
    }
}

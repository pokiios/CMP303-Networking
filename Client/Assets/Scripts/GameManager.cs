using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, CoinSpawner> spawners = new Dictionary<int, CoinSpawner>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject coinSpawnerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Set the instance to this GameManager
            DontDestroyOnLoad(gameObject); // Don't destroy the GameManager when loading a new scene
        }
        else
        {
            Destroy(gameObject); // If the instance already exists, destroy this GameManager
        }
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player; // Create a new GameObject called player

        if (id == Client.instance.id) // If the ID is the same as the client's ID
        {
            player = Instantiate(localPlayerPrefab, position, rotation); // Instantiate the local player prefab
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation); // Instantiate the player prefab
        }

        player.GetComponent<PlayerManager>().Initialize(id, username); // Initialize the player
        players.Add(id, player.GetComponent<PlayerManager>()); // Add the player to the dictionary of players
    }

    public void CreateCoinSpawner(int spawnerId, Vector3 position, bool hasItem)
    {
        GameObject coinSpawner = Instantiate(coinSpawnerPrefab, position, coinSpawnerPrefab.transform.rotation); // Instantiate the coin spawner prefab
        coinSpawner.GetComponent<CoinSpawner>().Initialize(spawnerId, hasItem); // Initialize the coin spawner
        spawners.Add(spawnerId, coinSpawner.GetComponent<CoinSpawner>()); // Add the coin spawner to the dictionary of coin spawners
    }
}

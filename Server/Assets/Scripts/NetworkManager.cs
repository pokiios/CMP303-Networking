using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Init Instance
    public static NetworkManager instance;

    public GameObject playerPrefab;

    // Create singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = 30; // Set the application target frame rate

        // Stop program from running in Unity Editor
        Server.Start(50, 25252); // Start the server with a maximum of 50 players and on port 25252
    }

    
    private void OnApplicationQuit()
    {
        Server.Stop(); // Stop the server
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Player>();
    }
}

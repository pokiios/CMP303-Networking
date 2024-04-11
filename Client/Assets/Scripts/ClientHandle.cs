using System.Collections;
using System.Collections.Generic;
using System.Net;
using GameServer;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString(); // Read the message from the packet
        int id = packet.ReadInt(); // Read the client's ID from the packet

        Debug.Log($"Message from server: {message}"); // Log the message to the console
        Client.instance.id = id; // Set the client's ID
        ClientSend.WelcomeReceived(); // Send the welcome received packet back to the server

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port); // Connect to the UDP server 
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt(); // Read the player's ID from the packet
        string username = packet.ReadString(); // Read the player's username from the packet
        Vector3 position = packet.ReadVector3(); // Read the player's position from the packet
        Quaternion rotation = packet.ReadQuaternion(); // Read the player's rotation from the packet

        GameManager.instance.SpawnPlayer(id, username, position, rotation); // Spawn the player
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt(); // Read the player's ID from the packet
        Vector3 position = packet.ReadVector3(); // Read the player's position from the packet

        if (GameManager.players.TryGetValue(id, out PlayerManager player)) // Get the player from the dictionary
        {
            player.transform.position = position; // Update the player's position
        }
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt(); // Read the player's ID from the packet
        Quaternion rotation = packet.ReadQuaternion(); // Read the player's rotation from the packet

        if (GameManager.players.TryGetValue(id, out PlayerManager player)) // Get the player from the dictionary
        {
            player.transform.rotation = rotation; // Update the player's rotation
        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = Client.instance.id; // Get the client's ID

        GameManager.players.Remove(id); // Remove the player from the dictionary
    }

    public static void CreateCoinSpawner(Packet packet)
    {
        int spawnerId = packet.ReadInt(); // Read the spawner's ID from the packet
        Vector3 spawnerPosition = packet.ReadVector3(); // Read the spawner's position from the packet
        bool hasItem = packet.ReadBool(); // Read if the spawner has an item from the packet

        GameManager.instance.CreateCoinSpawner(spawnerId, spawnerPosition, hasItem); // Create the coin spawner
    }

    public static void CoinSpawned(Packet packet)
    {
        int spawnerId = packet.ReadInt(); // Read the spawner's ID from the packet

        GameManager.spawners[spawnerId].CoinSpawned(); // Call the CoinSpawned method on the coin spawner
    }

    public static void CoinPickedUp(Packet packet)
    {
        int spawnerId = packet.ReadInt(); // Read the spawner's ID from the packet
        int byPlayer = packet.ReadInt(); // Read the player who picked up the coin

        GameManager.spawners[spawnerId].CoinPickedUp(); // Call the CoinPickedUp method on the coin spawner
        GameManager.players[byPlayer].points++; // Increment the player's points
    }
}

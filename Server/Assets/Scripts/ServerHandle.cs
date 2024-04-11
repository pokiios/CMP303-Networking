using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
   // Handle the welcome received packet
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt(); // Read the client's ID
        string username = packet.ReadString(); // Read the client's username

        Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}."); // Log the client's connection

        // Check if the client's ID is correct
        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!"); // Log the error
        }

        Server.clients[fromClient].SendIntoGame(username); // Send the client into the game
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        // Read the data from the packet
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();

        Server.clients[fromClient].player.SetInput(inputs, rotation); // Update the player's input
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    // Send a packet to a client
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet
        Server.clients[toClient].tcp.SendData(packet); // Send the packet to the specified client
    }

    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet
        Server.clients[toClient].udp.SendData(packet); // Send the packet to the specified client
    }

    // Send a packet to all clients
    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet

        // Loop through all clients
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(packet); // Send the packet client
        }
    }

    private static void SendTCPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet

        // Loop through all clients
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].tcp.SendData(packet); // Send the packet to all clients except one
            }
        }
    }

    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet

        // Loop through all clients
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].udp.SendData(packet); // Send the packet client
        }
    }

    private static void SendUDPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength(); // Add the packet length to the front of the packet

        // Loop through all clients
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].udp.SendData(packet); // Send the packet to all clients except one
            }
        }
    }

    public static void Welcome(int toClient, string message)
    {
        // Create a welcome packet, using function disposes of the packet after it's done
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(message); // Write the message to the packet
            packet.Write(toClient); // Write the client's ID to the packet

            SendTCPData(toClient, packet); // Send the packet to the client
        }
    }

    public static void SpawnPlayer(int toClient, Player player)
    {
        // Create a spawn player packet, using function disposes of the packet after it's done
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(player.id); // Write the player's ID to the packet
            packet.Write(player.username); // Write the player's username to the packet
            packet.Write(player.transform.position); // Write the player's position to the packet
            packet.Write(player.transform.rotation); // Write the player's rotation to the packet

            SendTCPData(toClient, packet); // Send the packet to the client
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id); // Write the player's ID to the packet
            packet.Write(player.transform.position); // Write the player's position to the packet

            SendUDPDataToAll(packet); // Send the packet to all clients
        }
    }

    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id); // Write the player's ID to the packet
            packet.Write(player.transform.rotation); // Write the player's rotation to the packet

            SendUDPDataToAll(player.id, packet); // Send the packet to all clients except one
        }
    }

    public static void playerDisconnected(int playerID)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected)) // Create a packet to send the player disconnected message
        {
            packet.Write(playerID); // Write the player's ID to the packet

            SendTCPDataToAll(packet); // Send the packet to all clients
        }
    }

    public static void CreateCoinSpawner(int toClient, int spawnerId, Vector3 position, bool hasItem)
    {
        using (Packet packet = new Packet((int)ServerPackets.createCoinSpawner))
        {
            packet.Write(spawnerId); // Write the spawner's ID to the packet
            packet.Write(position); // Write the spawner's position to the packet
            packet.Write(hasItem); // Write whether the spawner has an item or not to the packet

            SendTCPData(toClient, packet); // Send the packet to the client
        }
    }

    public static void CoinSpawned(int spawnerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.coinSpawned))
        {
            packet.Write(spawnerId); // Write the spawner's ID to the packet

            SendTCPDataToAll(packet); // Send the packet to all clients
        }
    }

    public static void coinPickedUp(int spawnerId, int byPlayer)
    {
        using (Packet packet = new Packet((int)ServerPackets.coinPickedUp))
        {
            packet.Write(spawnerId); // Write the spawner's ID to the packet
            packet.Write(byPlayer); // Write the ID of the player that picked up the coin to the packet

            SendTCPDataToAll(packet); // Send the packet to all clients
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client
{
    public int id; // Client ID
    public Player player; // Player object
    public TCP tcp; // TCP connection
    public UDP udp; // UDP connection

    public static int dataBufferSize = 4096; // Data buffer size

    public Client(int _clientID)
    {
        id = _clientID; // Set the client ID
        tcp = new TCP(id); // Create a new TCP connection
        udp = new UDP(id); // Create a new UDP connection
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;

        private NetworkStream stream; // Network stream
        private byte[] receiveBuffer;

        private Packet receivedData; // Received data

        public TCP(int _id)
        {
            id = _id; // Set the ID
        }

        public void Connect(TcpClient _socket)
        {
            // Connect to the server
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); // Start receiving data

            ServerSend.Welcome(id, "Welcome to the server!"); // Send a welcome message
        }

        public void SendData(Packet packet)
        {
            try // Try Code
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Start sending data
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {exception}");
            }
        }

        // Checks if the client is still connected, if not, disconnect
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int byteLength = stream.EndRead(asyncResult); // End reading data
                if (byteLength <= 0)
                {
                    // Disconnect
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength]; // Create a new byte array
                Array.Copy(receiveBuffer, data, byteLength); // Copy the received data to the byte array

                // Handle data
                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); // Start receiving data again
            }
            // Catch any exceptions
            catch (Exception exception)
            {
                Debug.Log($"Error receiving TCP data: {exception}");
                // Disconnect
                Server.clients[id].Disconnect();
            }
        }

        public bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data); // Set the bytes of the packet

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt(); // Read the packet length
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength); // Read the bytes of the packet
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt(); // Read the packet ID
                        Server.packetHandlers[packetId](id, packet); // Call the appropriate packet handler
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt(); // Read the packet length
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false; // Add a return statement to handle all code paths
        }

        public void Disconnect()
        {
            socket.Close(); // Close the socket
            stream = null; // Set the stream to null
            receivedData = null; // Set the received data to null
            receiveBuffer = null; // Set the receive buffer to null
            socket = null; // Set the socket to null
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint; // End point
        private int id; // ID

        public UDP(int _id)
        {
            id = _id; // Set the ID
        }

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint; // Set the end point
        }

        public void SendData(Packet newPacket)
        {
            Server.SendUDPData(endPoint, newPacket); // Send data to the specified end point
        }

        public void HandleData(Packet packetData)
        {
            int packetLength = packetData.ReadInt(); // Read the packet length
            byte[] packetBytes = packetData.ReadBytes(packetLength); // Read the packet bytes

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet newPacket = new Packet(packetBytes))
                {
                    int packetId = newPacket.ReadInt(); // Read the packet ID
                    Server.packetHandlers[packetId](id, newPacket); // Call the appropriate packet handler
                }
            });
        }

        public void Disconnect()
        {
            endPoint = null; // Set the end point to null
        }

    }

    public void SendIntoGame(string playerName)
    {
        player = NetworkManager.instance.InstantiatePlayer(); // Instantiate the player object
        player.Initialize(id, playerName);

        foreach (Client client in Server.clients.Values) // Loop through all clients
        {
            if (client.player != null)
            {
                if (client.id != id)
                {
                    ServerSend.SpawnPlayer(id, client.player); // Tell the new player about the existing players
                }
            }
        }

        foreach (Client client in Server.clients.Values) // Loop through all clients
        {
            if (client.player != null)
            {
                ServerSend.SpawnPlayer(client.id, player); // Tell the existing players about the new player
            }
        }

        foreach (CoinSpawner spawner in CoinSpawner.spawners.Values) // Loop through all coin spawners
        {
            ServerSend.CreateCoinSpawner(id, spawner.spawnerId, spawner.transform.position, spawner.hasItem); // Tell the new player about the coin spawners
        }
    }

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected."); // Log the disconnection

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject); // Destroy the player object
            player = null; // Remove the player object
        });

        tcp.Disconnect(); // Disconnect the TCP connection
        udp.Disconnect(); // Disconnect the UDP connection

        ServerSend.playerDisconnected(id); // Send the other player the disconnection message
    }
}

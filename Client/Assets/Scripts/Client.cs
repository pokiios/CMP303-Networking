using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using GameServer;

public class Client : MonoBehaviour
{
    public static Client instance; // Singleton instance of the client
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1"; // IP address for local host
    public int Port = 25252; // Port number for the server
    public int id = 0; // ID of the client

    public TCP tcp; // TCP connection
    public UDP udp; // UDP connection

    private bool isConnected = false; // Is the client connected to the server?

    private delegate void PacketHandler(Packet packet); // Delegate for handling packets
    private static Dictionary<int, PacketHandler> packetHandlers; // Dictionary of packet handlers

    private void Awake()
    {
        // Check if the instance of the client is null
        if (instance == null)
        {
            // Set the instance to this client
            instance = this;
        }
        else
        {
            // If the instance is not null, destroy this client
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        // Connect to the server
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        // Disconnect from the server
        Disconnect();
    }

    public void ConnectToServer()
    {
        // Initialise the client data
        InitialiseClientData();

        // Set the client to connected
        isConnected = true;

        // Connect to the server
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket; // TCP client socket

        private NetworkStream stream; // Network stream
        private byte[] receiveBuffer; // Receive buffer

        private Packet receivedData; // Packet

        public void Connect()
        {
            // creates and initialises new socket.
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.Port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            // Check if socket is connected
            socket.EndConnect(asyncResult);

            // If the socket isn't connected, return
            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream(); // Get the network stream

            receivedData = new Packet(); // Create a new packet

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); // Begin reading the stream
        }

        // Send data to the server
        public void SendData(Packet packet)
        {
            try // Try Code
            {
                // If the socket is not null
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Begin writing the packet to the stream
                }
            }
            catch (Exception exception) // Catch Code
            {
                Debug.Log($"Error sending data to server via TCP: {exception}"); // Log the error
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Get the length of the byte array
                int byteLength = stream.EndRead(asyncResult);

                // If the byte length is less than or equal to 0, return/disconnect
                if (byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                // Create a new byte array and copy the data from the receive buffer
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data)); // Reset the packet and handle the data

                // Handle data
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // Disconnect
                Disconnect();
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

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) // Loops while there is still data to be read
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength); // Read the bytes of the packet
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt(); // Read the packet ID
                        packetHandlers[packetId](packet); // Call the appropriate packet handler
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
            Debug.Log("Handle Data Returned False.");
            return false; // Return false if the data is incomplete
        }

        private void Disconnect()
        {
            instance.Disconnect(); // Disconnect the client
            stream = null; // Reset the network stream
            receivedData = null; // Reset the received data
            receiveBuffer = null; // Reset the receive buffer
            socket = null; // Reset the socket
        }
    }

    public class UDP
    {
        public UdpClient socket; // UDP client socket
        public IPEndPoint endPoint; // End point 

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.Port); // Create a new end point
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort); // Create a new UDP client

            socket.Connect(endPoint); // Connect the socket
            socket.BeginReceive(ReceiveCallback, null); // Begin receiving data

            using (Packet packet = new Packet())
            {
                SendData(packet); // Send a packet
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.id); // Insert the client's ID into the packet
                if (socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null); // Begin sending the packet
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"Error sending data to server via UDP: {exception}"); // Log the error
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try // Try Code
            {
                byte[] data = socket.EndReceive(asyncResult, ref endPoint); // End receiving the data
                socket.BeginReceive(ReceiveCallback, null); // Begin receiving data again

                if (data.Length < 4)
                {
                    // Disconnect
                    instance.Disconnect(); // Disconnect the client
                    return;
                }

                HandleData(data); // Handle the data
            }
            catch
            {
                // Disconnect
                Disconnect(); // Disconnect the client
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data)) // Create a new packet
            {
                int packetLength = packet.ReadInt(); // Read the packet length
                data = packet.ReadBytes(packetLength); // Read the bytes of the packet
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data)) // Create a new packet
                {
                    int packetId = packet.ReadInt(); // Read the packet ID
                    packetHandlers[packetId](packet); // Call the appropriate packet handler
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect(); // Disconnect the client
            endPoint = null; // Reset the end point
            socket = null; // Reset the socket
        }
    }

    private void InitialiseClientData()
    {
        // Initialise the dictionary of packet handlers
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome }, // Welcome packet
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer }, // Spawn Player Packet
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition }, // Player Position Packet
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation }, // Player Rotation Packet
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected }, // Player Disconnected Packet
            { (int)ServerPackets.createCoinSpawner, ClientHandle.CreateCoinSpawner }, // Create Coin Spawner Packet
            { (int)ServerPackets.coinSpawned, ClientHandle.CoinSpawned }, // Coin Spawned Packet
            { (int)ServerPackets.coinPickedUp, ClientHandle.CoinPickedUp } // Coin Picked Up Packet
        };
        Debug.Log("Initialised packets."); // Log that the packets have been initialised
    }

    private void Disconnect()
    {
        // If the client is connected
        if (isConnected)
        {
            // Disconnect from the server
            isConnected = false;
            tcp.socket.Close(); // Close the TCP socket
            udp.socket.Close(); // Close the UDP socket

            Debug.Log("Disconnected from server."); // Log that the client has disconnected
        }
    }
}


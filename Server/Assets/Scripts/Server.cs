using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server
{
    public static int maxPlayers { get; private set; } // Maximum number of players
    public static int port { get; private set; } // Port number

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); // Dictionary of clients

    public delegate void PacketHandler(int fromClient, Packet packet); // Packet handler delegate
    public static Dictionary<int, PacketHandler> packetHandlers; // Dictionary of packet handlers

    private static TcpListener tcpListener; // TCP listener
    private static UdpClient udpListener; // UDP listener

    public static void Start(int _maxPlayers, int _port)
    {
        maxPlayers = _maxPlayers; // Set the maximum number of players
        port = _port; // Set the port number

        Debug.Log("Starting server...");
        InitServerData(); // Initialize server data

        tcpListener = new TcpListener(IPAddress.Any, port); // Create a new TCP listener
        tcpListener.Start(); // Start the TCP listener
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // Start accepting TCP clients

        udpListener = new UdpClient(port); // Create a new UDP listener
        udpListener.BeginReceive(UDPReceiveCallback, null); // Start receiving UDP data

        Debug.Log($"Server started on {port}.");
    }

    private static void TCPConnectCallback(IAsyncResult asyncResult)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(asyncResult); // End accepting TCP clients;
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // Start accepting TCP clients again
        Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}..."); // Log the incoming connection

        for (int i = 1; i <= maxPlayers; i++)
        {
            // Check if the slot is available/empty
            if (clients[i].tcp.socket == null)
            {
                // Connect the client if the slot is available
                clients[i].tcp.Connect(client);
                return;
            }
        }
        // If the server is full, tell the ip/client that the server is full
        Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0); // Create a new IPEndPoint
            byte[] data = udpListener.EndReceive(asyncResult, ref clientEndPoint); // End receiving UDP data

            udpListener.BeginReceive(UDPReceiveCallback, null); // Start receiving UDP data again

            if (data.Length < 4)
            {
                return;
            }

            using (Packet packet = new Packet(data))
            {
                int clientId = packet.ReadInt(); // Read the client's ID

                if (clientId == 0) // This ID should not exist
                {
                    return;
                }

                if (clients[clientId].udp.endPoint == null) // If this is a new connection
                {
                    clients[clientId].udp.Connect(clientEndPoint); // Connect the client
                    return;
                }

                if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString()) // Ensure that the client can't be impersonated
                {
                    clients[clientId].udp.HandleData(packet); // Handle the data
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"Error receiving UDP data: {exception}"); // Log the error
        }
    }

    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null); // Start sending UDP data
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"Error sending data to {clientEndPoint} via UDP: {exception}"); // Log the error
        }
    }

    private static void InitServerData()
    {
        for (int i = 1; i <= maxPlayers; i++)
        {
            clients.Add(i, new Client(i)); // Add a new client
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }, // Add the welcome received packet handler
            { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
        };

        Debug.Log("Initialized packets.");
    }

    public static void Stop()
    {
        tcpListener.Stop(); // Stop the TCP listener
        udpListener.Close(); // Close the UDP listener

        Debug.Log("Server stopped.");
    }
}


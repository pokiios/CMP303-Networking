using System.Collections;
using System.Collections.Generic;
using GameServer;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    // Send TCP packet to server
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength(); // Write the packet length
        Client.instance.tcp.SendData(packet); // Send the packet to the server
    }

    // Send UDP packet to server
    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength(); // Write the packet length
        Client.instance.udp.SendData(packet); // Send the packet to the server
    }

    // Send welcome received packet to server
    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.id); // Write the client's ID
            packet.Write(UIManager.instance.usernameField.text); // Write the client's username

            SendTCPData(packet); // Send the packet to the server
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                packet.Write(_input);
            }
            packet.Write(GameManager.players[Client.instance.id].transform.rotation);
            SendUDPData(packet);
        }
    }

    public static void PlayerPosition(Vector3 position, float time)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerPosition))
        {
            packet.Write(position);
            packet.Write(time);
            SendUDPData(packet);
        }
    }
}


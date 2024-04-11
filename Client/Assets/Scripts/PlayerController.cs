using System.Collections;
using System.Collections.Generic;
using GameServer;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W), // Forward
            Input.GetKey(KeyCode.S), // Backward
            Input.GetKey(KeyCode.A), // Left
            Input.GetKey(KeyCode.D), // Right
            Input.GetKey(KeyCode.Space) // Jump
        };

        ClientSend.PlayerMovement(inputs);
    }
}

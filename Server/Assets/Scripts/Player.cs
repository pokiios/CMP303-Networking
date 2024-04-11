using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public int id; // Player's ID
    public string username; // Player's username
    public CharacterController controller; // Player controller
    public float gravity = -9.81f; // Gravity
    public float moveSpeed = 5f; // Player movement speed (accounting for server ticks)
    public float jumpSpeed = 5f; // Player jump speed
    private float yVelocity = 0; // Player's Y velocity

    float timer = 0;

    public int points = 0; // Player's points
    private bool[] inputs;


    private void Start()
    {
        gravity *= Time.fixedDeltaTime; // Calculate the modified gravity
    }
    public void Initialize(int _id, string _username)
    {
        id = _id; // Set the player's ID
        username = _username; // Set the player's username

        inputs = new bool[5]; // Initialize the inputs array
    }

    public void FixedUpdate()
    {

        Vector2 inputDirection = Vector2.zero; // Reset the input direction

        if (inputs[0]) // W
        {
            inputDirection.y += 1; // Add 1 to the Y value of the input direction
        }
        if (inputs[1]) // S
        {
            inputDirection.y -= 1; // Subtract 1 from the Y value of the input direction
        }
        if (inputs[2]) // D
        {
            inputDirection.x -= 1; // Add 1 to the X value of the input direction
        }
        if (inputs[3]) // A
        {
            inputDirection.x += 1; // Subtract 1 from the X value of the input direction
        }

        Move(inputDirection); // Move the player
    }

    private void Move(Vector2 inputDirection)
    {      
        Vector3 moveDir = transform.right * inputDirection.x + transform.forward * inputDirection.y; // Calculate the movement direction
        moveDir *= moveSpeed; // Apply the movement speed

        controller.Move(moveDir); // Move the controller

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }

        yVelocity += gravity; // Apply gravity

        moveDir.y = yVelocity; // Apply the vertical movement

        ServerSend.PlayerPosition(this); // Send the updated position to other clients
        ServerSend.PlayerRotation(this); // Send the updated rotation to other clients
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs; // Set the inputs
        transform.rotation = _rotation; // Set the rotation
    }

    public bool AttemptPickUp()
    {
        if (points > 10)
        {
            return false;
        }

        points++;
        return true;
    }
}

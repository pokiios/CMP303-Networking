using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 2f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x; // Get the camera's vertical rotation
        horizontalRotation = player.transform.eulerAngles.y; // Get the player's horizontal rotation
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorMode();
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
        Look();
        }

        Debug.DrawRay(transform.position, transform.forward * 10, Color.red); // Draw a red ray in the scene view
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y"); // Invert the vertical axis
        float _mouseHorizontal = Input.GetAxis("Mouse X"); // Get the horizontal axis

        verticalRotation += _mouseVertical * sensitivity; // Update the vertical rotation
        horizontalRotation += _mouseHorizontal * sensitivity; // Update the horizontal rotation

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle); // Clamp the vertical rotation

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0); // Update the camera's rotation
        player.transform.rotation = Quaternion.Euler(0, horizontalRotation, 0); // Update the player's rotation
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible; // Toggle the cursor visibility

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        }
    }
}

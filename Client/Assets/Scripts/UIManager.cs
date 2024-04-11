using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Singleton instance of the UIManager

    public GameObject startMenu; // Start menu UI
    public InputField usernameField; // Username input field

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
    public void ConnectToServer()
    {
        // Player can no longer interact with the start menu.
        startMenu.SetActive(false);

        // Player can no longer interact with the username field.
        usernameField.interactable = false;

        // Connect Client to the server
        Client.instance.ConnectToServer();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> executeOnMainThread = new List<Action>(); // List of actions to execute on the main thread
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>(); // List of actions to execute on the main thread
    private static bool actionToExecuteOnMainThread = false; // Boolean to check if there are actions to execute on the main thread


    private void FixedUpdate()
    {
        UpdateMain(); // Update the main thread
    }
    // Lock object for thread safety
    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Debug.Log("No action to execute on main thread!"); // Log the error
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(_action); // Add the action to the list
            actionToExecuteOnMainThread = true; // Set the boolean to true
        }
    }

    // Update the main thread
    public static void UpdateMain()
    {
        // Check if there are actions to execute on the main thread
        if (actionToExecuteOnMainThread)
        {
            executeCopiedOnMainThread.Clear(); // Clear the list of actions to execute on the main thread

            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread); // Add the actions to the list of actions to execute on the main thread
                executeOnMainThread.Clear(); // Clear the list of actions to execute on the main thread
                actionToExecuteOnMainThread = false; // Set the boolean to false
            }

            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i](); // Execute the actions on the main thread 
            }
        }
    }
}

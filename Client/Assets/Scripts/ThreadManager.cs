using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    // List of actions to execute on the main thread
    private static readonly List<Action> executeOnMainThread = new List<Action>();

    // List of copied actions to execute on the main thread
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();

    // Boolean to check if there is an action to execute on the main thread
    private static bool actionToExecuteOnMainThread = false;

    private void Update()
    {
        UpdateMain();
    }


    // Execute an action on the main thread
    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null) // If the action is null
        {
            Debug.Log("No action to execute on main thread!"); // Log a message to the console
            return;
        }

        lock (executeOnMainThread) // Lock the list of actions to execute on the main thread
        {
            executeOnMainThread.Add(action); // Add the action to the list
            actionToExecuteOnMainThread = true; // Set the boolean to true
        }
    }

    public static void UpdateMain()
    {
        if (actionToExecuteOnMainThread) // If there is an action to execute on the main thread
        {
            executeCopiedOnMainThread.Clear(); // Clear the list of copied actions to execute on the main thread
            lock (executeOnMainThread) // Lock the list of actions to execute on the main thread
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread); // Add all the actions to the list of copied actions
                executeOnMainThread.Clear(); // Clear the list of actions to execute on the main thread
                actionToExecuteOnMainThread = false; // Set the boolean to false
            }

            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i](); // Execute the action
            }
        }
    }
}
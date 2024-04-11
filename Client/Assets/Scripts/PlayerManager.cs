using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id; // Add an ID variable
    public string username; // Add a username variable
    public int points = 0; // Add a points variable
    public MeshRenderer model; // Add a model variable

    float timer = 0;
    public List<Vector3> predictedPositions = new List<Vector3>();
    public List<float> predictedTimes = new List<float>();


    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            predictedPositions.Add(new Vector3(0, 0, 0));
            predictedTimes.Add(i);
        
        }
    }
    public void Initialize(int _id, string _username)
    {
        id = _id; // Set the ID
        username = _username; // Set the username
        points = 0; // Set the points
    }

    public void FixedUpdate()
    {
        timer += Time.deltaTime;

        Vector3 position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        PredictPosition();
        ClientSend.PlayerPosition(position, timer);
    }

    public void PredictPosition()
    {
        float predictedX = -1;
        float predictedZ = -1;

        float speedX, speedZ;

        float distanceX = predictedPositions[0].x - predictedPositions[1].x; // Calculate the distance between the two positions
        float distanceZ = predictedPositions[0].z - predictedPositions[1].z; // Calculate the distance between the two positions

        float time = Convert.ToSingle(predictedTimes[0] - predictedTimes[1]); // Calculate the time between the two positions
        if (time == 0)
        {
            time = Time.deltaTime;
        }
        
        speedX = (float)(distanceX/time); // Calculate the speed of the player in the X direction
        speedZ = (float)(distanceZ/time); // Calculate the speed of the player in the Y direction

        float messageTime = Convert.ToSingle(predictedTimes[0] - predictedTimes[1]);

        if (messageTime == 0)
        {
            messageTime *= -1;
        }

        float displacementX = (float)speedX * messageTime;
        float displacementY = (float)speedZ * messageTime;

        transform.position = new Vector3(predictedX, 1, predictedZ);

        predictedPositions.RemoveAt(0);
        predictedTimes.RemoveAt(0);
    }
}

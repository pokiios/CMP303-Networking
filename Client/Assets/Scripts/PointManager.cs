using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public GameObject start;

    void Start()
    {
        pointsText.enabled = false;
    }

    void FixedUpdate()
    {
        // check if UI startmenu is active
        if (!start.activeSelf)
        {
            pointsText.enabled = true;
        }


    }
}

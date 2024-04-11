using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(transform.position, transform.right, 90 * Time.deltaTime);
    }
}

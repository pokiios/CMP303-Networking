using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public MeshRenderer itemModel;
    private Vector3 basePosition;

    public void Initialize(int _spawnerId, bool _hasItem)
    {
        spawnerId = _spawnerId;
        hasItem = _hasItem;
        itemModel.enabled = hasItem;

        basePosition = transform.position;
    }

    public void CoinSpawned()
    {
        hasItem = true;
        itemModel.enabled = true;
    }

    public void CoinPickedUp()
    {
        hasItem = false;
        itemModel.enabled = false;
    }
}

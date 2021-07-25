using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;

    void Start()
    {
        Instantiate(SelectACarPrefab(), transform);
    }

    private GameObject SelectACarPrefab()
    {
        var randomINdex = Random.Range(0, carPrefabs.Length);
        return carPrefabs[randomINdex];
    }

}

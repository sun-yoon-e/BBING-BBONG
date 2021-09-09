using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTerrainAndRoad : MonoBehaviour
{
    MeshGenerator map;
    RoadGenerator road;

    private void Start()
    {
        map = GameObject.Find("Terrain Generator").GetComponent<MeshGenerator>();
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        map.UpdateMesh();
        road.UpdateMesh();
    }
}

using System;
using UnityEngine.AI;
using UnityEngine;

public class BakeNavmesh : MonoBehaviour
{
    private RoadGenerator road;
    NavMeshSurface navMeshSurface;
    private void Start()
    {
        //navMeshSurface = GetComponent<NavMeshSurface>();

        //if (navMeshSurface == true)
        //    navMeshSurface.BuildNavMesh();

        //for (int i = 0; i < navMeshSurfaces.Length; ++i)
        //{
        //    if (navMeshSurfaces[i] == true)
        //        navMeshSurfaces[i].BuildNavMesh();
        //}
    }

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        road.OnRoadReady += GenerateNavMesh;
    }

    void GenerateNavMesh(object sender, EventArgs args)
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == true)
            navMeshSurface.BuildNavMesh();
    }
}

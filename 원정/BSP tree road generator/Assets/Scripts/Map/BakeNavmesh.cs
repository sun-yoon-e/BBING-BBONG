using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class BakeNavmesh : MonoBehaviour
{
    //MeshGenerator map;

    //void Start()
    //{
    //    map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();
    //    map.BakeNavMesh();
    //}

    [SerializeField]
    NavMeshSurface[] navMeshSurfaces;


    private void Start()
    {
        for (int i = 0; i < navMeshSurfaces.Length; ++i)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}

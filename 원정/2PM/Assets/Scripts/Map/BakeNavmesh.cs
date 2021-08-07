using UnityEngine.AI;
using UnityEngine;

public class BakeNavmesh : MonoBehaviour
{
    NavMeshSurface navMeshSurface;
    private void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == true)
            navMeshSurface.BuildNavMesh();

        //for (int i = 0; i < navMeshSurfaces.Length; ++i)
        //{
        //    if (navMeshSurfaces[i] == true)
        //        navMeshSurfaces[i].BuildNavMesh();
        //}
    }
}

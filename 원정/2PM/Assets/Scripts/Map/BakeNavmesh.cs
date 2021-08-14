using UnityEngine.AI;
using UnityEngine;

public class BakeNavmesh : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;
    private void Start()
    {
        for (int i = 0; i < navMeshSurfaces.Length; ++i)
        {
            if (navMeshSurfaces[i] == true)
                navMeshSurfaces[i].BuildNavMesh();
        }
    }
}

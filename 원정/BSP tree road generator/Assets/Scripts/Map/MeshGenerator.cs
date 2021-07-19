using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    public Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;

    public float mapHeight;
    public Vector3 mapPosition;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mapPosition = new Vector3(0.0f, 0.0f, 0.0f);

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
    }

    void Start()
    {
        CreateShape();
        CreateTriangle();
        UpdateMesh();
    }

    public void CreateShape()
    {
        float y;

        //for (int i = 0, z = 0; z <= zSize; ++z)
        //{
        //    for (int x = 0; x < xSize / 2; ++x, ++i)
        //    {
        //        y = Mathf.PerlinNoise(x * .3f, z * .3f) * mapHeight;

        //        vertices[i] = new Vector3(x * 5, y, z * 5);
        //    }
        //    for (int x = xSize / 2; x <= xSize; ++x, ++i)
        //    {
        //        y = Mathf.PerlinNoise(x * .2f, z * .2f) * mapHeight;

        //        vertices[i] = new Vector3(x * 5, y, z * 5);
        //    }
        //}

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x, ++i)
            {
                y = Mathf.PerlinNoise(x * .3f, z * .3f) * mapHeight;

                vertices[i] = new Vector3(x * 5, y, z * 5);
            }
        }
    }

    public void CreateTriangle()
    {
        triangles = new int[xSize * zSize * 6];

        int v = 0;
        int t = 0;

        for (int z = 0; z < zSize; ++z)
        {
            for (int x = 0; x < xSize; ++x)
            {
                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + xSize + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + xSize + 1;
                triangles[t + 5] = v + xSize + 2;

                v++;
                t += 6;
            }
            v++;
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        MeshCollider col = GetComponent<MeshCollider>();
        if (col == null)
            col = gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;
    }
    //public void BakeNavMesh()
    //{
    //    NavMeshSurface[] surfaces = gameObject.GetComponentsInChildren<NavMeshSurface>();

    //    foreach (var s in surfaces)
    //    {
    //        s.RemoveData();
    //        s.BuildNavMesh();
    //    }
    //}
}

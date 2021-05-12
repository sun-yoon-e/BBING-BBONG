using UnityEngine;

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

    public int[] buildingPlace;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mapPosition = new Vector3(0.0f, 0.0f, 0.0f);

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        buildingPlace = new int[(xSize + 1) * (zSize + 1)];
        buildingPlace = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>().isBuildingPlace;
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

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x, ++i)
            {
                y = Mathf.PerlinNoise(x * .3f, z * .3f) * mapHeight;

                vertices[i] = new Vector3(x * 10, y, z * 10);
            }
        }

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x, ++i)
            {
                //if(buildingPlace[i] != 0)
                //{
                //    y = 5f;
                //    vertices[i].y = y;
                //}

                //if (i > 0 && (buildingPlace[i] == 1 || buildingPlace[i] == 2))
                //{
                //    y = vertices[i].y;
                //    vertices[i - 1].y = y;
                //    vertices[i + 1].y = y;
                //    //if (i > xSize + 1)
                //    //{
                //    //    vertices[i - xSize + 1].y = y;
                //    //    vertices[i - xSize].y = y;
                //    //    vertices[i - xSize - 1].y = y;
                //    //}
                //    vertices[i + xSize + 1].y = y;
                //    vertices[i + xSize].y = y;
                //    vertices[i + xSize - 1].y = y;
                //}
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

    void UpdateMesh()
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
}

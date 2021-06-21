using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

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
        
    }

    void Start()
    {
        gameClient.OnMeshChanged += SetMeshEvent;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mapPosition = new Vector3(0.0f, 0.0f, 0.0f);

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        buildingPlace = new int[(xSize + 1) * (zSize + 1)];
        buildingPlace = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>().isBuildingPlace;

        gameClient.GetMesh();
    }

    public void SetMeshEvent(object sender, MeshEventArgs args)
    {
        if (!args.ready)
        {
            Debug.Log("SetMeshEvent() 처음!!");
            CreateShape();
            CreateTriangle();
            gameClient.SetMesh(vertices, triangles);
        }
        else
        {
            Debug.Log("SetMeshEvent() 업뎃");
            vertices = args.vertices;
            triangles = args.triangles;

            UpdateMesh();
        }
    }

    public void CreateShape()
    {
        float y;

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x, ++i)
            {
                y = Mathf.PerlinNoise(x * .3f, z * .3f) * mapHeight;
                //y = 1;

                vertices[i] = new Vector3(x * 10, y, z * 10);
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
}

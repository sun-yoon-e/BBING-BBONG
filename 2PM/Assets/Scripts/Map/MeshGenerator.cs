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

    public float perlinNoiseNum;

    private void Awake()
    {
        gameClient.OnMeshChanged += SetMeshEvent;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        gameClient.GetMesh();
    }

    void Start()
    {
        //gameClient.OnMeshChanged += SetMeshEvent;
        
        //mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;
        //vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //gameClient.GetMesh();
    }

    private bool IsMapGenerated = false;
    
    public void SetMeshEvent(object sender, MeshEventArgs args)
    {
        if (!args.ready)
        {
            //Debug.Log("SetMeshEvent() 처음!!");
            CreateShape();
            CreateTriangle();
            //UpdateMesh();
            //IsMapGenerated = true;
            gameClient.SetMesh(vertices, triangles);
        }
        else/* if(!IsMapGenerated)
*/        {
            //Debug.Log("SetMeshEvent() 업뎃");
            vertices = args.vertices;
            triangles = args.triangles;

            //Debug.Log("Vertex size : " + vertices.Length);
            //Debug.Log("triangle size : " + triangles.Length);

            //Draw에서 호출
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
                y = Mathf.PerlinNoise(x * perlinNoiseNum, z * perlinNoiseNum) * mapHeight;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadGenerator2 : MeshGenerator
{
    Mesh roadMesh;

    Vector3[] roadVertices;
    int[] roadTriangles;

    public int roadXSize = 1;
    public int roadZSize = 1;

    int v = 0;
    int t = 0;

    public SimpleVisualizer visualizer;
    public List<Vector3> roadPosition = new List<Vector3>();

    MeshGenerator map;

    private void Awake()
    {
        roadMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = roadMesh;
    }

    private void Start()
    {
        //visualizer = GameObject.Find("SimpleVisualizer").GetComponent<SimpleVisualizer>();
        //roadPosition = visualizer.positions;
        ////Debug.Log(roadPosition[1].x);

        map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();

        SetVertices();
        CreateRoad();

        UpdateLine();
    }

    void SetVertices()
    {
        roadVertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector3 position;

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                position = new Vector3(x, mapHeight, z);

                // 레이캐스팅으로 지형의 y좌표 얻기
                RaycastHit hit;

                if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "Terrian")
                    {
                        position = hit.point;
                        roadVertices[i] = position;
                        ++i;

                        Debug.Log("Hit!");
                    }
                   
                }
            }
        }
    }
    void CreateRoad()
    {
        roadTriangles = new int[xSize * zSize * 6];
        v = Random.Range(xSize / 3, xSize - xSize / 3);

        foreach (var positions in roadPosition)
        {
            Vector3Int position = Vector3Int.RoundToInt(positions);
        }
    }

    void CreateRect(int vertex)
    {
        roadTriangles[t + 0] = vertex + 0;
        roadTriangles[t + 1] = vertex + xSize + 1;
        roadTriangles[t + 2] = vertex + 1;
        roadTriangles[t + 3] = vertex + 1;
        roadTriangles[t + 4] = vertex + xSize + 1;
        roadTriangles[t + 5] = vertex + xSize + 2;

        t += 6;
    }

    void UpdateLine()
    {
        roadMesh.Clear();

        roadMesh.vertices = roadVertices;
        roadMesh.triangles = roadTriangles;

        roadMesh.RecalculateNormals();
    }
}

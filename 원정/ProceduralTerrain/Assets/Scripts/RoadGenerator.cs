using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum RoadGenerate {FAIL, MINUS90, PLUS90}

public class RoadGenerator : MeshGenerator
{
    Mesh roadMesh;

    Vector3[] roadVertices;
    int[] roadTriangles;

    public int roadXSize = 1;
    public int roadZSize = 1;

    int v = 0;
    int t = 0;

    int total = 100;

    SimpleVisualizer visualizer;
    List<Vector3> roadPosition;

    private void Start()
    {
        roadMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = roadMesh;

        visualizer = GameObject.Find("SimpleVisualizer").GetComponent<SimpleVisualizer>();
        roadPosition = visualizer.positions;

        SetVertices();
        CreateRoad();

        UpdateLine();
    }

    int RandomRoad()
    {
        int[] weight = new int[3] { 94, 3, 3 };
        int selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

        int num = 0;
        for (int i = 0; i < 3; ++i)
        {
            num += weight[i];
            if (selectNum <= num)
            {
                return i;
            }
        }
        return 0;
    }
    void SetVertices()
    {
        roadVertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float y = Mathf.PerlinNoise(x * .2f, z * .2f) * mapHeight * mapSize;
                roadVertices[i] = new Vector3(x * mapSize, y + 1f, z * mapSize);
                ++i;
            }
        }
    }
    void CreateRoad()
    {
        roadTriangles = new int[xSize * zSize * 6];
        v = Random.Range(xSize / 3, xSize - xSize / 3);

        int xPos = v;
        int addRoad;

        for (int z = 0; z < zSize; ++z)
        {
            CreateRect(v);

            addRoad = RandomRoad();
            if (addRoad == 1)
            {
                //CreateRect();

                int j = 0;
                for (int i = v - xPos - 1; i < v - 1; ++i)
                {
                    CreateRect(v - xPos + j);
                    ++j;

                    //addRoad = RandomRoad();
                }
            }
            else if (addRoad == 2)
            {
                for (int i = 0; i < xSize - xPos; ++i)
                    CreateRect(v + 1 + i);
            }

            v += xSize + roadXSize;
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

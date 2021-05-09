using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMesh
{
    public Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;

    public float mapHeight;
    public Vector3 mapPosition;

    public MapMesh()
    {
        mesh = new Mesh();
        xSize = 100;
        zSize = 100;
        mapHeight = 5.0f;
        mapPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float y = Mathf.PerlinNoise(x * .2f, z * .2f) * mapHeight;
                vertices[i] = new Vector3(x * 10, y, z * 10);
                ++i;
            }
        }

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
    }
}

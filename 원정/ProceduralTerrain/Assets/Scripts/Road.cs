using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 기본 아이디어는 선으로 시작한다는 것입니다.
 * 이 선은 앞으로 랜덤하게 이동하거나, 왼쪽 또는 오른쪽으로 회전하거나, 분기하여 다른 선으로 이동할 수 있습니다.
 * 이 시스템은 계속해서 반복되며 결국에는 상당히 멋진 도로망을 형성합니다.
 * 다음 단계는 선을 그리드에 맞는 균일한 간격의 좌표로 변환하는 것입니다.
 * 그런 다음, 각 좌표를 인접한 4개의 그리드 공간과 비교하고,
 * 이 정보를 사용하여 그곳에 배치할 도로의 유형(직선, 모서리 등)을 선택한다.
 * 길 옆에 보이는 하얀 사각형이 산란주택의 시작이다.
 * 각각의 흰색 사각형은 집이나 건물을 위한 잠재적인 산란 위치를 나타냅니다.
 */

/*
 * 1. 선 하나를 생성한다
 * 2. 생성한 선 중간(랜덤)에 양쪽으로 뻗어나가는 선 2개를 생성
 * 3. 반복
 */
public class Road : MeshGenerator
{
    Mesh roadMesh;

    Vector3[] roadVertices;
    int[] roadTriangles;

    public int roadXSize = 1;
    public int roadZSize = 1;

    int v = 0;
    int t = 0;

    void Start()
    {
        roadMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = roadMesh;

        GenerateRoad();
        CreateRootRoad();

        UpdateLine();
    }

    void GenerateRoad()
    {
        roadVertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float y = Mathf.PerlinNoise(x * .2f, z * .2f) * mapHeight;
                roadVertices[i] = new Vector3(x, y + 1f, z);
                ++i;
            }
        }
    }

    int Root = 0;

    int leftDown = 0;
    int rightDown = 0;
    int leftUp = 0;
    int rightUp = 0;
    
    int leftZ = 0;
    int rightZ = 0;

    void CreateRootRoad()
    {
        roadTriangles = new int[xSize * zSize * 6];

        v = Random.Range(xSize / 3, xSize - xSize / 3);
        int rootV = v;
        for (int z = 0; z < zSize; ++z)
        {
            roadTriangles[t + 0] = v + 0;
            roadTriangles[t + 1] = v + xSize + 1;
            roadTriangles[t + 2] = v + 1;
            roadTriangles[t + 3] = v + 1;
            roadTriangles[t + 4] = v + xSize + 1;
            roadTriangles[t + 5] = v + xSize + 2;

            t += 6;
            v += xSize + roadXSize;
        }

        CreateRoad(rootV, 0, xSize, 0, zSize);
        CreateRoad(leftDown, 0, Root, 0, leftZ);
        //CreateRoad(leftUp, 0, Root, leftZ, zSize);
    }

    int num = 0;
    void CreateRoad(int root, int minXSize, int maxXSize, int minZSize, int maxZSize)
    {
        if (num % 3 == 0)
            Root = root;
        ++num;

        v = 0;

        int xRoot = root;
        int zRoot = 0;
        while(xRoot > xSize)
        {
            xRoot /= xSize;
            ++zRoot;
        }

        // 가로 왼쪽
        leftZ = Random.Range(minZSize, maxZSize);
        int roadZ = leftZ * (zSize + 1);
        for (int x = minXSize; x < xRoot; ++x)
        {
            roadTriangles[t + 0] = roadZ + 0 + v;
            roadTriangles[t + 1] = roadZ + zSize + 1 + v;
            roadTriangles[t + 2] = roadZ + 1 + v;
            roadTriangles[t + 3] = roadZ + 1 + v;
            roadTriangles[t + 4] = roadZ + zSize + 1 + v;
            roadTriangles[t + 5] = roadZ + zSize + 2 + v;

            v++;
            t += 6;
        }

        //가로 오른쪽
        rightZ = Random.Range(minZSize, maxZSize);
        roadZ = rightZ * (zSize + 1);
        for (int x = xRoot; x < maxXSize; ++x)
        {
            roadTriangles[t + 0] = roadZ + 0 + v;
            roadTriangles[t + 1] = roadZ + zSize + 1 + v;
            roadTriangles[t + 2] = roadZ + 1 + v;
            roadTriangles[t + 3] = roadZ + 1 + v;
            roadTriangles[t + 4] = roadZ + zSize + 1 + v;
            roadTriangles[t + 5] = roadZ + zSize + 2 + v;

            v++;
            t += 6;
        }

        //왼 아래
        v = Random.Range(minXSize, xRoot);
        leftDown = v;
        v += zSize * zRoot;
        for (int z = minZSize; z < leftZ; ++z)
        {
            roadTriangles[t + 0] = v + 0;
            roadTriangles[t + 1] = v + xSize + 1;
            roadTriangles[t + 2] = v + 1;
            roadTriangles[t + 3] = v + 1;
            roadTriangles[t + 4] = v + xSize + 1;
            roadTriangles[t + 5] = v + xSize + 2;

            t += 6;
            v += xSize + 1 ;
        }

        // 왼 위
        v = Random.Range(minXSize, xRoot);
        v += (leftZ + 1) * (zSize + 1);
        leftUp = v;
        for (int z = leftZ; z < maxZSize - 1; ++z)
        {
            roadTriangles[t + 0] = v + 0;
            roadTriangles[t + 1] = v + xSize + 1;
            roadTriangles[t + 2] = v + 1;
            roadTriangles[t + 3] = v + 1;
            roadTriangles[t + 4] = v + xSize + 1;
            roadTriangles[t + 5] = v + xSize + 2;

            t += 6;
            v += xSize + 1;
        }

        // 오른 아래
        v = Random.Range(xRoot, maxXSize);
        rightDown = v;
        for (int z = minZSize; z < rightZ; ++z)
        {
            roadTriangles[t + 0] = v + 0;
            roadTriangles[t + 1] = v + xSize + 1;
            roadTriangles[t + 2] = v + 1;
            roadTriangles[t + 3] = v + 1;
            roadTriangles[t + 4] = v + xSize + 1;
            roadTriangles[t + 5] = v + xSize + 2;

            t += 6;
            v += xSize + 1;
        }

        // 오른 위
        v = Random.Range(xRoot, maxXSize) + (rightZ + 1) * (zSize + 1);
        rightUp = v;
        for (int z = rightZ; z < maxZSize - 1; ++z)
        {
            roadTriangles[t + 0] = v + 0;
            roadTriangles[t + 1] = v + xSize + 1;
            roadTriangles[t + 2] = v + 1;
            roadTriangles[t + 3] = v + 1;
            roadTriangles[t + 4] = v + xSize + 1;
            roadTriangles[t + 5] = v + xSize + 2;

            t += 6;
            v += xSize + 1;
        }
    }

    void UpdateLine()
    {
        roadMesh.Clear();

        roadMesh.vertices = roadVertices;
        roadMesh.triangles = roadTriangles;

        roadMesh.RecalculateNormals();
    }
}
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RoadGenerator : MonoBehaviour
{
    Mesh mesh;

    public Vector3[] vertices;
    public int[] triangles;

    public int xSize;
    public int zSize;

    public float mapHeight = 2.0f;
    public Vector3 mapPosition;

    int t = 0;

    int xSplit = 0;
    int[] upXSplit = { };
    int[] downXSplit = { };
    int[] leftZSplit = { };
    int[] rightZSplit = { };

    public bool[] isRoad = { };
    public bool[] isBuildingPlace = { };

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mapPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void Start()
    {
        CreateShape();
        CreateTriangle();
        UpdateMesh();
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
    }

    public void CreateTriangle()
    {
        rightZSplit = new int[100];
        leftZSplit = new int[100];
        upXSplit = new int[100];
        downXSplit = new int[100];

        triangles = new int[xSize * zSize * 6];
        isBuildingPlace = new bool[(xSize + 1) * (zSize + 1)];
        isRoad = new bool[(xSize + 1) * (zSize + 1)];

        splitX(40, 0, zSize);

        leftSplitZ(40, zSize - 40, 0, xSplit, 0);
        {
            downSplitX(20, xSplit - 20, 0, leftZSplit[0], 0);
            {
                rightSplitZ(0, leftZSplit[0], downXSplit[0], xSplit, 0);
                {
                    downSplitX(downXSplit[0], xSplit, 0, rightZSplit[0], 1);
                    {
                        leftSplitZ(0, rightZSplit[0], downXSplit[0], downXSplit[1], 1);
                        rightSplitZ(0, rightZSplit[0], downXSplit[1], xSplit, 1);
                    }
                    upSplitX(downXSplit[0], xSplit, rightZSplit[0], leftZSplit[0], 0);
                    {
                        leftSplitZ(rightZSplit[0], leftZSplit[0], downXSplit[0], upXSplit[0], 2);
                        rightSplitZ(rightZSplit[0], leftZSplit[0], upXSplit[0], xSplit, 2);
                    }
                }


                leftSplitZ(0, leftZSplit[0], 0, downXSplit[0], 10);
                {
                    downSplitX(0, downXSplit[0], 0, leftZSplit[10], 10);
                    {
                        leftSplitZ(0, leftZSplit[10], 0, downXSplit[10], 11);
                        rightSplitZ(0, leftZSplit[10], downXSplit[10], downXSplit[0], 10);
                    }
                    upSplitX(0, downXSplit[0], leftZSplit[10], leftZSplit[0], 10);
                    {
                        leftSplitZ(leftZSplit[10], leftZSplit[0], 0, upXSplit[10], 12);
                        rightSplitZ(leftZSplit[10], leftZSplit[0], upXSplit[10], downXSplit[0], 11);
                    }
                }
            }

            upSplitX(20, xSplit - 20, leftZSplit[0], zSize, 20);
            {
                leftSplitZ(leftZSplit[0], zSize, 0, upXSplit[20], 20);
                {
                    downSplitX(0, upXSplit[20], leftZSplit[0], leftZSplit[20], 20);
                    {
                        leftSplitZ(leftZSplit[0], leftZSplit[20], 0, downXSplit[20], 21);
                        rightSplitZ(leftZSplit[0], leftZSplit[20], downXSplit[20], upXSplit[20], 21);
                    }
                    upSplitX(0, upXSplit[20], leftZSplit[20], zSize, 21);
                    {
                        leftSplitZ(leftZSplit[20], zSize, 0, upXSplit[21], 22);
                        rightSplitZ(leftZSplit[20], zSize, upXSplit[21], upXSplit[20], 22);
                    }
                }

                rightSplitZ(leftZSplit[0], zSize, upXSplit[20], xSplit, 30);
                {
                    downSplitX(upXSplit[20], xSplit, leftZSplit[0], rightZSplit[30], 30);
                    {
                        leftSplitZ(leftZSplit[0], rightZSplit[30], upXSplit[20], downXSplit[30], 30);
                        rightSplitZ(leftZSplit[0], rightZSplit[30], downXSplit[30], xSplit, 31);
                    }
                    upSplitX(upXSplit[20], xSplit, rightZSplit[30], zSize, 30);
                    {
                        leftSplitZ(rightZSplit[30], zSize, upXSplit[20], upXSplit[30], 31);
                        rightSplitZ(rightZSplit[30], zSize, upXSplit[30], xSplit, 32);
                    }
                }
            }
        }

        rightSplitZ(40, zSize - 40, xSplit, xSize, 50);
        {
            downSplitX(xSplit + 20, xSize - 20, 0, rightZSplit[50], 50);
            {
                rightSplitZ(0, rightZSplit[50], downXSplit[50], xSize, 51);
                {
                    downSplitX(downXSplit[50], xSize, 0, rightZSplit[51], 51);
                    {
                        rightSplitZ(0, rightZSplit[51], downXSplit[51], xSize, 52);
                        leftSplitZ(0, rightZSplit[51], downXSplit[50], downXSplit[51], 51);
                    }
                    upSplitX(downXSplit[50], xSize, rightZSplit[51], rightZSplit[50], 50);
                    {
                        rightSplitZ(rightZSplit[51], rightZSplit[50], upXSplit[50], xSize, 53);
                        leftSplitZ(rightZSplit[51], rightZSplit[50], downXSplit[50], upXSplit[50], 52);
                    }
                }

                leftSplitZ(0, rightZSplit[50], xSplit, downXSplit[50], 60);
                {
                    downSplitX(xSplit, downXSplit[50], 0, leftZSplit[60], 60);
                    {
                        rightSplitZ(0, leftZSplit[60], downXSplit[60], downXSplit[50], 60);
                        leftSplitZ(0, leftZSplit[60], xSplit, downXSplit[60], 61);
                    }
                    upSplitX(xSplit, downXSplit[50], leftZSplit[60], rightZSplit[50], 60);
                    {
                        rightSplitZ(leftZSplit[60], rightZSplit[50], upXSplit[60], downXSplit[50], 62);
                        leftSplitZ(leftZSplit[60], rightZSplit[50], xSplit, upXSplit[60], 62);
                    }
                }
            }

            upSplitX(xSplit + 20, xSize - 20, rightZSplit[50], zSize, 70);
            {
                rightSplitZ(rightZSplit[50], zSize, upXSplit[70], xSize, 70);
                {
                    upSplitX(upXSplit[70], xSize, rightZSplit[70], zSize, 71);
                    {
                        rightSplitZ(rightZSplit[70], zSize, upXSplit[71], xSize, 71);
                        leftSplitZ(rightZSplit[70], zSize, upXSplit[70], upXSplit[71], 70);
                    }
                    downSplitX(upXSplit[70], xSize, rightZSplit[50], rightZSplit[70], 70);
                    {
                        rightSplitZ(rightZSplit[50], rightZSplit[70], downXSplit[70], xSize, 72);
                        leftSplitZ(rightZSplit[50], rightZSplit[70], upXSplit[70], downXSplit[70], 71);
                    }
                }

                leftSplitZ(rightZSplit[50], zSize, xSplit, upXSplit[70], 80);
                {
                    upSplitX(xSplit, upXSplit[70], leftZSplit[80], zSize, 80);
                    {
                        rightSplitZ(leftZSplit[80], zSize, upXSplit[80], upXSplit[70], 80);
                        leftSplitZ(leftZSplit[80], zSize, xSplit, upXSplit[80], 81);
                    }
                    downSplitX(xSplit, upXSplit[70], rightZSplit[50], leftZSplit[80], 80);
                    {
                        rightSplitZ(rightZSplit[50], leftZSplit[80], downXSplit[80], upXSplit[70], 81);
                        leftSplitZ(rightZSplit[50], leftZSplit[80], xSplit, downXSplit[80], 82);
                    }
                }
            }
        }

        for (int i = 0; i < isRoad.Length; ++i)
        {
            if (isRoad[i] == true)
            {
                isBuildingPlace[i] = false;
            }
        }
    }

    void splitX(int minX, int minZ, int maxZ)
    {
        xSplit = Random.Range(minX, xSize - minX);

        int v = xSplit;

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;

            isRoad[v] = true;
            isRoad[v + 1] = true;

            if (z > minZ)
            {
                isBuildingPlace[v + 2] = true;
                isBuildingPlace[v - 1] = true;
            }
            v += xSize + 1;
            t += 6;
        }
    }
    void upSplitX(int minX, int maxX, int minZ, int maxZ, int num)
    {
        upXSplit[num] = Random.Range(minX + 4, maxX - 4);

        int v = ((xSize + 1) * minZ) + upXSplit[num];

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;
            isRoad[v] = true;
            isRoad[v + 1] = true;

            if (z > minZ + 1)
            {
                isBuildingPlace[v + 2] = true;
                isBuildingPlace[v - 1] = true;
            }
            v += xSize + 1;
            t += 6;
        }
    }
    void downSplitX(int minX, int maxX, int minZ, int maxZ, int num)
    {
        downXSplit[num] = Random.Range(minX + 4, maxX - 4);

        int v;
        if (num - 1 < 0)
            v = downXSplit[num];
        else
            v = ((xSize + 1) * minZ) + downXSplit[num];

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;
            isRoad[v] = true;
            isRoad[v + 1] = true;

            if (z > minZ + 1)
            {
                isBuildingPlace[v + 2] = true;
                isBuildingPlace[v - 1] = true;
            }

            v += xSize + 1;
            t += 6;
        }
    }
    void leftSplitZ(int minZ, int maxZ, int minX, int maxX, int num)
    {
        leftZSplit[num] = Random.Range(minZ + 4, maxZ - 4);

        int v = (xSize + 1) * (leftZSplit[num]) + minX;

        for (int x = minX + 1; x < maxX + 1; ++x)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;
            isRoad[v] = true;
            isRoad[v + xSize + 1] = true;

            if (minX + 1 < x && x < maxX - 1)
            {
                isBuildingPlace[v + (xSize * 2) + 3] = true;
                isBuildingPlace[v - xSize] = true;
            }
            v++;
            t += 6;
        }
    }
    void rightSplitZ(int minZ, int maxZ, int minX, int maxX, int num)
    {
        rightZSplit[num] = Random.Range(minZ + 4, maxZ - 4);

        int v = (xSize + 1) * (rightZSplit[num]) + minX;

        for (int x = minX + 1; x < maxX + 1; ++x)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;
            isRoad[v] = true;
            isRoad[v + xSize + 1] = true;

            if (minX + 1 < x && x < maxX - 1)
            {
                isBuildingPlace[v + (xSize * 2) + 3] = true;
                isBuildingPlace[v - xSize] = true;
            }
            v++;
            t += 6;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}

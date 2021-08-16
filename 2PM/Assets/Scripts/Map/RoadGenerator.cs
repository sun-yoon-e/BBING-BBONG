using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class RoadGenerator : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;
    
    Mesh mesh;
    MeshGenerator map;

    public int xSize;
    public int zSize;

    int xSplit;
    int[] upXSplit;
    int[] downXSplit;
    int[] leftZSplit;
    int[] rightZSplit;

    public Vector3[] passibleItemPlace;
    public int middleRoadNum;
    public int[] buildingState;

    public bool[] isRoad;
    //public bool[] isDestination;
    public bool[] isItemPlace;
    public bool[] isWayPointPlace;
    public bool[] isObjectPlace;

    public GameObject wayPointPrefab;
    public GameObject[] wayPoint;
    public int wayPointNum;

    int t;
    public Vector3[] vertices;
    public int[] triangles;

    public int roadInterval;
    
    public event EventHandler OnRoadReady;
    public event EventHandler OnRoadReady2;
    public event EventHandler OnRoadReady3;
    public event EventHandler OnRoadReady4;
    public bool isRoadReady = false;

    private void Awake()
    {
        gameClient.OnRoadChanged += SetRoadEvent;
        
        map = GameObject.Find("Terrain Generator").GetComponent<MeshGenerator>();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Start()
    {
        Debug.Log("RoadGenerator Start!!");
        vertices = new Vector3[map.vertices.Length];

        wayPoint = new GameObject[500];
        wayPointNum = 0;
        isWayPointPlace = new bool[vertices.Length];

        isObjectPlace = new bool[vertices.Length];

        xSize = map.xSize;
        zSize = map.zSize;

        gameClient.GetRoad();
        //CreateShape();
        //CreateTriangle();
        //UpdateMesh();
    }

    public void SetRoadEvent(object sender, RoadEventArgs args)
    {
        if (!args.ready)
        {
            Debug.Log("SetRoadEvent() 처음!!");
            CreateShape();
            CreateTriangle();
            gameClient.SetRoad(vertices, triangles, isRoad, buildingState);
        }
        else
        {
            Debug.Log("SetRoadEvent() 업뎃");
            vertices = args.vertices;
            triangles = args.triangles;
            isRoad = args.isRoad;
            buildingState = args.isBuildingPlace;

            UpdateMesh();
            isRoadReady = true;

            if (OnRoadReady != null)
            {
                OnRoadReady(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("OnRoadReady() is null");
            }

            if (OnRoadReady2 != null)
            {
                OnRoadReady2(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("OnRoadReady2() is null");
            }

            if (OnRoadReady3 != null)
            {
                OnRoadReady3(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("OnRoadReady3() is null");
            }

            if (OnRoadReady4 != null)
            {
                OnRoadReady4(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("OnRoadReady4() is null");
            }
        }
    }

    public void CreateShape()
    {
        vertices = map.vertices;
    }

    public void CreateTriangle()
    {
        rightZSplit = new int[100];
        leftZSplit = new int[100];
        upXSplit = new int[100];
        downXSplit = new int[100];

        triangles = new int[xSize * zSize * 6];
        buildingState = new int[vertices.Length];
        isRoad = new bool[vertices.Length];
        //isDestination = new bool[vertices.Length];
        isItemPlace = new bool[vertices.Length];

        passibleItemPlace = new Vector3[vertices.Length];


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

        makeNotBuildingPlace();
    }

    void splitX(int minX, int minZ, int maxZ)
    {
        xSplit = xSize / 2;
        int v = xSplit;

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;

            triangles[t + 6] = v + 1;
            triangles[t + 7] = v + xSize + 2;
            triangles[t + 8] = v + 2;
            triangles[t + 9] = v + 2;
            triangles[t + 10] = v + xSize + 2;
            triangles[t + 11] = v + xSize + 3;

            isRoad[v] = true;
            isRoad[v + 1] = true;
            isRoad[v + 2] = true;

            isItemPlace[v + 1] = true;

            if (z > minZ)
            {
                buildingState[v + 5] = (int)buildingDirection.LEFT;
                buildingState[v - 3] = (int)buildingDirection.RIGHT;
            }
            v += xSize + 1;
            t += 12;
        }
    }
    void upSplitX(int minX, int maxX, int minZ, int maxZ, int num)
    {
        upXSplit[num] = Random.Range(minX + roadInterval, maxX - roadInterval);

        int v = ((xSize + 1) * minZ) + upXSplit[num];
        bool isWayPoint = false;

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;

            triangles[t + 6] = v + 1;
            triangles[t + 7] = v + xSize + 2;
            triangles[t + 8] = v + 2;
            triangles[t + 9] = v + 2;
            triangles[t + 10] = v + xSize + 2;
            triangles[t + 11] = v + xSize + 3;

            isRoad[v] = true;
            isRoad[v + 1] = true;
            isRoad[v + 2] = true;

            isItemPlace[v + 1] = true;
            if (!isWayPoint)
            {
                GenerateWayPoint(vertices[v + 1 + (xSize + 1)]);
                isWayPointPlace[v + 1 + (xSize + 1)] = true;
                isWayPoint = true;
            }

            if (z > minZ + 1)
            {
                buildingState[v + 5] = (int)buildingDirection.LEFT;
                buildingState[v - 3] = (int)buildingDirection.RIGHT;
            }
            v += xSize + 1;
            t += 12;
        }
        if (v + 1 + (xSize + 1) < xSize * zSize)
            GenerateWayPoint(vertices[v + 1 + (xSize + 1)]);
    }
    void downSplitX(int minX, int maxX, int minZ, int maxZ, int num)
    {
        downXSplit[num] = Random.Range(minX + roadInterval, maxX - roadInterval);

        int v;
        if (num - 1 < 0)
            v = downXSplit[num];
        else
            v = ((xSize + 1) * minZ) + downXSplit[num];

        bool isWayPoint = false;

        for (int z = minZ; z < maxZ; ++z)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;

            triangles[t + 6] = v + 1;
            triangles[t + 7] = v + xSize + 2;
            triangles[t + 8] = v + 2;
            triangles[t + 9] = v + 2;
            triangles[t + 10] = v + xSize + 2;
            triangles[t + 11] = v + xSize + 3;

            isRoad[v] = true;
            isRoad[v + 1] = true;
            isRoad[v + 2] = true;

            isItemPlace[v + 1] = true;
            if (!isWayPoint)
            {
                GenerateWayPoint(vertices[v + 1 + (xSize + 1)]);
                isWayPointPlace[v + 1 + (xSize + 1)] = true;
                isWayPoint = true;
            }

            if (z > minZ + 1)
            {
                buildingState[v + 5] = (int)buildingDirection.LEFT;
                buildingState[v - 3] = (int)buildingDirection.RIGHT;
            }

            v += xSize + 1;
            t += 12;
        }
        GenerateWayPoint(vertices[v + 1 + (xSize + 1)]);
    }
    void leftSplitZ(int minZ, int maxZ, int minX, int maxX, int num)
    {
        leftZSplit[num] = Random.Range(minZ + roadInterval, maxZ - roadInterval);

        int v = (xSize + 1) * (leftZSplit[num]) + minX;
        bool isWayPoint = false;

        for (int x = minX + 1; x < maxX + 1; ++x)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;

            if (v + xSize * 2 + 2 < xSize * zSize)
            {
                triangles[t + 6] = v + xSize + 1;
                triangles[t + 7] = v + xSize * 2 + 2;
                triangles[t + 8] = v + xSize + 2;
                triangles[t + 9] = v + xSize + 2;
                triangles[t + 10] = v + xSize * 2 + 2;
                triangles[t + 11] = v + xSize * 2 + 3;

                isRoad[v] = true;
                isRoad[v + xSize + 1] = true;
                isRoad[v + xSize * 2 + 2] = true;

                isItemPlace[v + xSize + 1] = true;
                if (!isWayPoint)
                {
                    GenerateWayPoint(vertices[v + xSize + 1 + 1]);
                    isWayPointPlace[v + xSize + 1 + 1] = true;
                    isWayPoint = true;
                }
            }


            if (minX + 1 < x && x < maxX - 1)
            {
                if (v + (xSize + 1) * 5 < (xSize + 1) * (zSize + 1))
                    buildingState[v + (xSize + 1) * 5] = (int)buildingDirection.DOWN;
                if (v - (xSize - 1) * 3 > 0)
                    buildingState[v - (xSize - 1) * 3] = (int)buildingDirection.UP;
            }
            v++;
            t += 12;
        }
        GenerateWayPoint(vertices[v + 1 + xSize + 1]);
    }
    void rightSplitZ(int minZ, int maxZ, int minX, int maxX, int num)
    {
        rightZSplit[num] = Random.Range(minZ + roadInterval, maxZ - roadInterval);

        int v = (xSize + 1) * (rightZSplit[num]) + minX;
        bool isWayPoint = false;

        for (int x = minX + 1; x < maxX + 1; ++x)
        {
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + xSize + 1;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + xSize + 1;
            triangles[t + 5] = v + xSize + 2;


            if (v + xSize * 2 + 2 < xSize * zSize)
            {
                triangles[t + 6] = v + xSize + 1;
                triangles[t + 7] = v + xSize * 2 + 2;
                triangles[t + 8] = v + xSize + 2;
                triangles[t + 9] = v + xSize + 2;
                triangles[t + 10] = v + xSize * 2 + 2;
                triangles[t + 11] = v + xSize * 2 + 3;

                isRoad[v] = true;
                isRoad[v + xSize + 1] = true;
                isRoad[v + xSize * 2 + 2] = true;

                isItemPlace[v + xSize + 1] = true;

                if (!isWayPoint)
                {
                    GenerateWayPoint(vertices[v + xSize + 1 + 1]);
                    isWayPointPlace[v + xSize + 1 + 1] = true;
                    isWayPoint = true;
                }
            }

            if (minX + 1 < x && x < maxX - 1)
            {
                if (v + (xSize + 1) * 5 < (xSize + 1) * (zSize + 1))
                    buildingState[v + (xSize + 1) * 5] = (int)buildingDirection.DOWN;
                if (v - (xSize - 1) * 3 > 0)
                    buildingState[v - (xSize + 1) * 3] = (int)buildingDirection.UP;
            }
            v++;
            t += 12;
        }
        if (v + xSize + 1 < xSize * zSize)
            GenerateWayPoint(new Vector3(vertices[v + xSize + 1].x + 5, vertices[v + xSize + 1].y, vertices[v + xSize + 1].z));
    }

    void makeNotBuildingPlace()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            if (isRoad[i] == true
                && i + (xSize + 1) * 2 < (xSize + 1) * (zSize + 1)
                && i - (xSize - 1) * 2 > 0)
            {
                buildingState[i - 2] = (int)buildingDirection.NOTBUILDINGPLACE;
                buildingState[i - 1] = (int)buildingDirection.NOTBUILDINGPLACE;
                buildingState[i] = (int)buildingDirection.NOTBUILDINGPLACE;
                buildingState[i + 1] = (int)buildingDirection.NOTBUILDINGPLACE;
                buildingState[i + 2] = (int)buildingDirection.NOTBUILDINGPLACE;

                buildingState[i - xSize - 1] = (int)buildingDirection.NOTBUILDINGPLACE;
                buildingState[i + xSize + 1] = (int)buildingDirection.NOTBUILDINGPLACE;
            }
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        middleRoadNum = 0;
        for (int i = 0; i < isRoad.Length; ++i)
        {
            if (isItemPlace[i] == true)
            {
                buildingState[i] = (int)buildingDirection.NOTBUILDINGPLACE;
                passibleItemPlace[middleRoadNum] = vertices[i];
                ++middleRoadNum;
            }
        }
    }
    public void RefreshRoadVertices()
    {
        vertices = map.vertices;
    }

    void GenerateWayPoint(Vector3 position)
    {
        Transform parent = GameObject.Find("WayPoint").transform;
        wayPoint[wayPointNum] = Instantiate(wayPointPrefab, position, Quaternion.identity, parent);
        wayPoint[wayPointNum].layer = 16;
        ++wayPointNum;
    }

    enum buildingDirection
    {
        NOTBUILDINGPLACE,
        DOWN,
        UP,
        RIGHT,
        LEFT,
        PIZZABUILDING,
        BUILDING,
    };
}
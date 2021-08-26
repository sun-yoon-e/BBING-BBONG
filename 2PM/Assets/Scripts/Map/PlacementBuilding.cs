using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlacementBuilding : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    RoadGenerator road;
    MeshGenerator map;

    public GameObject[] buildingPrefab;
    public GameObject buildingParent;

    public GameObject[] buildingObject;

    public int num;
    public int buildingNum;
    public float buildingScale;

    // 건물간 간격 사이즈
    private int interval;

    public event EventHandler OnBuildingReady;
    public event EventHandler OnBuildingReady2;
    public bool isBuildingReady = false;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        map = GameObject.Find("Terrain Generator").GetComponent<MeshGenerator>();

        interval = 2;

        num = 0;
        buildingNum = 0;
        buildingObject = new GameObject[1000];

        road.OnRoadReady2 += OnRoadReady;
        if (road.isRoadReady)
        {
            OnRoadReady(this, EventArgs.Empty);
        }
        GameClient.Instance.OnMakeBuilding += OnMakeBuilding;
    }

    private void Start()
    {
    }

    private void OnRoadReady(object sender, EventArgs args)
    {
        //Debug.Log("OnRoadReady() 동작");

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.buildingState[i] == (int)buildingDirection.NOTBUILDINGPLACE
                || road.buildingState[i] == (int)buildingDirection.BUILDING
                || road.buildingState[i] == (int)buildingDirection.PIZZABUILDING)
                continue;

            int prefab = Random.Range(0, buildingPrefab.Length);

            // 오른쪽 메쉬가 빌딩플레이스일 때 옆으로 간격 조정
            if (road.buildingState[i + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                i += interval;
            }

            // i(vertex)가 array를 넘어가지 않고 i 위의 메쉬가 빌딩플레이스일 때
            if ((i + ((road.xSize + 1) * interval) < road.vertices.Length) &&
                road.buildingState[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                // 위 간격 조정을 위한 notplace지정
                for (int j = 1; j <= interval; ++j)
                    road.buildingState[i + (road.xSize + 1) * j] = (int)buildingDirection.NOTBUILDINGPLACE;
            }

            if (gameClient.client_host)
            {
                gameClient.BuildingInfo[num] = new MakeBuildingMessageEventArgs();
                //GameClient.Instance.MakeBuilding((byte)prefab, road.vertices[i], road.buildingState[i]);
                gameClient.BuildingInfo[num].Type = (byte)prefab;
                gameClient.BuildingInfo[num].Position = road.vertices[i];
                gameClient.BuildingInfo[num].dir = road.buildingState[i];
                num++;
            }

            makeNotBuildingPlace(i);
        }
        map.UpdateMesh();
        road.vertices = map.vertices;
        road.UpdateMesh();
        
        isBuildingReady = true;
        gameClient.isReadyToControl = true;

        if (OnBuildingReady != null)
        {
            OnBuildingReady(this, EventArgs.Empty);
        }
        if (OnBuildingReady2 != null)
        {
            OnBuildingReady2(this, EventArgs.Empty);
        }
    }

    void makeNotBuildingPlace(int place)
    {
        for (int i = 1; i <= interval; ++i)
        {
            if ((place + 4 * (road.xSize + 1) + i < road.vertices.Length) && (place - 2 * (road.xSize + 1) - 2 > 0))
            {
                road.buildingState[place - 2 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place - 2 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place - 2 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;

                road.buildingState[place - i] = (int)buildingDirection.BUILDING;
                road.buildingState[place] = (int)buildingDirection.BUILDING;
                road.buildingState[place + i] = (int)buildingDirection.BUILDING;

                road.buildingState[place - road.xSize - i - 1] = (int)buildingDirection.BUILDING;
                road.buildingState[place - road.xSize - i] = (int)buildingDirection.BUILDING;
                road.buildingState[place - road.xSize] = (int)buildingDirection.BUILDING;
                road.buildingState[place - road.xSize + 1] = (int)buildingDirection.BUILDING;

                road.buildingState[place + road.xSize - 1] = (int)buildingDirection.BUILDING;
                road.buildingState[place + road.xSize] = (int)buildingDirection.BUILDING;
                road.buildingState[place + road.xSize + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + road.xSize + i + 1] = (int)buildingDirection.BUILDING;

                // z축 건물 간격 조정
                road.buildingState[place + 2 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 2 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 2 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;

                road.buildingState[place + 3 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 3 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 3 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;

                if (road.buildingState[place] == (int)buildingDirection.LEFT || road.buildingState[place] == (int)buildingDirection.RIGHT)
                {
                    road.buildingState[place + 4 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                    road.buildingState[place + 4 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                    road.buildingState[place + 4 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;
                }

                // 건물 주변 지형height 건물 height와 같게 맞추기
                map.vertices[place - i].y = map.vertices[place].y;
                map.vertices[place + i].y = map.vertices[place].y;

                map.vertices[place - road.xSize - i - 1].y = map.vertices[place].y;
                map.vertices[place - road.xSize - i].y = map.vertices[place].y;
                map.vertices[place - road.xSize].y = map.vertices[place].y;
                map.vertices[place - road.xSize + 1].y = map.vertices[place].y;

                map.vertices[place + road.xSize - 1].y = map.vertices[place].y;
                map.vertices[place + road.xSize].y = map.vertices[place].y;
                map.vertices[place + road.xSize + i].y = map.vertices[place].y;
                map.vertices[place + road.xSize + i + 1].y = map.vertices[place].y;
            }
        }
    }

    //void makeObjectPlace(int place)
    //{
    //    // 건물 사이 
    //    if (place + 3 * (road.xSize + 1) + 1 < road.vertices.Length && place - 2 * (road.xSize + 1) - 1 > 0)
    //    {
    //        road.isObjectPlace[place + 2 * (road.xSize + 1) + 1] = true;
    //        road.isObjectPlace[place + 2 * (road.xSize + 1)] = true;
    //        road.isObjectPlace[place + 2 * (road.xSize + 1) - 1] = true;

    //        road.isObjectPlace[place - 2 * (road.xSize + 1) + 1] = true;
    //        road.isObjectPlace[place - 2 * (road.xSize + 1)] = true;
    //        road.isObjectPlace[place - 2 * (road.xSize + 1) - 1] = true;

    //        road.isObjectPlace[place + road.xSize + 3] = true;
    //        road.isObjectPlace[place + 2] = true;
    //        road.isObjectPlace[place - road.xSize + 1] = true;

    //        road.isObjectPlace[place + road.xSize - 1] = true;
    //        road.isObjectPlace[place - 2] = true;
    //        road.isObjectPlace[place - road.xSize - 3] = true;

    //        if (road.buildingState[place] == (int)buildingDirection.DOWN)
    //        {
    //            road.isObjectPlace[place + 3 * (road.xSize + 1) + 1] = true;
    //            road.isObjectPlace[place + 3 * (road.xSize + 1)] = true;
    //            road.isObjectPlace[place + 3 * (road.xSize + 1) - 1] = true;
    //        }
    //    }
    //}

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

    public void OnMakeBuilding(object sender, MakeBuildingMessageEventArgs args)
    {
        if (!gameClient.isRenderBuilding)
        {
            //Debug.Log(args.dir);
            if (args.dir == (int)buildingDirection.DOWN)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[args.Type], args.Position, Quaternion.identity);
            else if (args.dir == (int)buildingDirection.UP)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[args.Type], args.Position, Quaternion.Euler(0, 180, 0));
            else if (args.dir == (int)buildingDirection.LEFT)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[args.Type], args.Position, Quaternion.Euler(0, 90, 0));
            else if (args.dir == (int)buildingDirection.RIGHT)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[args.Type], args.Position, Quaternion.Euler(0, -90, 0));
            else
                return;

            buildingObject[buildingNum].transform.SetParent(buildingParent.transform);
            buildingObject[buildingNum].transform.localScale = new Vector3(buildingScale, buildingScale, buildingScale);
            ++buildingNum;
        }
    }
}
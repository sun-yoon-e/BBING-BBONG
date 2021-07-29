using UnityEngine;

public class PlacementBuilding : MonoBehaviour
{
    RoadGenerator road;
    MeshGenerator map;

    public GameObject[] buildingPrefab;
    public GameObject buildingParent;

    public GameObject[] buildingObject;

    public int buildingNum;

    // 건물간 간격 사이즈
    private int interval = 2;

    public GameObject boxcol;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();
    }

    private void Start()
    {
        buildingNum = 0;
        buildingObject = new GameObject[1000];

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
            if ((i + ((road.xSize + 1) * interval) < (road.xSize * road.zSize)) &&
                road.buildingState[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                // 위 간격 조정을 위한 notplace지정
                for (int j = 1; j <= interval; ++j)
                    road.buildingState[i + (road.xSize + 1) * j] = (int)buildingDirection.NOTBUILDINGPLACE;
            }
            if (road.buildingState[i] == (int)buildingDirection.DOWN)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);
            else if (road.buildingState[i] == (int)buildingDirection.UP)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 180, 0));
            else if (road.buildingState[i] == (int)buildingDirection.LEFT)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 90, 0));
            else if (road.buildingState[i] == (int)buildingDirection.RIGHT)
                buildingObject[buildingNum] = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, -90, 0));
            else continue;

            makeNotBuildingPlace(i);

            buildingObject[buildingNum].transform.SetParent(buildingParent.transform);

            buildingObject[buildingNum].AddComponent<BoxCollider>();
            BoxCollider col = buildingObject[buildingNum].GetComponent<BoxCollider>();
            col.tag = "buildingBoxCollider";

            //GameObjectUtility.SetStaticEditorFlags(buildingObject[buildingNum], StaticEditorFlags.NavigationStatic);

            ++buildingNum;
        }
        print(buildingNum);

        map.UpdateMesh();

        road.vertices = map.vertices;
        road.UpdateMesh();

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isRoad[i] == true)
            {
                if (i + 1 < road.xSize * road.zSize && road.isRoad[i + 1] == false)
                    Instantiate(boxcol, road.vertices[i + 1], Quaternion.identity, buildingParent.transform);

                else if (i + road.xSize + 1 < road.xSize * road.zSize && road.isRoad[i + road.xSize + 1] == false)
                    Instantiate(boxcol, road.vertices[i + road.xSize + 1], Quaternion.identity, buildingParent.transform);

                else if (i - road.xSize - 1 > 0 && road.isRoad[i - road.xSize - 1] == false)
                    Instantiate(boxcol, road.vertices[i - road.xSize - 1], Quaternion.identity, buildingParent.transform);

                else if (i - 1 > 0 && road.isRoad[i - 1] == false)
                    Instantiate(boxcol, road.vertices[i - 1], Quaternion.identity, buildingParent.transform);
            }
            
        }
    }

    void makeNotBuildingPlace(int place)
    {   
        for (int i = 1; i <= interval; ++i)
        {
            if (place + i < road.xSize * road.zSize
                || place - i > 0)
            {
                road.buildingState[place - i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + i] = (int)buildingDirection.BUILDING;
                map.vertices[place - i].y = map.vertices[place].y;
                map.vertices[place + i].y = map.vertices[place].y;
            }

            if (place - road.xSize - 2 > 0)
            {
                road.buildingState[place - road.xSize - i] = (int)buildingDirection.BUILDING;
                road.buildingState[place - road.xSize] = (int)buildingDirection.BUILDING;
                road.buildingState[place - road.xSize - i - 1] = (int)buildingDirection.BUILDING;
                map.vertices[place - road.xSize - i].y = map.vertices[place].y;
                map.vertices[place - road.xSize].y = map.vertices[place].y;
                map.vertices[place - road.xSize - i - 1].y = map.vertices[place].y;
            }

            if (place + road.xSize + 2 < road.xSize * road.zSize)
            {
                road.buildingState[place + road.xSize] = (int)buildingDirection.BUILDING;
                road.buildingState[place + road.xSize + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + road.xSize + i + 1] = (int)buildingDirection.BUILDING;
                map.vertices[place + road.xSize].y = map.vertices[place].y;
                map.vertices[place + road.xSize + i].y = map.vertices[place].y;
                map.vertices[place + road.xSize + i + 1].y = map.vertices[place].y;
            }

            if (place + 4 * (road.xSize + 1) + i < road.xSize * road.zSize)
            {
                road.buildingState[place + 2 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 2 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 2 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;

                road.buildingState[place + 3 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 3 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 3 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;

                road.buildingState[place + 4 * (road.xSize + 1) + i] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 4 * (road.xSize + 1)] = (int)buildingDirection.BUILDING;
                road.buildingState[place + 4 * (road.xSize + 1) - i] = (int)buildingDirection.BUILDING;
            }
        }

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

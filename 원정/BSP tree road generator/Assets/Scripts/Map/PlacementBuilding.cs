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
    private int interval;

    
    public float buildingScale;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        map = GameObject.Find("Terrain Generator").GetComponent<MeshGenerator>();

        interval = 2;
    }

    private void Start()
    {
        buildingNum = 0;
        buildingObject = new GameObject[1000];

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.buildingState[i] == (int) buildingDirection.NOTBUILDINGPLACE
                || road.buildingState[i] == (int) buildingDirection.BUILDING
                || road.buildingState[i] == (int) buildingDirection.PIZZABUILDING)
                continue;

            int prefab = Random.Range(0, buildingPrefab.Length);

            // 오른쪽 메쉬가 빌딩플레이스일 때 옆으로 간격 조정
            if (road.buildingState[i + 1] != (int) buildingDirection.NOTBUILDINGPLACE)
            {
                i += interval;
            }

            // i(vertex)가 array를 넘어가지 않고 i 위의 메쉬가 빌딩플레이스일 때
            if ((i + ((road.xSize + 1) * interval) < (road.xSize * road.zSize)) &&
                road.buildingState[i + road.xSize + 1] != (int) buildingDirection.NOTBUILDINGPLACE)
            {
                // 위 간격 조정을 위한 notplace지정
                for (int j = 1; j <= interval; ++j)
                    road.buildingState[i + (road.xSize + 1) * j] = (int) buildingDirection.NOTBUILDINGPLACE;
            }

            if (road.buildingState[i] == (int) buildingDirection.DOWN)
                buildingObject[buildingNum] =
                    Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);
            else if (road.buildingState[i] == (int) buildingDirection.UP)
                buildingObject[buildingNum] =
                    Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 180, 0));
            else if (road.buildingState[i] == (int) buildingDirection.LEFT)
                buildingObject[buildingNum] =
                    Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 90, 0));
            else if (road.buildingState[i] == (int) buildingDirection.RIGHT)
                buildingObject[buildingNum] =
                    Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, -90, 0));
            else continue;

            makeNotBuildingPlace(i);
            //makeObjectPlace(i);

            buildingObject[buildingNum].transform.SetParent(buildingParent.transform);
            buildingObject[buildingNum].transform.localScale = new Vector3(buildingScale, buildingScale, buildingScale);

            buildingObject[buildingNum].AddComponent<BoxCollider>();
            BoxCollider col = buildingObject[buildingNum].GetComponent<BoxCollider>();
            col.tag = "buildingBoxCollider";

            ++buildingNum;
        }

        print(buildingNum);

        map.UpdateMesh();

        road.vertices = map.vertices;
        road.UpdateMesh();
    }

void makeNotBuildingPlace(int place)
    {
        for (int i = 1; i <= interval; ++i)
        {
            if ((place + 2 * (road.xSize + 1) + 2 < road.xSize * road.zSize) && (place - 2 * (road.xSize + 1) - 2 > 0))
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
    //    if (place + 3 * (road.xSize + 1) + 1 < road.xSize * road.zSize && place - 2 * (road.xSize + 1) - 1 > 0)
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
}

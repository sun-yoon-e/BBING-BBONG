using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    PlacementBuilding building;
    RoadGenerator road;
    public GameObject[] objectPrefab;
    int num = 0, treeNum = 0;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        building = GameObject.Find("Building Generator").GetComponent<PlacementBuilding>();
        building.OnBuildingReady2 += CreateObjectPrefab;
        if (building.isBuildingReady)
        {
            CreateObjectPrefab(this, EventArgs.Empty);
        }

        gameClient.OnMakeTree += OnMakeTree;
    }

    private void Start()
    {

    }

    private void CreateObjectPrefab(object sender, EventArgs args)
    {
        if (gameClient.client_host)
        {
            int prefab = 0;
            int isObject;

            for (int i = 0; i < road.vertices.Length; ++i)
            {
                if ((road.isObjectPlace[i] == false && road.buildingState[i] != 0) || road.isRoad[i] == true)
                    continue;

                isObject = Random.Range(0, 4);
                if (isObject == 1)
                {
                    prefab = Random.Range(0, objectPrefab.Length);
                    //gameClient.MakeTree((byte)prefab, road.vertices[i]);
                    gameClient.TreeInfo[num] = new MakeTreeMessageEventArgs();
                    gameClient.TreeInfo[num].Type = (byte)prefab;
                    gameClient.TreeInfo[num].Position = road.vertices[i];
                    num++;
                }
            }
        }
    }

    public void OnMakeTree(object sender, MakeTreeMessageEventArgs args)
    {
        //Debug.Log($"{args.Type}, {args.Position}");
        if (!gameClient.isRenderTree)
        {
            Instantiate(objectPrefab[args.Type], args.Position, Quaternion.identity, transform);
            ++treeNum;

            if (gameClient.TreeInfo.Length == treeNum) gameClient.isRenderTree = true;
        }
    }
}
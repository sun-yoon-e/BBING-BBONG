using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
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

        GameClient.Instance.OnMakeTree += OnMakeTree;
    }

    private void Start()
    {

    }

    private void CreateObjectPrefab(object sender, EventArgs args)
    {
        if (GameClient.Instance.client_host)
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
                    //GameClient.Instance.MakeTree((byte)prefab, road.vertices[i]);
                    GameClient.Instance.TreeInfo[num] = new MakeTreeMessageEventArgs();
                    GameClient.Instance.TreeInfo[num].Type = (byte)prefab;
                    GameClient.Instance.TreeInfo[num].Position = road.vertices[i];
                    num++;
                }
            }
        }
    }

    public void OnMakeTree(object sender, MakeTreeMessageEventArgs args)
    {
        //Debug.Log($"{args.Type}, {args.Position}");
        if (!GameClient.Instance.isRenderTree)
        {
            Instantiate(objectPrefab[args.Type], args.Position, Quaternion.identity, transform);
            ++treeNum;

            if (GameClient.Instance.TreeInfo.Length == treeNum) GameClient.Instance.isRenderTree = true;
        }
    }
}
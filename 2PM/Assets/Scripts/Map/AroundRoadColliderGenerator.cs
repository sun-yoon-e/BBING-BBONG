using System;
using UnityEngine;

public class AroundRoadColliderGenerator : MonoBehaviour
{
    private RoadGenerator road;
    public GameObject boxcol;
    
    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        road.OnRoadReady += GenerateBoxCollider;
        if (road.isRoadReady)
        {
            GenerateBoxCollider(this, EventArgs.Empty);
        }
    }

    void GenerateBoxCollider(object sender, EventArgs args)
    {
        GameObject col = new GameObject();
        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isRoad[i] == true)
            {
                if (i + 1 < road.xSize * road.zSize && road.isRoad[i + 1] == false)
                    col = Instantiate(boxcol, road.vertices[i + 1], Quaternion.identity, transform);
                
                else if (i + road.xSize + 1 < road.xSize * road.zSize && road.isRoad[i + road.xSize + 1] == false)
                    col = Instantiate(boxcol, road.vertices[i + road.xSize + 1], Quaternion.identity, transform);

                else if (i - road.xSize - 1 > 0 && road.isRoad[i - road.xSize - 1] == false)
                    col = Instantiate(boxcol, road.vertices[i - road.xSize - 1], Quaternion.identity, transform);

                else if (i - 1 > 0 && road.isRoad[i - 1] == false)
                    col = Instantiate(boxcol, road.vertices[i - 1], Quaternion.identity, transform);

                col.tag = "mapBoxCollider";
            }
        }
    }
}

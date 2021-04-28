using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementBuilding : MonoBehaviour
{
    public GameObject buildingPrefab;

    private void Start()
    {
        RoadGenerator road = GetComponent<RoadGenerator>();

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isBuildingPlace[i] == false)
                continue;

            //if (i % (road.xSize + 1) != 0)
            {
                Instantiate(buildingPrefab, road.vertices[i], Quaternion.identity);
                
                Vector3 size = buildingPrefab.GetComponent<Renderer>().bounds.size;
                //i += (int)size.x;
            }
        }
    }
}

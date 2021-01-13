using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHelper : MonoBehaviour
{
    public GameObject roadStraight, roadCorner, road3way, road4way, roadEnd;
    private Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
    private HashSet<Vector3Int> fixRoadCandidates = new HashSet<Vector3Int>();

    public void PlaceStreetPositions(Vector3 startPosition, Vector3 direction, int length)
    {
        var rotation = Quaternion.identity;
        if (direction.x == 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        for (int i = 0; i < length; i++)
        {
            var position = Vector3Int.RoundToInt(startPosition + direction * i);
            if (roadDictionary.ContainsKey(position))
            {
                continue;
            }
            var road = Instantiate(roadStraight, position, rotation, transform);
            roadDictionary.Add(position, road);
            if (i == 0 || i == length - 1)
            {
                fixRoadCandidates.Add(position);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTerrainHeight : MonoBehaviour
{
    public Terrain TerrainMain;

    public void FixTerrainHeight(List<Vector3Int> roadPositions)
    {
        int roadX; 
        int roadZ;

        foreach (var position in roadPositions)
        {
            if (position.x > -128 && position.x < 128 && position.z > -128 && position.z < 128)
            {
                Debug.Log(position.ToString());
                roadX = position.x;
                roadZ = position.z;

                float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, 256, 256);

                heights[roadZ + 128, roadX + 128] = 1f;
                // 음수여서 오류남

                TerrainMain.terrainData.SetHeights(0, 0, heights);
            }
        }
    }
}

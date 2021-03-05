using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTerrainHeight : MonoBehaviour
{
    public Terrain TerrainMain;

    public float terrainHeight = 0.4f;
    
    public void FixTerrainHeight(List<Vector3Int> roadPositions)
    {
        Vector3 terrainSize = TerrainMain.terrainData.size;
        int terrainSizeX = (int)terrainSize.x;
        int terrainSizeZ = (int) terrainSize.z;
        
        int roadX; 
        int roadZ;

        foreach (var position in roadPositions)
        {
            if (position.x > -128 && position.x < 128 && position.z > -128 && position.z < 128)
            {
                Debug.Log(position.ToString());
                roadX = position.x;
                roadZ = position.z;

            float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, terrainSizeX, terrainSizeZ);

            for (int i = -5; i < 5; i++)
            {
                for (int j = -5; j < 5; j++)
                {
                    if (Mathf.Abs(i) == Mathf.Abs(j))
                    {
                        terrainHeight = 0.4f + Mathf.Abs(i) * 0.01f;
                    }
                    heights[roadZ + i + terrainSizeX/2, roadX + j + terrainSizeZ/2] = terrainHeight;
                    // 음수여서 오류남
                }
            }
            TerrainMain.terrainData.SetHeights(0, 0, heights);
        }
    }
}

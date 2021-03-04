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
        int roadY;
        
        foreach (var position in roadPositions)
        {
            Debug.Log(position.ToString());
            roadX = position.x;
            roadY = position.y;

            float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, 256, 256);

            heights[roadX, roadY] = 1f;

            TerrainMain.terrainData.SetHeights(0, 0, heights);
        }
    }
}

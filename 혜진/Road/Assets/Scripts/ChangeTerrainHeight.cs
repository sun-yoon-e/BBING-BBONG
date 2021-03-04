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
            Debug.Log(position.ToString());
            roadX = position.x;
            roadZ = position.z;

            float[,] heights = TerrainMain.terrainData.GetHeights(0, 0, 256, 256);

            heights[roadX+128, roadZ+128] = 1f;
            // 음수여서 오류남

            TerrainMain.terrainData.SetHeights(0, 0, heights);
        }
    }

    //public void ConvertWordCor2TerrCor(List<Vector3Int> roadPositions)
    //{
    //    foreach (var wordCor in roadPositions)
    //    {
    //        Vector3 vecRet = new Vector3();
    //        Terrain ter = Terrain.activeTerrain;
    //        Vector3 terPosition = ter.transform.position;
    //        vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
    //        vecRet.z = ((wordCor.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
    //    }
    //}
}

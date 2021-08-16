using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TempAIGenerator : MonoBehaviour
{
    // 클라에서 AI 생성을 위해 임시로 작성한 스크립트 - 서버를 붙이면 삭제할 것

    RoadGenerator road;

    public GameObject AIPrefab;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        transform.position = road.vertices[road.vertices.Length / 2 + 1 + road.xSize + 1];
        Instantiate(AIPrefab, transform.position, Quaternion.identity);
    }
}

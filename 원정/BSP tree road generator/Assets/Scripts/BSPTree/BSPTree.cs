using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPTree : MonoBehaviour
{
    public BSPNode parentNode;

    private void Start()
    {
        // 제일 큰 초기 큐브를 처음에 하나 만든다
        GameObject startCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        startCube.transform.localScale = new Vector3(1000, 1, 1000);
        startCube.tag = "GenSection";
        startCube.transform.position = new Vector3(transform.position.x + startCube.transform.localScale.x / 2,
            transform.position.y,
            transform.position.z + startCube.transform.localScale.z / 2);

        parentNode = new BSPNode();
        parentNode.setCube(startCube);

        for (int i = 0; i < 9; ++i)
        {
            split(parentNode);
        }
    }

    public void split(BSPNode _aNode)
    {
        if (_aNode.getLeftNode() != null)
        {
            split(_aNode.getLeftNode());
        }
        else
        {
            _aNode.cut();
            return;
        }

        if (_aNode.getLeftNode() != null)
        {
            split(_aNode.getRightNode());
        }
    }
}

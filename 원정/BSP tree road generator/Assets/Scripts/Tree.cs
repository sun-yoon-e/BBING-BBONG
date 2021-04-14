using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public Node parentNode;

    void Start()
    {
        MeshGenerator startMesh = new MeshGenerator();
        startMesh.xSize = 100;
        startMesh.zSize = 100;
        startMesh.tag = "GenSection";

        parentNode = new Node();
        parentNode.setMesh(startMesh);

        for(int i=0;i < 9; ++i)
        {
            split(parentNode);
        }
    }

    public void split(Node node)
    {
        if(node.getLeftNode() != null)
        {
            split(node.getLeftNode());
        }
        else
        {
            node.cut();
            return;
        }

        if(node.getLeftNode() != null)
        {
            split(node.getRightNode());
        }
    }
}

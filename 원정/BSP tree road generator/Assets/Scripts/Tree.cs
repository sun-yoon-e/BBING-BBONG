using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    Node parentNode;

    void Start()
    {
        MapMesh startMesh = new MapMesh();

        GetComponent<MeshFilter>().mesh = startMesh.mesh;
        startMesh.CreateShape();
        startMesh.UpdateMesh();
        //startMesh.tag = "GenSection";

        parentNode = new Node();
        parentNode.setMesh(startMesh);

        //for (int i = 0; i < 9; ++i)
        //{
        //    split(parentNode);
        //}

        Instantiate(parentNode.mapMesh.mesh, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        
        GetComponent<MeshFilter>().mesh = parentNode.mapMesh.mesh;
        parentNode.mapMesh.CreateShape();
        parentNode.mapMesh.UpdateMesh();
        

        Destroy(startMesh.mesh);
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

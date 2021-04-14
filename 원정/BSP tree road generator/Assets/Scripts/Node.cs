using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    MeshGenerator mapMesh;
    Node parentNode;
    Node leftNode;
    Node rightNode;

    void splitX(MeshGenerator map, float minX)
    {
        //float split = Random.Range(minX, map.transform.localScale.x - minX);
        float split = Random.Range(minX, map.xSize - minX);

        if (split > minX)
        {
            MeshGenerator leftPlace = new MeshGenerator();
            leftPlace.xSize = (int)split;
            leftPlace.zSize = map.zSize;
            leftPlace.mapPosition = map.mapPosition;

            leftPlace.CreateShape();
            leftPlace.tag = "GenSection";

            leftNode = new Node();
            leftNode.setMesh(leftPlace);
            leftNode.setParentNode(this);


            MeshGenerator rightPlace = new MeshGenerator();
            rightPlace.xSize = map.xSize - (int)split;
            rightPlace.zSize = map.zSize;
            rightPlace.mapPosition.x = map.mapPosition.x + split;
            rightPlace.mapPosition.z = map.mapPosition.z;

            rightPlace.CreateShape();
            rightPlace.tag = "GenSection";

            rightNode = new Node();
            rightNode.setMesh(rightPlace);
            rightNode.setParentNode(this);

            GameObject.Destroy(map);
        }
    }

    void splitZ(MeshGenerator map, float minZ)
    {
        //float split = Random.Range(minZ, map.transform.localScale.z - minZ);
        float split = Random.Range(minZ, map.zSize - minZ);

        if (split > minZ)
        {
            MeshGenerator upPlace = new MeshGenerator();
            upPlace.xSize = map.xSize;
            upPlace.zSize = map.zSize - (int)split;
            upPlace.mapPosition.x = map.mapPosition.x;
            upPlace.mapPosition.z = map.mapPosition.z + split;

            upPlace.CreateShape();
            upPlace.tag = "GenSection";

            leftNode = new Node();
            leftNode.setMesh(upPlace);
            leftNode.setParentNode(this);


            MeshGenerator downPlace = new MeshGenerator();
            downPlace.xSize = map.xSize;
            downPlace.zSize = (int)split;
            downPlace.mapPosition.x = map.mapPosition.x;
            downPlace.mapPosition.z = map.mapPosition.z;

            downPlace.CreateShape();
            downPlace.tag = "GenSection";

            rightNode = new Node();
            rightNode.setMesh(downPlace);
            rightNode.setParentNode(this);

            GameObject.Destroy(map);
        }
    }

    public void setMesh(MeshGenerator mesh)
    {
        mapMesh = mesh;
    }
    public MeshGenerator getMesh()
    {
        return mapMesh;
    }

    public void cut()
    {
        float choice = Random.Range(0, 2);
        if (choice <= 0.5)
        {
            splitX(mapMesh, 10);
        }
        else
        {
            splitZ(mapMesh, 10);
        }
    }

    public void setLeftNode(Node node)
    {
        leftNode = node;
    }
    public void setRightNode(Node node)
    {
        rightNode = node;
    }
    public void setParentNode(Node node)
    {
        parentNode = node;
    }

    public Node getLeftNode()
    {
        return leftNode;
    }
    public Node getRightNode()
    {
        return rightNode;
    }
    public Node getParentNode()
    {
        return parentNode;
    }
}

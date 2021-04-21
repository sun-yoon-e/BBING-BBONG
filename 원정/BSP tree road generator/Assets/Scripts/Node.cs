using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public MapMesh mapMesh;
    Node parentNode;
    Node leftNode;
    Node rightNode;

    void splitX(MapMesh map, float minX)
    {
        float split = Random.Range(minX, map.xSize - minX);
        
        if (split > minX)
        {
            MapMesh leftPlace = new MapMesh();
            leftPlace.mesh = new Mesh();

            leftPlace.xSize = (int)split;
            leftPlace.zSize = map.zSize;
            leftPlace.mapPosition = map.mapPosition;

            leftPlace.CreateShape();
            leftPlace.UpdateMesh();
            //leftPlace.tag = "GenSection";

            leftNode = new Node();
            leftNode.setMesh(leftPlace);
            leftNode.setParentNode(this);


            MapMesh rightPlace = new MapMesh();
            rightPlace.mesh = new Mesh();

            rightPlace.xSize = map.xSize - (int)split;
            rightPlace.zSize = map.zSize;
            rightPlace.mapPosition.x = map.mapPosition.x + split;
            rightPlace.mapPosition.z = map.mapPosition.z;

            rightPlace.CreateShape();
            rightPlace.UpdateMesh();
            //rightPlace.tag = "GenSection";

            rightNode = new Node();
            rightNode.setMesh(rightPlace);
            rightNode.setParentNode(this);

            Mesh.Destroy(map.mesh);
        }
    }

    void splitZ(MapMesh map, float minZ)
    {
        //float split = Random.Range(minZ, map.transform.localScale.z - minZ);
        float split = Random.Range(minZ, map.zSize - minZ);

        if (split > minZ)
        {
            MapMesh upPlace = new MapMesh();
            upPlace.mesh = new Mesh();

            upPlace.xSize = map.xSize;
            upPlace.zSize = map.zSize - (int)split;
            upPlace.mapPosition.x = map.mapPosition.x;
            upPlace.mapPosition.z = map.mapPosition.z + split;

            upPlace.CreateShape();
            upPlace.UpdateMesh();
            //upPlace.tag = "GenSection";

            leftNode = new Node();
            leftNode.setMesh(upPlace);
            leftNode.setParentNode(this);


            MapMesh downPlace = new MapMesh();
            downPlace.mesh = new Mesh();

            downPlace.xSize = map.xSize;
            downPlace.zSize = (int)split;
            downPlace.mapPosition.x = map.mapPosition.x;
            downPlace.mapPosition.z = map.mapPosition.z;

            downPlace.CreateShape();
            downPlace.UpdateMesh();
            //downPlace.tag = "GenSection";

            rightNode = new Node();
            rightNode.setMesh(downPlace);
            rightNode.setParentNode(this);
            
            Mesh.Destroy(map.mesh);
        }
    }

    public void setMesh(MapMesh mesh)
    {
        mapMesh = mesh;
    }
    public MapMesh getMesh()
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

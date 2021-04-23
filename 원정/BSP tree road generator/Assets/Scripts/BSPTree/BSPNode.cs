using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
    GameObject cube;
    BSPNode parentNode;
    BSPNode leftNode;
    BSPNode rightNode;

    Color myColor;

    public BSPNode()
    {
        myColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public void setLeftNode(BSPNode _aNode)
    {
        leftNode = _aNode;
    }
    public void setRightNode(BSPNode _aNode)
    {
        rightNode = _aNode;
    }
    public void setParentNode(BSPNode _aNode)
    {
        parentNode = _aNode;
    }

    public BSPNode getLeftNode()
    {
        return leftNode;
    }
    public BSPNode getRightNode()
    {
        return rightNode;
    }
    public BSPNode getParentNode()
    {
        return parentNode;
    }

    void splitX(GameObject _aSection, float minX)
    {
        // 입력받은 minX로 x자름
        float xSplit = Random.Range(minX, _aSection.transform.localScale.x - minX);

        if (xSplit > minX)
        {
            // 게임오브젝트 (left 큐브) 생성
            GameObject leftPlace = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // left 큐브의 스케일, 포지션, 컬러 지정
            // 포지션과 스케일의 x는 xSplit에 영향을 받음, 그외에는 _aSection과 동일
            leftPlace.transform.localScale = new Vector3(xSplit, _aSection.transform.localScale.y, _aSection.transform.localScale.z);
            leftPlace.transform.position = new Vector3(
                _aSection.transform.position.x - ((xSplit - _aSection.transform.localScale.x) / 2),
                _aSection.transform.position.y,
                _aSection.transform.position.z);
            leftPlace.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            // 큐브의 태그 지정, 이 노드의 leftNode로 left큐브를 지정
            leftPlace.tag = "GenSection";
            leftNode = new BSPNode();
            leftNode.setCube(leftPlace);
            leftNode.setParentNode(this);

            GameObject rightPlace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float split1 = _aSection.transform.localScale.x - xSplit;
            rightPlace.transform.localScale = new Vector3(split1, _aSection.transform.localScale.y, _aSection.transform.localScale.z);
            rightPlace.transform.position = new Vector3(
                _aSection.transform.position.x + ((split1 - _aSection.transform.localScale.x) / 2),
                _aSection.transform.position.y,
                _aSection.transform.position.z);
            rightPlace.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            rightPlace.tag = "GenSection";
            rightNode = new BSPNode();
            rightNode.setCube(rightPlace);
            rightNode.setParentNode(this);

            // 받아온 GameObject _aSection 제거
            GameObject.DestroyImmediate(_aSection);
        }
    }

    void splitZ(GameObject _aSection, float minZ)
    {
        float zSplit = Random.Range(minZ, _aSection.transform.localScale.z - minZ);
        float zSplit1 = _aSection.transform.localScale.z - zSplit;

        if (zSplit > minZ)
        {
            GameObject leftPlace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPlace.transform.localScale = new Vector3(_aSection.transform.localScale.x, _aSection.transform.localScale.y, zSplit);
            leftPlace.transform.position = new Vector3(
                _aSection.transform.position.x,
                _aSection.transform.position.y,
                _aSection.transform.position.z - ((zSplit - _aSection.transform.localScale.z) / 2));
            leftPlace.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            leftPlace.tag = "GenSection";
            leftNode = new BSPNode();
            leftNode.setCube(leftPlace);
            leftNode.setParentNode(this);

            GameObject rightPlace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPlace.transform.localScale = new Vector3(_aSection.transform.localScale.x, _aSection.transform.localScale.y, zSplit1);
            rightPlace.transform.position = new Vector3(
                _aSection.transform.position.x,
                _aSection.transform.position.y,
                _aSection.transform.position.z + ((zSplit1 - _aSection.transform.localScale.z) / 2));
            rightPlace.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            rightPlace.tag = "GenSection";
            rightNode = new BSPNode();
            rightNode.setCube(rightPlace);
            rightNode.setParentNode(this);

            GameObject.DestroyImmediate(_aSection);
        }
    }

    public void setCube(GameObject _aCube)
    {
        cube = _aCube;
    }

    public GameObject getCube()
    {
        return cube;
    }

    public void cut()
    {
        float choice = Random.Range(0, 2);
        if (choice <= 0.5)
        {
            splitX(cube, 10);
        }
        else
        {
            splitZ(cube, 10);
        }
    }

    public Color getColor()
    {
        return myColor;
    }
}

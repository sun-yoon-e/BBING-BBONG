using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIFirePizza : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    AIMovement movementScript;
    RoadGenerator road;

    public LayerMask whatIsDoor;
    
    Collider[] col;

    public GameObject pizzaPrefab;
    public Transform firePos;

    CreateAIID ID;

    public float sight;
    bool isReDestination = false;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        ID = transform.Find("AIID").GetComponent<CreateAIID>();
    }

    void Update()
    {
        if (movementScript.isArriveDestination == true)
        {
            if (!isReDestination)
                StartCoroutine("FindRoad");

            StartCoroutine("InstantiatePizza");
            StartCoroutine("resetSettings");
        }
    }
    IEnumerator FindRoad()
    {
        movementScript.isArriveDestination = false;

        yield return new WaitForSeconds(1f);

        isReDestination = true;

        col = Physics.OverlapSphere(transform.position, sight, whatIsDoor);

        if (col[0].enabled)
        {
            float minDistance = 10000f;
            int minDistanceRoadPosition = 0;

            for (int i = 0; i < road.passibleItemPlace.Length; ++i)
            {
                float distance = Vector3.Distance(road.passibleItemPlace[i], col[0].transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceRoadPosition = i;
                }
            }

            movementScript.agent.SetDestination(road.passibleItemPlace[minDistanceRoadPosition]);

            //Collider[] roadCol = Physics.OverlapSphere(col[0].gameObject.transform.position, sight, whatIsRoad);
            //print(roadCol.Length);

            //if (roadCol[0].enabled)
            //{
            //    //Vector3 dir = col[0].gameObject.transform.position - roadCol[0].transform.position;
            //    movementScript.agent.SetDestination(roadCol[0].gameObject.transform.position);
            //}
            //Physics.Raycast(col[0].transform.position, dir, out hit, 12f, whatIsRoad);
        }
        StopCoroutine("FindRoad");
    }

    IEnumerator InstantiatePizza()
    {
        yield return new WaitForSeconds(4f);

        if (col[0].enabled)
        {
            Vector3 vec = col[0].transform.position - transform.position;
            vec.Normalize();
            Quaternion pizzaRotation = Quaternion.LookRotation(vec);
            Vector3 pizzaPosition = firePos.position;

            //pizza = Instantiate(pizzaPrefab, pizzaPosition, pizzaRotation);
            if (gameClient.client_host)
            {
                gameClient.FirePizzaAI(ID.idNum, pizzaPosition, col[0].transform.position);
            }
            Destroy(col[0]);
        }
        StopCoroutine("InstantiatePizza");
    }

    IEnumerator resetSettings()
    {
        yield return new WaitForSeconds(3.9f);

        movementScript.SetAIDestination();
        movementScript.isArriveDestination = false;

        isReDestination = false;

        StopCoroutine("resetSettings");
    }

}
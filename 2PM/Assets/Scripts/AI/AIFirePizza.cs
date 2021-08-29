using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIFirePizza : MonoBehaviour
{
    AIMovement movementScript;
    AIRBController rbController;
    Rigidbody rb;

    public LayerMask whatIsDoor;
    Collider[] col;

    public GameObject pizzaPrefab;
    public Transform firePos;
    GameObject pizza;

    CreateAIID ID;

    public float sight;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
        ID = GameObject.Find("AIID").GetComponent<CreateAIID>();

        //rbController = GetComponent<AIRBController>();
        //rb = rbController.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (movementScript.isArriveDestination == true)
        {
            //print("기다림..");
            StartCoroutine("InstantiatePizza");
            StartCoroutine("resetSettings");
        }
    }

    IEnumerator InstantiatePizza()
    {
        yield return new WaitForSeconds(4f);

        col = Physics.OverlapSphere(transform.position, sight, whatIsDoor);

        if (col[0].enabled)
        {
            Vector3 vec = col[0].transform.position - transform.position;
            vec.Normalize();
            Quaternion pizzaRotation = Quaternion.LookRotation(vec);
            Vector3 pizzaPosition = firePos.position;

            //pizza = Instantiate(pizzaPrefab, pizzaPosition, pizzaRotation);
            if (GameClient.Instance.client_host)
            {
                GameClient.Instance.FirePizzaAI(ID.idNum, pizzaPosition, col[0].transform.position);
            }
            //print(col[0].transform.position);
            Destroy(col[0]);
        }

        StopCoroutine("InstantiatePizza");
    }

    IEnumerator resetSettings()
    {
        yield return new WaitForSeconds(3.9f);

        movementScript.SetAIDestination();
        movementScript.isStopPosition = false;
        movementScript.isArriveDestination = false;

        StopCoroutine("resetSettings");
    }

}
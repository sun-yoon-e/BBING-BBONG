using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFirePizza : MonoBehaviour
{
    AIMovement movementScript;

    public GameObject pizzaPrefab;
    public Transform firePos;
    GameObject pizza;

    public LayerMask whatIsDoor;
    Collider[] col;

    bool isFire;
    public float sight;

    Vector3 pizzaPosition;
    Quaternion pizzaRotation;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
    }

    void Update()
    {
        if(movementScript.isArriveDestination == true)
        {
            print("기다림..");
            StartCoroutine("InstantiatePizza");
            StartCoroutine("resetSettings");
            //Invoke("InstantiatePizza", 3f);
            
        }
    }

    IEnumerator InstantiatePizza()
    {
        yield return new WaitForSeconds(3f);

        col = Physics.OverlapSphere(transform.position, sight, whatIsDoor);

        if (col[0].enabled)
        {
            Vector3 vec = col[0].transform.position - transform.position;
            vec.Normalize();
            //firePos.rotation = Quaternion.LookRotation(vec);
            pizzaRotation = Quaternion.LookRotation(vec);
            pizzaPosition = firePos.position;

            pizza = Instantiate(pizzaPrefab, pizzaPosition, pizzaRotation);

            print(col[0].transform.position);

            Destroy(col[0]);
        }
        else
            print("주변에 문 없음");

        movementScript.isArriveDestination = false;

        StopCoroutine("InstantiatePizza");
    }

    IEnumerator resetSettings()
    {
        yield return new WaitForSeconds(2f);

        movementScript.SetAIDestination();
        movementScript.isStopPosition = false;

        StopCoroutine("resetSettings");
        //isFire = false;
    }

}

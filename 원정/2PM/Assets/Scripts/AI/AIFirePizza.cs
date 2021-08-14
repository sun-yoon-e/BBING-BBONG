using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFirePizza : MonoBehaviour
{
    AIMovement movementScript;
    public GameObject pizzaPrefab;
    public Transform firePos;

    public float sight;

    public LayerMask whatIsDoor;
    Collider[] col;

    bool isFire;
    GameObject pizza;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
        col = new Collider[5];
    }

    void Update()
    {
        if(movementScript.isArriveDestination == true)
        {
            col = Physics.OverlapSphere(transform.position, sight, whatIsDoor);

            if(col[0] == true)
            {
                if (!isFire)
                {
                    Vector3 vec = col[0].transform.position - transform.position;
                    vec.Normalize();

                    firePos.rotation = Quaternion.LookRotation(vec);

                    pizza = Instantiate(pizzaPrefab, firePos.position, firePos.rotation);
                    isFire = true;
                }

                print(col[0].transform.position);
            }
            else
            {
                // agent setDestination
            }
        }
    }

}

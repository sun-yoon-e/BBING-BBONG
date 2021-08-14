using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFirePizza : MonoBehaviour
{
    AIMovement movementScript;
    public GameObject pizzaPrefab;
    public Transform firePos;

    bool isDoor;
    public float sight;

    public LayerMask whatIsDoor;
    Collider[] col;

    bool isFire;
    GameObject pizza;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
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
                    Quaternion rot = firePos.rotation;
                    //Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                    firePos.rotation = Quaternion.Slerp(rot, Quaternion.LookRotation(col[0].transform.position), 1f);
                    pizza = Instantiate(pizzaPrefab, firePos);
                    isFire = true;
                }

                print(col[0].transform.position);
                print("있음");
                
            }
            else
            {
                // agent setDestination
            }
        }
    }

}

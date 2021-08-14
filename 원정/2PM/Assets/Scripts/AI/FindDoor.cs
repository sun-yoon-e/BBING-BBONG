using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindDoor : MonoBehaviour
{
    AIMovement movementScript;

    bool isDoor;
    public float sight;

    public LayerMask whatIsDoor;

    void Start()
    {
        movementScript = GetComponent<AIMovement>();
    }

    void Update()
    {
        if(movementScript.isArriveDestination == true)
        {
            isDoor = Physics.CheckSphere(transform.position, sight, whatIsDoor);
            if(isDoor == true)
            {
                print("있음");
            }
        }
    }

}

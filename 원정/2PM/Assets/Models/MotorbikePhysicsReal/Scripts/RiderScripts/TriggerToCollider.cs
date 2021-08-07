using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToCollider : MonoBehaviour
{

    CapsuleCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CapsuleCollider>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        collider.isTrigger = false;
    }
    private void OnTriggerExit(Collider other)
    {
        collider.isTrigger = true;
    }
}

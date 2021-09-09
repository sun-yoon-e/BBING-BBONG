using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePizza : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;
    
    public Transform firePos;
    public Transform targetPos;
    public GameObject pizza;
    
    public float range = 100f;
    public float coolTime = 20f;

    private float nextTimeToFire = 0f;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 20f / coolTime;
            animator.SetTrigger("FirePizza");
            SoundManager.instance.PlaySE("FirePizza");
        }
    }

    void Fire()
    {
        RaycastHit hit;
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.tag = "Player";
        pizzaObject.transform.position = firePos.position;
        Vector3 dir = new Vector3();
        
        int layermask = (1 << LayerMask.NameToLayer("BoxCol"));
        layermask = ~layermask;
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, range, layermask))
        {
            dir = hit.point - firePos.position;
        }
        else
        {
            dir = targetPos.position - firePos.position;
        }
        pizzaObject.transform.forward = dir;

        gameClient.FirePizza(firePos.position, targetPos.position, gameClient.playerRoomNum);
    }
}
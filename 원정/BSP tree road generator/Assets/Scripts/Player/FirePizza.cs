using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePizza : MonoBehaviour
{
    public Transform firePos;
    public Transform targetPos;
    public GameObject pizza;
    Animator anim;

    public float coolTime = 20f;

    private float nextTimeToFire = 0f;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 20f / coolTime;
            Fire();

           // anim.SetBool("isFire", true);
        }
    }

    void Fire()
    {
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = firePos.position;
        var dir = targetPos.position - firePos.position;
        pizzaObject.transform.forward = dir;
    }
}
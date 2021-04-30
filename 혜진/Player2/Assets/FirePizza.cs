using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePizza : MonoBehaviour
{
    public Camera cam;

    public GameObject pizza;
    //public Transform firePos;

    public float range = 100f;
    public float coolTime = 10f;

    private float nextTimeToFire = 0f;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 10f / coolTime;
            Fire();
        }
    }

    void Fire()
    {
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = cam.transform.position + cam.transform.forward;
        pizzaObject.transform.forward = cam.transform.forward;
    }
}
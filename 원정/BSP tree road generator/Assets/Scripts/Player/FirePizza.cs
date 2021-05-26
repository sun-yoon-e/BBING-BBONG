using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FirePizza : MonoBehaviour
{
    public Camera cam;
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
        }
    }

    void Fire() //애니메이션 이벤트로 발생
    {
        RaycastHit hit;
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.tag = "Player";
        pizzaObject.transform.position = firePos.position;
        Vector3 dir = new Vector3();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            dir = hit.point - firePos.position;
        }
        else
        {
            dir = targetPos.position - firePos.position;
        }
        pizzaObject.transform.forward = dir;
    }
}
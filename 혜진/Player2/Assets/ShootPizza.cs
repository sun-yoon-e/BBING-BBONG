using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPizza : MonoBehaviour
{
    public Camera cam;
    public GameObject pizza;
    //public Transform firePos;

    public float range = 100f;
    public float cooltime = 10f;

    private float nextTimeToFire = 0f;
    
    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 10f / cooltime;
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            
            if (target != null)
            {
                target.Hit();
            }

            GameObject pizzaGO = Instantiate(pizza, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(pizzaGO, 2f);   //2초 후 생성된 피자 오브젝트 삭제
        }
    }
}

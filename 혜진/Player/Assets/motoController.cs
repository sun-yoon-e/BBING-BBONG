using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motoController : MonoBehaviour
{
    public float speed;
    public float maxspeed = 100;
    public float rot = 1f;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") > 0 && speed < maxspeed)
        {
            speed += 10f * Time.deltaTime;
        }

        /*
        if (Input.GetKey(KeyCode.Space))
        {
            speed -= 1f;
            speed = Mathf.Clamp(speed, 0, maxspeed);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Rotate(Vector3.up * rot * Input.GetAxis("Horizontal") * Time.deltaTime);
        }
        */
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //anim.SetFloat("rotation", Input.GetAxis("Horizontal"));
    }
}

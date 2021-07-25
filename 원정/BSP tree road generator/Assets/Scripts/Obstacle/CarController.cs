﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    private float power = 5f;
    [SerializeField]
    private float torque = 5f;
    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private Vector3 movementVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 movementInput)
    {
        this.movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * power);
        }
        rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
    }
}

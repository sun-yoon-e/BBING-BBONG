using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Rigidbody rb;
    private Animator anim;
    
    public float moveSpeed = 10.0f;
    public float rotSpeed = 10.0f;
    
    public float mouseSensitivity = 10.0f;
    private float cameraRotationLimit = 60.0f;
    private float cameraRotationX;
    private float cameraRotationY;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        PlayerMove();
        CameraRotate();
    }

    void PlayerMove()
    {
        //플레이어 이동, 회전
        float keyboardX = Input.GetAxisRaw("Horizontal");
        float keyboardY = Input.GetAxisRaw("Vertical");

        float movDir = keyboardY * moveSpeed * Time.deltaTime;
        float rotDir = keyboardX * rotSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + transform.forward * movDir);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotDir));
    }

    void CameraRotate()
    {
        //카메라 회전 
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        cameraRotationY += mouseX;
        cameraRotationY = Mathf.Clamp(cameraRotationY, -cameraRotationLimit, cameraRotationLimit);

        cam.transform.localEulerAngles = new Vector3(cameraRotationX, cameraRotationY, 0f);
    }
}
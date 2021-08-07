using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCam : MonoBehaviour
{
    public Transform player;
    private Vector3 newPosition;

    private void Update()
    {
        transform.position = new Vector3(player.position.x, 490f, player.position.z);
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}

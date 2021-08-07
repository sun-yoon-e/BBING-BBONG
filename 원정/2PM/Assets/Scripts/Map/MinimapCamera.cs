using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Sprite destinationSprite;
    public Transform target;
    
    private void Update()
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        Vector3 ObjectScale = new Vector3(10, 10, 10);

        Vector3 destinationPosition =
            new Vector3(target.position.x,
                target.position.y,
                target.position.z);

        GameObject indicator = new GameObject("DestinationSprite");
        indicator.transform.position = destinationPosition;
        indicator.transform.rotation = SpriteRotation;
        indicator.transform.localScale = ObjectScale;

        SpriteRenderer indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
        indicatorRenderer.sprite = destinationSprite;

        indicator.layer = 8;
    }
}

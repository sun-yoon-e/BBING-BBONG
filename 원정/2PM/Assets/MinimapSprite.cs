using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    //private void Awake()
    //{
    //    var obj = FindObjectsOfType<DontDestroyObject>();
    //    if (obj.Length == 1)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        //if (gameObject != GameObject.Find("UnityMainThreadDispatcher"))
    //        Destroy(gameObject);
    //    }
    //}
}

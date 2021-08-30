using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaAnimator : MonoBehaviour
{
    public bool isAnimated;

    // Update is called once per frame
    void Update()
    {
        if (isAnimated)
        {
            GetComponent<Animator>().SetTrigger("FirePizza");
            //SoundManager.instance.PlaySE("FirePizza");
            isAnimated = false;
        }
    }
}

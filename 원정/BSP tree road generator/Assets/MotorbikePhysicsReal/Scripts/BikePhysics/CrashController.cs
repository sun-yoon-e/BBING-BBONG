using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gadd420
{

    public class CrashController : MonoBehaviour
    {
        public bool isCrashed;
        public string[] crashTag;

        //When the bikes crash trigger hits the ground isCrashed is set and only reset after respawning 
        //Check KeyboardShortucts script for isCrashed = false; 
        private void OnTriggerEnter(Collider other)
        {
            for(int i = 0; i < crashTag.Length; i++)
            {
                if (other.gameObject.CompareTag(crashTag[i]))
                {
                    isCrashed = true;
                }
            }
            
        }

    }
}

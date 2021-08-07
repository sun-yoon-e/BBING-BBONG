using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Gadd420
{

    public class KeyBoardShortCuts : MonoBehaviour
    {
        string sceneName;
        public Transform currentBike;
        Rigidbody bikeRB;
        CrashController crashScript;


        // Start is called before the first frame update
        void Start()
        {
            //Makes sure mouse is unlocked after restart
            Cursor.lockState = CursorLockMode.None;

            //Gets scene name for reloading the scene
            sceneName = SceneManager.GetActiveScene().name;

            
        }

        // Update is called once per frame
        void Update()
        {
            //When Bike is selected
            if (currentBike && !crashScript)
            {
                //Lock Cursor and get Components from the selected bike
                Cursor.lockState = CursorLockMode.Locked;
                bikeRB = currentBike.gameObject.GetComponent<Rigidbody>();
                crashScript = currentBike.gameObject.GetComponent<CrashController>();
            }

            //Restart
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(sceneName);
            }

            //Flip over
            if (Input.GetKeyDown(KeyCode.F) && crashScript.isCrashed)
            {
                //sets bike position upright
                currentBike.position = new Vector3(currentBike.position.x, currentBike.position.y + 1, currentBike.position.z);
                currentBike.rotation = Quaternion.Euler(0, currentBike.eulerAngles.y, 0);

                //stop velocitys carrying over into respawn
                bikeRB.velocity = Vector3.zero;
                bikeRB.angularVelocity = Vector3.zero;

                //Makes you be able to drive again
                crashScript.isCrashed = false;
            }

        }
    }
}

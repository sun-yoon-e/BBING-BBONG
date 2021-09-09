using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyObject : MonoBehaviour
{
    public static DontDestroyObject instance;
    
    void Start()
    {
        instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        /*
        var obj = FindObjectsOfType<DontDestroyObject>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }

    public void SceneChange()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Destroy(gameObject);
        }
        
        SceneManager.LoadScene("Scenes/LobbyScene", LoadSceneMode.Single);
    }
}

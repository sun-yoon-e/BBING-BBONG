using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[5];
    public Image[] images = new Image[2];

    private int ItemCnt = 0;
    private int[] MyItems = new int[2];

    GameObject mainCamera;
    GameObject miniCamera;
    [SerializeField] GameObject fogParticle;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        miniCamera = GameObject.Find("Mini Camera");
    }

    private void Update()
    {
        UseItem();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("ItemBox"))
        {
            Destroy(col.gameObject);
            RandomItem();
            ChangeSprite();
        }
    }

    private void RandomItem()
    {
        int index = Random.Range(0, sprites.Length);
        //MyItems배열 체크
        switch (ItemCnt)        //보유 아이템 개수
        {
            case 0:
                MyItems[0] = index;
                ItemCnt++;
                break;

            case 1:
                MyItems[1] = index;
                ItemCnt++;
                break;
            case 2:
                MyItems[0] = MyItems[1];
                MyItems[1] = index;
                break;
        }
    }

    private void ChangeSprite()
    {
        for (int i = 0; i < ItemCnt; ++i)
        {
            if (MyItems[i] != null)
            {
                Sprite select = sprites[MyItems[i]];
                images[i].sprite = select;
            }
        }
    }

    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector3 fogPosition = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z + 3);

            GameObject fog = Instantiate(fogParticle, fogPosition, mainCamera.transform.rotation, mainCamera.transform);
            Destroy(fog, 20f);


            Vector3 miniFogPosition = new Vector3(miniCamera.transform.position.x,
                miniCamera.transform.position.y - 7,
                miniCamera.transform.position.z);

            GameObject miniFog = Instantiate(fogParticle, miniFogPosition, miniCamera.transform.rotation, miniCamera.transform);
            Destroy(miniFog, 20f);
            
        }
    }

    //StartCoroutine(LateCall());
    //IEnumerator LateCall()
    //{

    //    yield return new WaitForSeconds(20);

    //    miniMap.SetActive(true);
    //    //Do Function here...
    //}

}

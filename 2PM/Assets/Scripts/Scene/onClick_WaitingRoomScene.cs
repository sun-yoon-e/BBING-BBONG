using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class onClick_WaitingRoomScene : MonoBehaviour
{
    public int playerCnt = -1;
    
    public Text roomName;
    public Text[] PlayerID;
    public Image[] AIImages;
    public GameObject[] PlayerObjs;

    private bool[] PlayerList = new bool[4];
    private int maxPlayer = 4;
    
    private GameClient gameClient = GameClient.Instance;
    
    private void Start()
    {
        Debug.Log("waitingRoomScene.start()");
        gameClient.OnGameStateChanged += GameStateChanged;
        gameClient.OnRoomInfo += OnRoomInfo;
        gameClient.OnRoomNewPlayer += OnRoomNewPlayer;
        gameClient.OnAddAI += OnAddAI;
        gameClient.OnRemoveAI += OnRemoveAI;

        foreach (var image in AIImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var obj in PlayerObjs)
        {
            obj.gameObject.SetActive(false);
        }
        
        gameClient.RoomInfo(-1);
    }

    private void Update()
    {
        gameClient.RoomInfo(-1);
        for (int i = 0; i < PlayerList.Length; i++)
        {
            if (PlayerList[i])
            {
                PlayerObjs[i].SetActive(true);
            }
            else
            {
                PlayerObjs[i].SetActive(false);
            }
        }
    }

    private void OnRoomNewPlayer(object sender, RoomPlayer e)
    {
        if (e.nick1 != null)
        {
            if (PlayerID[0] != null)
            {
                if (e.nick1 == "")
                {
                    PlayerList[0] = false;
                }
                else
                {
                    PlayerList[0] = true;
                }
                PlayerID[0].text = e.nick1;
            }
        }

        if (e.nick2 != null)
        {
            if (PlayerID[1] != null)
            {
                if (e.nick2 == "")
                {
                    PlayerList[1] = false;
                }
                else
                {
                    PlayerList[1] = true;
                }
                PlayerID[1].text = e.nick2;
            }
        }
        
        if (e.nick3 != null)
        {
            if (PlayerID[2] != null)
            {
                if (e.nick3 == "")
                {
                    PlayerList[2] = false;
                }
                else
                {
                    PlayerList[2] = true;
                }
                PlayerID[2].text = e.nick3;
            }
        }
        
        if (e.nick4 != null)
        {
            if (PlayerID[3] != null)
            {
                if (e.nick4 == "")
                {
                    PlayerList[3] = false;
                }
                else
                {
                    PlayerList[3] = true;
                }
                PlayerID[3].text = e.nick4;
            }
        }
        gameClient.RoomInfo(-1);
    }

    private void OnRoomInfo(object sender, RoomInfo e)
    {
        playerCnt = e.PlayerNum;
        if (roomName != null)
        {
            roomName.text = e.RoomName;
        }
        //Debug.Log("roomCount : " + roomCount);
        //Debug.Log("roomName : " + roomName.text);
        
        if (PlayerID[0] != null && PlayerID[0].text == "AI(1)")
            AIImages[0].gameObject.SetActive(true);
        if (PlayerID[1] != null && PlayerID[1].text == "AI(2)")
            AIImages[1].gameObject.SetActive(true);
        if (PlayerID[2] != null && PlayerID[2].text == "AI(3)")
            AIImages[2].gameObject.SetActive(true);
        if (PlayerID[3] != null && PlayerID[3].text == "AI(4)")
            AIImages[3].gameObject.SetActive(true);
    }

    private void GameStateChanged(object sender, GameStateChangedEventArgs e)
    {
        if (e.gameState)
            SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
    }

    public void ExitRoom_Btn_Clicked()
    {
        gameClient.ExitRoom();
        SceneManager.LoadScene("Scenes/LobbyScene", LoadSceneMode.Single);
    }

    public void GameStart_Btn_Clicked()
    {
        if (playerCnt == maxPlayer)
        {
            gameClient.StartGame();
        }
        //SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
    }

    public void AddAI_Btn_Clicked()
    {
        if (playerCnt < maxPlayer)
        {
            gameClient.AddAI();
        }
    }

    public void RemoveAI_Btn_Clicked()
    {
        gameClient.RemoveAI();
    }

    private void OnAddAI(object sender, AIMessageEventArgs e)
    {
        if (e.ID < 5 && AIImages[e.ID - 1] != null)
        {
            switch (e.ID)
            {
                case 1:
                    AIImages[0].gameObject.SetActive(true);
                    break;
                case 2:
                    AIImages[1].gameObject.SetActive(true);
                    break;
                case 3:
                    AIImages[2].gameObject.SetActive(true);
                    break;
                case 4:
                    AIImages[3].gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnRemoveAI(object sender, AIMessageEventArgs e)
    {
        if (e.ID > 0 && AIImages[e.ID - 1] != null)
        {
            switch (e.ID)
            {
                case 1:
                    AIImages[0].gameObject.SetActive(false);
                    break;
                case 2:
                    AIImages[1].gameObject.SetActive(false);
                    break;
                case 3:
                    AIImages[2].gameObject.SetActive(false);
                    break;
                case 4:
                    AIImages[3].gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    // 게임종료 버튼 클릭
    public void ExitGame_btn_Clicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class onClick_LobbyScene: MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public GameObject CreateRoomMenu;
    public InputField RoomName;
    public InputField SendMessage;
    public Text ReceiveMessage;

    private void Start()
    {
        gameClient.OnReceivedMessage += ReceiveMsgResult;
    }

    private void ReceiveMsgResult(object sender, ReceiveMessageEventArgs e)
    {
        ReceiveMessage.text = e.msg;
    }

    // 방 입장 버튼 클릭
    public void EnterRoom_btn_Clicked()
    {
    }

    // 방 생성 버튼 클릭
    public void CreateRoom_btn_Clicked()
    {
        CreateRoomMenu.SetActive(true);
    }
    
    // 이전 리스트 버튼 클릭
    public void Left_btn_Clicked()
    {
    }
    
    // 다음 리스트 버튼 클릭
    public void Right_btn_Clicked()
    {
    }
    
    // 리스트 새로고침 버튼 클릭
    public void Refresh_btn_Clicked()
    {
    }

    public void SendChatMessage()
    {
        gameClient.SendMessage(SendMessage.text);
        SendMessage.text = "";
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
    
    // 방 생성 확인 버튼 클릭
    public void Ok_btn_Clicked()
    {
        gameClient.MakeRoom(RoomName.text);

        RoomName.text = "";
        CreateRoomMenu.SetActive(false);

        SceneManager.LoadScene("Scenes/WaitingRoomScene", LoadSceneMode.Single);
    }

    // 방 생성 취소 버튼 클릭
    public void Cancel_btn_Clicked()
    {
        RoomName.text = "";
        CreateRoomMenu.SetActive(false);
    }
}

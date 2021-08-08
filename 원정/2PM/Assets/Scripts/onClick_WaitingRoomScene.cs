using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class onClick_WaitingRoomScene: MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;
    public InputField SendMessage;
    public Text ReceiveMessage;

    private void Start()
    {
        gameClient.OnReceivedMessage += ReceiveMsgResult;
    }

    public void SendChatMessage()
    {
        gameClient.SendMessage(SendMessage.text);
        SendMessage.text = "";
    }

    private void ReceiveMsgResult(object sender, ReceiveMessageEventArgs e)
    {
        ReceiveMessage.text = e.msg;
    }

    public void ExitRoom_Btn_Clicked()
    {
        gameClient.ExitRoom();
        SceneManager.LoadScene("Scenes/LobbyScene", LoadSceneMode.Single);
    }

    public void GameStart_Btn_Clicked()
    {
        SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
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

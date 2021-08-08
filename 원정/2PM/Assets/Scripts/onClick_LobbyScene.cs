using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class onClick_LobbyScene: MonoBehaviour
{
    private int maxRoom = 6;
    private int RoomN = 0;

    public GameObject CreateRoomMenu;
    public GameObject InfoMenu;
    public Text InfoText;

    public InputField RoomName;

    public Text num;
    public Text name;
    public Text count;

    List<Room> RoomList = new List<Room>();

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

    // 게임종료 버튼 클릭
    public void ExitGame_btn_Clicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit()
#endif
    }

    public void CreateRoom(string text)
    {
        if (RoomList.Count >= maxRoom)
        {
            InfoMenu.SetActive(true);
            InfoText.text = "방 생성 개수를 초과하였습니다.";
        }

        Room newRoom = new Room();
        newRoom.RoomName = text;
        newRoom.RoomNum = ++RoomN;
        newRoom.RoomCount = 1;

        RoomList.Add(newRoom);
    }

    // 방 생성 확인 버튼 클릭
    public void Ok_btn_Clicked()
    {
        CreateRoom(RoomName.text);
        RoomName.text = "";
        CreateRoomMenu.SetActive(false);

        num.text = RoomList[RoomN - 1].RoomNum.ToString();
        name.text = RoomList[RoomN - 1].RoomName;
        count.text = RoomList[RoomN - 1].RoomCount.ToString();

        SceneManager.LoadScene("Scenes/WaitingRoomScene", LoadSceneMode.Single);
    }
    
    // 방 생성 취소 버튼 클릭
    public void Cancel_btn_Clicked()
    {
        RoomName.text = "";
        CreateRoomMenu.SetActive(false);
    }

    public void Return_btn_Clicked()
    {
        InfoMenu.SetActive(false);
    }
}

[System.Serializable]
public class Room
{
    public int RoomNum;
    public string RoomName;
    public int RoomCount;
}
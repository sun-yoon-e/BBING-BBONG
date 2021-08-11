﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onClick_LobbyScene: MonoBehaviour
{
    public GameObject CreateRoomMenu;
    public InputField RoomName;

    public Button[] RoomBtn;
    public Button PreviosBtn, NextBtn;
    
    public class RoomInfo
    {
        public int Num;
        public string Name;
        public int PlayerCount;
    }
    List<RoomInfo> roomList = new List<RoomInfo>();
    
    private int currentPage = 1, maxPage, multiple;

    private void Start()
    {
        RoomListrenewal();
    }

    // 이전 리스트 버튼 클릭
    public void Previous_btn_Clicked()
    {
        --currentPage;
        RoomListrenewal();
    }
    
    // 다음 리스트 버튼 클릭
    public void Next_btn_Clicked()
    {
        ++currentPage;
        RoomListrenewal();
    }
    
    // 리스트 새로고침 버튼 클릭
    public void Refresh_btn_Clicked()
    {
        RoomListrenewal();
    }

    //리스트 갱신
    void RoomListrenewal()
    {
        //최대 페이지
        maxPage = (roomList.Count % RoomBtn.Length == 0)
            ? roomList.Count / RoomBtn.Length
            : roomList.Count / RoomBtn.Length + 1;
        
        //이전, 다음 버튼
        PreviosBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;
        
        //페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * RoomBtn.Length;
        for (int i = 0; i < RoomBtn.Length; i++)
        {
            RoomBtn[i].interactable = (multiple + i < roomList.Count) ? true : false;
            //방 번호
            RoomBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].Num + "" : "";
            //방 이름
            RoomBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].Name : "";
            //인원
            RoomBtn[i].transform.GetChild(2).GetComponent<Text>().text = (multiple + i < roomList.Count) ? roomList[multiple + i].PlayerCount + "/4" : "";
        }
    }
    
    // 방 생성 버튼 클릭
    public void CreateRoom_btn_Clicked()
    {
        CreateRoomMenu.SetActive(true);
    }

    void changeValue(List<RoomInfo> inList, int index)
    {
        RoomInfo tmp = inList[index];
        tmp.Num = index + 1;
        tmp.Name = RoomName.text;
       //tmp.PlayerCount = 
        inList[index] = tmp;
    }
    
    // 방 클릭(방 입장)
    public void RoomBtn_Clicked()
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
    
    // 방 생성 확인 버튼 클릭
    public void Ok_btn_Clicked()
    {
        RoomInfo tmp = new RoomInfo();
        roomList.Add(tmp);
        changeValue(roomList, roomList.Count - 1);
        RoomListrenewal();
        
        RoomName.text = "";
        CreateRoomMenu.SetActive(false);
    }
    
    // 방 생성 취소 버튼 클릭
    public void Cancel_btn_Clicked()
    {
        RoomName.text = "";
        CreateRoomMenu.SetActive(false);
    }
}

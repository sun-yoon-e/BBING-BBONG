using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class onClick_LoginMenu : MonoBehaviour
{
    // 정상 상태. 팝업의 확인 버튼을 눌러도 아무것도 하지 않습니다.
    private const int POPUP_OK = 0;
    // 오류 상태. 팝업의 확인 버튼을 누르면 게임을 종료합니다. 
    private const int POPUP_ERROR = 1;
    
    private GameClient gameClient = GameClient.Instance;
    
    // 팝업 상태를 정의합니다. 오류가 발생해서 팝업이 뜬 거면 게임을 종료하도록 합니다. 
    private int popupStatus = 0;

    public GameObject LoginScene;
    public GameObject LoginMenu;
    public GameObject SignupMenu;
    public GameObject SignupPopup;
    public InputField Login_id;
    public InputField Login_pw;
    public InputField Signup_id;
    public InputField Signup_pw;
    public Text InformationText;
    
    // 회원가입 버튼 클릭
    public void Signup_btn_Clicked()
    {
        Login_id.text = "";
        Login_pw.text = "";
        LoginMenu.SetActive(false);
        SignupMenu.SetActive(true);
    }

    // 취소 버튼 클릭
    public void SignUp_back_Clicked()
    {
        Signup_id.text = "";
        Signup_pw.text = "";
        SignupMenu.SetActive(false);
        LoginMenu.SetActive(true);
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

    public void Popup_btn_Clicked()
    {
        if (popupStatus == POPUP_ERROR)
        {
            ExitGame_btn_Clicked();
        } else if (popupStatus == POPUP_OK)
        {
            SignupPopup.SetActive(false);
        }
    }

    public void Login_btn_Clicked()
    {
        if (Login_id.text == "" || Login_pw.text == "")
        {
            InformationText.text = "ID 또는 비밀번호가 비어있습니다";
            SignupPopup.SetActive(true);
            return;
        }

        gameClient.Login(Login_id.text, Login_pw.text);
    }

    public void Signup_OK_btn_Clicked()
    {
        if (Signup_id.text == "" || Signup_pw.text == "")
        {
            InformationText.text = "ID 또는 비밀번호가 비어있습니다";
            SignupPopup.SetActive(true);
            return;
        }
        
        gameClient.SignUp(Signup_id.text, Signup_pw.text);
    }

    private void Start()
    {
        Application.runInBackground = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Login_id.text = "";
        Login_pw.text = "";
        SignupMenu.SetActive(false);
        LoginMenu.SetActive(true);

        // GameClient에서 소켓 오류가 발생하면, 그 이벤트를 받아서 처리하도록 합니다.
        gameClient.OnSocketError += SocketError;
        gameClient.OnLogin += LoginResult;
        gameClient.OnSignup += SignupResult;
        if (!gameClient.IsConnect()) {
            if(!File.Exists("NetworkAddress.txt"))
            {
                //string[] lines = { "127.0.0.1", "13531" };
                string[] lines = { "14.38.227.223", "13531" };
                File.WriteAllLines("NetworkAddress.txt", lines);
            }

            string[] readLines = File.ReadAllLines("NetworkAddress.txt");
            gameClient.ConnectServer(readLines[0], int.Parse(readLines[1]));

            //gameClient.ConnectServer(GameClient.SERVER_IP, GameClient.SERVER_PORT);
        }
    }

    // GameClient에서 소켓 오류가 발생하면 이 메소드가 실행됩니다.
    private void SocketError(object sender, EventArgs e)
    {
        InformationText.text = "서버에 접속할 수 없습니다";
        
        // 팝업 활성화 후 "서버에 연결할 수 없습니다" 메시지를 띄웁니다.
        SignupPopup.SetActive(true);
        // 뒤에 있는건 전부 숨깁니다
        LoginMenu.SetActive(false);
        SignupMenu.SetActive(false);
        
        // 팝업에서 '확인' 버튼을 누르면 종료하도록 합니다.
        popupStatus = POPUP_ERROR;
    }

    private void LoginResult(object sender, LoginEventArgs e)
    {
        //Debug.Log("LoginResult()");
        //Debug.Log(e.success);
        if (e.success)
        {
            GameClient.Instance.client_nick = Login_id.text;
            // TODO: 다음 scene으로 넘어감
            SceneManager.LoadScene("Scenes/LobbyScene", LoadSceneMode.Single);
        }
        else if(InformationText!=null)
        {
            InformationText.text = "로그인에 실패했습니다.";
            SignupPopup.SetActive(true);
        }
    }

    private void SignupResult(object sender, SignupEventArgs e)
    {
        if (e.success)
        {
            InformationText.text = "회원가입에 성공했습니다.";
            SignupPopup.SetActive(true);
            LoginMenu.SetActive(true);
            SignupMenu.SetActive(false);
        }
        else
        {
            InformationText.text = "회원가입에 실패했습니다.";
            SignupPopup.SetActive(true);
        }
    }

    private void Update()
    {
        // 입력창 Tab키 이동
        if (Login_id.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Login_pw.Select();
            }
        }
        if (Login_pw.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Login_id.Select();
            }
        }
        if (Signup_id.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Signup_pw.Select();
            }
        }
        if (Signup_pw.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Signup_id.Select();
            }
        }
    }
}
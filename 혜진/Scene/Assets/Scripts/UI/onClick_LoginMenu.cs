using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onClick_LoginMenu : MonoBehaviour
{
    public GameObject LoginMenu;
    public GameObject SignupMenu;
    public InputField Login_id;
    public InputField Login_pw;
    public InputField Signup_id;
    public InputField Signup_pw;

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
            Application.Quit()
        #endif
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
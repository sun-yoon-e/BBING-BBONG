using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public string username;

    public int maxMsg = 30;

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public ScrollRect chatScroll;

    public Color playerMessage, info;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    void Start()
    {
    }

    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessgeToChat(username + " : " + chatBox.text, Message.MessageType.playerMessge);
                chatBox.text = "";
            }
        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }

        if (!chatBox.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessgeToChat("Space", Message.MessageType.info);
            }
        }
        
    }

    void ScrollDelay() => chatScroll.verticalScrollbar.value = 0;

    public void SendMessgeToChat(string text, Message.MessageType msgType)
    {
        if (messageList.Count >= maxMsg)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMsg = new Message();
        newMsg.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMsg.textObject = newText.GetComponent<Text>();
        newMsg.textObject.text = newMsg.text;
        newMsg.textObject.color = MsgTypeColor(msgType);

        messageList.Add(newMsg);

        Invoke("ScrollDelay", 0.03f);
    }

    Color MsgTypeColor(Message.MessageType msgType)
    {
        Color color = info;

        switch (msgType)
        {
            case Message.MessageType.playerMessge:
                color = playerMessage;
                break;

        }

        return color;
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType msgType;

    public enum MessageType
    {
        playerMessge,
        info,
    }
}

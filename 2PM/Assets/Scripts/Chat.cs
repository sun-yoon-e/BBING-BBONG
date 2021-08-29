using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public string username;

    public int maxMsg = 30;

    public GameObject chatPanel, textObject;
    public InputField chatBox;

    public Color playerMessage, info;

    private string[] split_text;

    private string[] colorList = new string[] { "<color=#d21404>", "<color=#ffd700>", "<color=#03ac13>", "<color=#0a75ad>" };

    [SerializeField]
    List<Message> messageList = new List<Message>();

    void Start()
    {
        gameClient.OnReceivedMessage += ReceiveMsgResult;
    }

    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string msg = chatBox.text;
                gameClient.SendMessage(msg);
                //SendMessgeToChat(msg, Message.MessageType.playerMessge);
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

    private void ReceiveMsgResult(object sender, ReceiveMessageEventArgs e)
    {
        SendMessgeToChat(e.msg, Message.MessageType.playerMessge);
    }

    public void SendMessgeToChat(string text, Message.MessageType msgType)
    {
        if (messageList.Count >= maxMsg)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMsg = new Message();
        split_text = text.Split(':');
        if (split_text[0] == gameClient.client_nick1)
        {
            newMsg.text = colorList[0] + split_text[0] + "</color>" + "   " + "<color=#ffffff>" + split_text[1] + "</color>";
        }
        if (split_text[0] == gameClient.client_nick2)
        {
            newMsg.text = colorList[1] + split_text[0] + "</color>" + "   " + "<color=#ffffff>" + split_text[1] + "</color>";
        }
        if (split_text[0] == gameClient.client_nick3)
        {
            newMsg.text = colorList[2] + split_text[0] + "</color>" + "   " + "<color=#ffffff>" + split_text[1] + "</color>";
        }
        if (split_text[0] == gameClient.client_nick4)
        {
            newMsg.text = colorList[3] + split_text[0] + "</color>" + "   " + "<color=#ffffff>" + split_text[1] + "</color>";
        }

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMsg.textObject = newText.GetComponent<Text>();
        newMsg.textObject.text = newMsg.text;
        newMsg.textObject.color = MsgTypeColor(msgType);

        messageList.Add(newMsg);
    }

    Color MsgTypeColor(Message.MessageType msgType)
    {
        Color color = info;

        switch (msgType)
        {
            case Message.MessageType.playerMessge:
                color = playerMessage;
                break;
            case Message.MessageType.info:
                color = info;
                break;
            default:
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
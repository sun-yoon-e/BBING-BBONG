using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    void Awake() => instance = this;

    public InputField SendInput;
    public RectTransform ChatContent;
    public Text ChatText;
    public ScrollRect ChatScrollRect;

    public void ShowMessage(string data)
    {
        ChatText.text += ChatText.text == "" ? data : "\n" + data;

        Fit(ChatText.GetComponent<RectTransform>());
        Fit(ChatContent);
        Invoke("ScrollDelay", 0.03f);
    }

    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    void ScrollDelay() => ChatScrollRect.verticalScrollbar.value = 0;
}

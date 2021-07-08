using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WebSocketScenesController : MonoBehaviour
{
    
    private StringBuilder _stringBuilder;

    //顯示狀態及文字
    Text SocketMessageText;

    //文字輸入框
    InputField MsgInputField;

    //送出文字按鈕
    Button SendMsgButton;

    ScrollRect ScrollControl;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();
    }

    void Start()
    {

        if (GameObject.Find("/Canvas/Content/Image/Message").TryGetComponent<Text>(out Text _socketMessageText))
        {
            SocketMessageText = _socketMessageText;
        }

        if (GameObject.Find("/Canvas/Content/InputField").TryGetComponent<InputField>(out InputField _msgInputField))
        {
            MsgInputField = _msgInputField;
            MsgInputField.onValueChanged.AddListener(delegate { InputFieldOnValueChanged(); });
        }

        if (GameObject.Find("/Canvas/Content/ButtonRow/SendMessage").TryGetComponent<Button>(out Button _sendMsgButton))
        {
            SendMsgButton = _sendMsgButton;
        }

        if (GameObject.Find("/Canvas/Content/Image").TryGetComponent<ScrollRect>(out ScrollRect _scrollControl))
        {
            ScrollControl = _scrollControl;
        }
        
        _stringBuilder = new StringBuilder();

        SocketMessageText.text = _stringBuilder.ToString();
        
        WebSocketController._instance.WebSocketReturnMsg += WebSocketReturnMsgListener;

        SendMsgButton.interactable = false;
        
    }

    /// <summary>
    /// 文字輸入框輸入文字時
    /// </summary>
    public void InputFieldOnValueChanged()
    {
        if (MsgInputField.text.Length == 0)
        {
            SendMsgButton.interactable = false;
        }
        else
        {
            SendMsgButton.interactable = true;
        }
        Debug.Log("InputFieldOnValueChanged: " + MsgInputField.text + "\n");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    /// <summary>
    /// 點擊送出訊息按鈕
    /// </summary>
    public void SendMessageButtonPressed()
    {
        WebSocketController._instance.SocketSendMsg(MsgInputField.text);
        _stringBuilder.Append("Send: " + MsgInputField.text + "\n");
        SocketMessageText.text = _stringBuilder.ToString();

        MsgInputField.text = "";

        StartCoroutine(nameof(ScrollToBottom));
    }
    
    /// <summary>
    /// 接收Websockett訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    void WebSocketReturnMsgListener(object sender, string msg)
    {
        Debug.Log("接收Websockett訊息: " + msg);
        _stringBuilder.Append(msg);

        SocketMessageText.text = _stringBuilder.ToString();

        StartCoroutine(nameof(ScrollToBottom));
    }


    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        ScrollControl.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// 點擊關閉socket按鈕
    /// </summary>
    public void CloseSocketButtonPressed()
    {
        WebSocketController._instance.CloseSocket();
    }

    /// <summary>
    /// 點擊socket連線按鈕
    /// </summary>
    public void SocketConnectButtonPressed()
    {
        WebSocketController._instance.CreateSocket();
        _stringBuilder.Append("開始連接socket" + "\n");
    }
}

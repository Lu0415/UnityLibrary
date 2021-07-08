using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketController : MonoBehaviour
{
    
    public event EventHandler<string> WebSocketReturnMsg;

    private string _socketEchoURL = "wss://echo.websocket.org";

    private WebSocket _ws;

    private bool _wsConnectState;

    //靜態單例
    public static WebSocketController _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);//儲存功能節點
        }
        else if (this != _instance)
        {
            Destroy(gameObject);
        }

        _wsConnectState = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 建立Socket連線
    /// </summary>
    public void CreateSocket()
    {
        
        _ws = new WebSocket(_socketEchoURL);
        _ws.WaitTime = new TimeSpan(0, 3, 0);
        _ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        _ws.OnOpen += SocketOpen;
        _ws.OnMessage += SocketOnMessage;
        //_ws.OnError += SocketOnError;
        _ws.OnClose += SocketOnClose;
        _ws.ConnectAsync();
        Debug.Log("開始連接");
        
    }

    /// <summary>
    /// 成功連線
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SocketOpen(object sender, EventArgs e)
    {
        _wsConnectState = true;
        Debug.Log("成功連線 SocketOpen \n");
        if (WebSocketReturnMsg != null) WebSocketReturnMsg(this, "成功連線 SocketOpen");
        _ws.EmitOnPing = true;
    }

    /// <summary>
    /// 接收Websockett訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">訊息</param>
    void SocketOnMessage(object sender, MessageEventArgs e)
    {
        if (e.IsPing)
        {
            Debug.Log("接收Websockett訊息: Ping");
            if (WebSocketReturnMsg != null) WebSocketReturnMsg(this, "Echo: Ping" + "\n");
        }
        else
        {
            Debug.Log("接收Websockett訊息: " + e.Data);
            if (WebSocketReturnMsg != null) WebSocketReturnMsg(this, "Echo: " + e.Data + "\n");
        }
    }

    /// <summary>
    /// Socket送出訊息
    /// </summary>
    /// <param name="msg"></param>
    public void SocketSendMsg(string msg)
    {
        _ws.Send(msg);
        
    }

    /// <summary>
    /// 關閉socket
    /// </summary>
    public void CloseSocket()
    {
        _ws.Close();
    }

    /// <summary>
    /// 接收關閉
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void SocketOnClose(object sender, CloseEventArgs e)
    {
        _wsConnectState = false;
        _ws.EmitOnPing = false;
        _ws.OnOpen -= SocketOpen;
        _ws.OnMessage -= SocketOnMessage;
        //_ws.OnError -= SocketOnError;
        _ws.OnClose -= SocketOnClose;
        Debug.Log("SocketClose 關閉拉");
        if (WebSocketReturnMsg != null) WebSocketReturnMsg(this, "SocketClose 關閉拉" + "\n");
    }

    /// <summary>
    /// 接收錯誤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void SocketOnError(object sender, CloseEventArgs e)
    {
        Debug.Log("SocketError 錯誤拉: " + sender);
        if (WebSocketReturnMsg != null) WebSocketReturnMsg(this, "SocketError 錯誤拉: " + sender + "\n");
    }
}

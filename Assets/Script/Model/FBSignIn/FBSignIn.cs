using System.Collections.Generic;
using System.Text;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FBSignIn : MonoBehaviour
{
    private bool isFacebookSignIn = false;
    private string getAccessToken = "";
    private string getEmail = "";

    //序列化狀態
    Text FBMessageText;

    //反序列化狀態
    Button FBLoginButton;

    //序列化內容
    Button FBLogoutButton;

    /// <summary>
    /// 初始化 Facebook SDK
    /// </summary>
    void Awake()
    {

        if (GameObject.Find("/Canvas/Content/Image/Message").TryGetComponent<Text>(out Text textSerialization))
        {
            FBMessageText = textSerialization;
        }

        if (GameObject.Find("/Canvas/Content/LogIn").TryGetComponent<Button>(out Button textFBLoginButton))
        {
            FBLoginButton = textFBLoginButton;
        }

        if (GameObject.Find("/Canvas/Content/LogOut").TryGetComponent<Button>(out Button textFBLogoutButton))
        {
            FBLogoutButton = textFBLogoutButton;
        }


        FBLoginButton.interactable = false;
        FBLogoutButton.interactable = false;

        if (FB.IsInitialized)
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        else
        {
            // Initialize the Facebook SDK
            FB.Init(OnInitComplete, OnHideUnity);
        }
    }

    /// <summary>
    /// FB 登入回傳狀態
    /// </summary>
    protected void HandleResult(IResult result)
    {
        var stringBuilder = new StringBuilder();

        getAccessToken = "";
        getEmail = "";

        if (result == null)
        {
            isFacebookSignIn = false;
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            //回傳錯誤
            isFacebookSignIn = false;
            Debug.Log("Error Response:\n" + result.Error);
            stringBuilder.AppendLine("Error Response:\n" + result.Error);
        }
        else if (result.Cancelled)
        {
            //取消
            isFacebookSignIn = false;
            Debug.Log("Cancelled Response:\n" + result.RawResult);
            stringBuilder.AppendLine("Cancelled Response:\n" + result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            //FB 登入成功
            isFacebookSignIn = true;
            Debug.Log("Success Response:\n" + result.RawResult);
            getAccessToken = (string)result.ResultDictionary["access_token"];
            stringBuilder.AppendLine("Success getAccessToken:\n" + getAccessToken);
            //如果Email沒有授權獲認證則填空
            if (result.ResultDictionary.ContainsKey("email"))
            {
                getEmail = (string)result.ResultDictionary["email"];
            }
            
            stringBuilder.AppendLine("Success getEmail:\n" + getEmail);

            FBLoginButton.interactable = false;
            FBLogoutButton.interactable = true;
        }
        else
        {
            Debug.Log("Empty Response\n");
        }
        FBMessageText.text = stringBuilder.ToString();
        Debug.Log("FB 登入回傳狀態 result: " + result.ToString());
        
        Debug.Log(isFacebookSignIn);
    }

    /// <summary>
    /// FB 初始化完畢
    /// </summary>
    private void OnInitComplete()
    {
        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);

        Debug.Log("FB 初始化完畢 logMessage: " + logMessage);

        if (AccessToken.CurrentAccessToken != null)
        {
            Debug.Log("FB 初始化完畢 CurrentAccessToken: " + AccessToken.CurrentAccessToken);
            FBMessageText.text = "FB 初始化完畢 CurrentAccessToken: " + AccessToken.CurrentAccessToken;
            isFacebookSignIn = true;

            FBLoginButton.interactable = false;
            FBLogoutButton.interactable = true;
        }
        else
        {
            isFacebookSignIn = false;

            FBLoginButton.interactable = true;
            FBLogoutButton.interactable = false;
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        FBMessageText.text = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
        Debug.Log(string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown));
    }

    /// <summary>
    /// FB 登入按鈕點擊事件
    /// </summary>
    public void SignInWithFacebookButtonPressed()
    {
        FBMessageText.text = "FB 登入按鈕點擊事件";

        if (FB.IsInitialized) //已經初始化了
        {
            //顯示 Facebook 登入中 的提示框 
            CallFBLogin();
        }
    }

    /// <summary>
    /// FB 登入方法執行
    /// </summary>
    private void CallFBLogin()
    {
        FBMessageText.text = "FB 登入方法執行";
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    }
    
    
    /// <summary>
    /// FB 登出方法執行
    /// </summary>
    public void CallFBLogout()
    {
        FBMessageText.text = "FB 登出方法執行";
        FB.LogOut();
        isFacebookSignIn = false;

        FBLoginButton.interactable = true;
        FBLogoutButton.interactable = false;
    }

}

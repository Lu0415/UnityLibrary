using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FBSignIn : MonoBehaviour
{
    private bool isFacebookSignIn = false;
    private string getAccessToken = "";
    private string getEmail = "";

    /// <summary>
    /// 初始化 Facebook SDK
    /// </summary>
    void Awake()
    {
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
        if (result == null)
        {
            isFacebookSignIn = false;
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            isFacebookSignIn = false;
            Debug.Log("Error Response:\n" + result.Error);
        }
        else if (result.Cancelled)
        {
            isFacebookSignIn = false;
            Debug.Log("Cancelled Response:\n" + result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            //FB 登入成功
            isFacebookSignIn = true;
            Debug.Log("Success Response:\n" + result.RawResult);

            getAccessToken = (string)result.ResultDictionary["access_token"];

            //如果Email沒有授權獲認證則填空
            try
            {
                getEmail = (string)result.ResultDictionary["email"];
            }
            catch
            {
                getEmail = "";
            }

            //送出登入資訊
            //AotHelper.EnsureType<FacebookLogin>();
            //FacebookLogin data = new FacebookLogin();
            //data.AccessToken = getAccessToken;
            //data.Email = getEmail;
            //data.AppVersion = Application.version;
            //data.UserAgent = GameControl._instance.GetUserAgent();
            //Debug.Log("UserAgent: " + data.UserAgent);
            //Debug.Log("FacebookLogin: " + JsonConvert.SerializeObject(data));
            //GameControl _GameControl = GameControl._instance.GetComponent<GameControl>();
            //_GameControl.SendEncryptedMessage(BaseConstants.EVENT_FACEBOOK_LOGIN, data);

        }
        else
        {
            Debug.Log("Empty Response\n");
        }
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
            isFacebookSignIn = true;
        }
        else
        {
            isFacebookSignIn = false;
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log(string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown));
    }

    /// <summary>
    /// FB 登入按鈕點擊事件
    /// </summary>
    public void SignInWithFacebookButtonPressed()
    {
        CallFBLogout();

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
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    }
    /*
    private void CallFBLoginForPublish()
    {
        // It is generally good behavior to split asking for read and publish
        // permissions rather than ask for them all at once.
        //
        // In your own game, consider postponing this call until the moment
        // you actually need it.
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, HandleResult);
    }
    */
    /// <summary>
    /// FB 登出方法執行
    /// </summary>
    public void CallFBLogout()
    {
        FB.LogOut();
        isFacebookSignIn = false;
    }

    /// <summary>
    /// 更改按鍵可觸碰與否狀態(FB)
    /// </summary>
    /// <param name="isEnable"></param>
    private void UpdateButtonEnable(bool _isEnable)
    {
        Debug.Log("UpdateButtonEnable: " + _isEnable);
        GetComponent<Button>().enabled = _isEnable;
    }

}

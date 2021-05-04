using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInHandler : MonoBehaviour
{
    //序列化狀態
    Text GoogleMessageText;

    //反序列化狀態
    Button GoogleLoginButton;

    //序列化內容
    Button GoogleLogoutButton;

    private GoogleSignInConfiguration configuration;

    /// <summary>
    /// Google 登入初始化
    /// </summary>
    void Awake()
    {

        if (GameObject.Find("/Canvas/Content/Image/Message").TryGetComponent<Text>(out Text textSerialization))
        {
            GoogleMessageText = textSerialization;
        }

        if (GameObject.Find("/Canvas/Content/LogIn").TryGetComponent<Button>(out Button textGoogleLoginButton))
        {
            GoogleLoginButton = textGoogleLoginButton;
        }

        if (GameObject.Find("/Canvas/Content/LogOut").TryGetComponent<Button>(out Button textGoogleLogoutButton))
        {
            GoogleLogoutButton = textGoogleLogoutButton;
        }


        

        //填入ClientID及可以取得id, token
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = "303413321002-osmliudj71vc5e0lqjd8l6mb137v0clj.apps.googleusercontent.com",
            RequestIdToken = true
        };


        if (GoogleSignIn.Configuration != null)
        {
            GoogleLoginButton.interactable = false;
            GoogleLogoutButton.interactable = false;
        }
        else
        {
            GoogleLoginButton.interactable = true;
            GoogleLogoutButton.interactable = false;
        }


    }

    /// <summary>
    /// Google 登入
    /// </summary>
    public void OnSignIn()
    {
        if (GoogleSignIn.Configuration != null)
        {
            Debug.Log("GoogleSignIn.Configuration != null");
            OnSignOut();

        }
        
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        Debug.Log("Google Calling SignIn");
        GoogleMessageText.text = "Google Calling SignIn";

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    /// <summary>
    /// Google 登出
    /// </summary>
    public void OnSignOut()
    {
        Debug.Log("Google Calling SignOut");
        GoogleMessageText.text = "Google Calling SignOut";
        GoogleSignIn.DefaultInstance.SignOut();

        GoogleLoginButton.interactable = true;
        GoogleLogoutButton.interactable = false;
    }

    /// <summary>
    /// Google 登入資訊回傳
    /// </summary>
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted) //登入錯誤
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                
                if (enumerator.MoveNext()) //錯誤訊息
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                    GoogleMessageText.text = "Got Error 錯誤訊息: " + error.Status + " " + error.Message;
                }
                else //例外狀況
                {
                    Debug.Log("Got Unexpected Exception !?" + task.Exception);
                    GoogleMessageText.text = "Got Unexpected Exception 例外狀況 !? " + task.Exception;
                }
            }
        }
        else if (task.IsCanceled) //取消登入
        {
            Debug.Log("Canceled");
            GoogleMessageText.text = "取消登入";


        }
        else //登入成功
        {
            var stringBuilder = new StringBuilder();
            Debug.Log("Google AuthCode: " + task.Result.AuthCode);
            Debug.Log("Google IdToken: " + task.Result.IdToken);
            Debug.Log("Google UserId: " + task.Result.UserId);

            stringBuilder.Append("---登入成功---\n");
            stringBuilder.Append("Google AuthCode: " + task.Result.AuthCode + "\n");
            stringBuilder.Append("Google IdToken: " + task.Result.IdToken + "\n");
            stringBuilder.Append("Google UserId: " + task.Result.UserId + "\n");

            GoogleMessageText.text = stringBuilder.ToString();

            GoogleLoginButton.interactable = false;
            GoogleLogoutButton.interactable = true;
        }
    }

    /// <summary>
    /// 快速登入(已登入過)
    /// </summary>
    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Google Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }

    /// <summary>
    /// Google Game Servise 登入
    /// </summary>
    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Google Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }
}

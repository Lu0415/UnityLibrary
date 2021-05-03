using System.Collections;
using System.Collections.Generic;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class AppleSignIn : MonoBehaviour
{

    private const string AppleUserIdKey = "AppleUserId";

    private IAppleAuthManager _appleAuthManager;

    public AppleSignInHandler _AppleSignInHandler;

    public Text DebugLogText;

    // Start is called before the first frame update
    void Start()
    {
        // 如果是iOS裝置
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            Debug.Log("是iOS裝置");
            DebugLogText.text = "是iOS裝置";
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // 創建一個Apple身份驗證管理器
            _appleAuthManager = new AppleAuthManager(deserializer);
        }
        else
        {
            Debug.Log("不是iOS裝置");
            DebugLogText.text = "不是iOS裝置";
            GetComponent<Button>().enabled = false;
            GetComponent<Button>().interactable = false;
        }

        InitializeAppleSignIn();
    }

    // Update is called once per frame
    void Update()
    {
        // 更新AppleAuthManager
        if (_appleAuthManager != null)
        {
            _appleAuthManager.Update();
        }
    }

    /// <summary>
    /// 點擊Apple登入按鈕
    /// </summary>
    public void SignInWithAppleButtonPressed()
    {
        Debug.Log("SignInWithAppleButtonPressed");
        DebugLogText.text = "點擊Apple登入按鈕";
        // 如果是iOS裝置
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            SignInWithApple();
            Debug.Log("是iOS裝置");
        }
        else
        {
            Debug.Log("不是iOS裝置");
        }
    }

    /// <summary>
    /// 初始化Apple登錄
    /// </summary>
    private void InitializeAppleSignIn()
    {

        // 檢查當前裝置是否支持使用Apple登錄
        if (_appleAuthManager == null)
        {
            Debug.Log("當前裝置沒有支持使用Apple登錄");
            return;
        }

        // 如果突然收到憑證撤銷通知
        _appleAuthManager.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log("Received revoked callback " + result);
            // 將存儲的用戶ID刪除
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        });

        if (PlayerPrefs.HasKey(AppleUserIdKey)) // 如果有可用的Apple用戶ID，獲取憑證狀態
        {
            var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
            CheckCredentialStatusForUserId(storedAppleUserId);
        }

    }

    /// <summary>
    /// 檢查憑證狀態以獲取UserId
    /// </summary>
    private void CheckCredentialStatusForUserId(string appleUserId)
    {
        // 如果有可用的Apple ID，則檢查憑證狀態
        _appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    // 如果獲得授權，使用該用戶ID登錄
                    case CredentialState.Authorized:
                        //SetupAppleSignIn(appleUserId, null);
                        return;

                    // 如果被撤消或找不到，需要重新登錄Apple
                    // 將存儲的用戶ID刪除
                    case CredentialState.Revoked:
                    case CredentialState.NotFound:
                        //SetupLoginMenuForSignInWithApple();
                        PlayerPrefs.DeleteKey(AppleUserIdKey);
                        return;
                }
            },
            error =>
            {
                // 獲取憑證狀態時出錯
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                DebugLogText.text = "Error authorizationErrorCode: " + authorizationErrorCode.ToString() + " " + error.ToString();
                Debug.Log("Error authorizationErrorCode: " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }

    /// <summary>
    /// 登入Apple
    /// </summary>
    private void SignInWithApple()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        _appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // 如果使用Apple登錄成功，則可獲得帶有用戶ID，Name和Email的憑證
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                SetupAppleSignIn(credential.User, credential);
            },
            error =>
            {
                // Sign in with Apple 失敗
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.Log("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }

    /// <summary>
    /// 設置AppleSignIn驗證資料
    /// </summary>
    /// <param name="appleUserId"></param>
    /// <param name="credential"></param>
    private void SetupAppleSignIn(string appleUserId, ICredential credential)
    {
        _AppleSignInHandler.SetupAppleData(appleUserId, credential, this);
    }

    /// <summary>
    /// AppleSignIn Apple端回傳成功
    /// </summary>
    /// <param name="code"></param>
    /// <param name="token"></param>
    public void CallBackForAppleSignIn(string returnLog)
    {
        DebugLogText.text = "AppleSignIn Apple端回傳成功: " + returnLog;
        Debug.Log("AppleSignIn Apple端回傳成功: " + returnLog);
    }

    /// <summary>
    /// AppleSignIn Apple端回傳失敗
    /// </summary>
    /// <param name="error"></param>
    public void CallBackForAppleSignInError(string error)
    {
        DebugLogText.text = "AppleSignIn Apple端回傳失敗: " + error;
        Debug.Log("AppleSignIn Apple端回傳失敗: " + error);
    }
}
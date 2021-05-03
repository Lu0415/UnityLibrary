using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using System;
using System.Text;
using UnityEngine;


[Serializable]
public class AppleSignInHandler
{
    private string code = ""; //AppleSignIn 的 AuthorizationCode
    private string token = ""; //AppleSignIn 的 IdentityToken

    /// <summary>
    /// AppleSignIn 取得使用者資料
    /// </summary>
    /// <param name="appleUserID"></param>
    /// <param name="receivedCredential"></param>
    /// <param name="appleSignIn"></param>
    public void SetupAppleData(string appleUserID, ICredential receivedCredential, AppleSignIn appleSignIn)
    {
        var stringBuilder = new StringBuilder();

        if (receivedCredential == null)
        {
            // 使用者憑證沒有授權
            stringBuilder.AppendLine("使用者: " + appleUserID + " 憑證沒有授權");
            appleSignIn.CallBackForAppleSignIn(stringBuilder.ToString());
            return;
        }

        var appleIdCredential = receivedCredential as IAppleIDCredential;
        var passwordCredential = receivedCredential as IPasswordCredential;

        if (appleIdCredential != null)
        {
            //回傳AppleSignIn資訊
            stringBuilder.AppendLine("---------------------回傳 AppleSignIn 資訊-----------------------");
            stringBuilder.AppendLine("appleIdCredential.User: " + appleIdCredential.User);
            stringBuilder.AppendLine("Real user status: " + appleIdCredential.RealUserStatus.ToString());

            if (appleIdCredential.State != null)
            {
                stringBuilder.AppendLine("State:  " + appleIdCredential.State);
            }

            //IdentityToken
            if (appleIdCredential.IdentityToken != null)
            {
                var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                token = identityToken;
                stringBuilder.AppendLine("Identity Token (" + appleIdCredential.IdentityToken.Length + " bytes)");
                stringBuilder.AppendLine("Identity Token:  " + identityToken);
            }

            //AuthorizationCode
            if (appleIdCredential.AuthorizationCode != null)
            {
                var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);
                code = authorizationCode;
                stringBuilder.AppendLine("Authorization Code (" + appleIdCredential.AuthorizationCode.Length + " bytes)");
                stringBuilder.AppendLine("Authorization Code:  " + authorizationCode);
            }

            if (appleIdCredential.AuthorizedScopes != null)
            {
                stringBuilder.AppendLine("Authorized Scopes::  " + string.Join(", ", appleIdCredential.AuthorizedScopes));
            }

            //Email: 首次登入時可選擇是否隱藏，若隱藏則為 null
            if (appleIdCredential.Email != null)
            {
                //You can test this again by revoking credentials in Settings
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Email: 首次登入時可選擇是否隱藏，若隱藏則為 null");
                stringBuilder.AppendLine("Email:  " + appleIdCredential.Email);
            }

            if (appleIdCredential.FullName != null)
            {
                //NAME RECEIVED: YOU WILL ONLY SEE THIS ONCE PER SIGN UP. SEND THIS INFORMATION TO YOUR BACKEND!
                //You can test this again by revoking credentials in Settings
                var fullName = appleIdCredential.FullName;
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Name:  " + fullName.ToLocalizedString());
                stringBuilder.AppendLine("Name (Short):  " + fullName.ToLocalizedString(PersonNameFormatterStyle.Short));
                stringBuilder.AppendLine("Name (Medium):  " + fullName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                stringBuilder.AppendLine("Name (Long):  " + fullName.ToLocalizedString(PersonNameFormatterStyle.Long));
                stringBuilder.AppendLine("Name (Abbreviated):  " + fullName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));

                if (appleIdCredential.FullName.PhoneticRepresentation != null)
                {
                    var phoneticName = appleIdCredential.FullName.PhoneticRepresentation;
                    stringBuilder.AppendLine("Name:  " + phoneticName.ToLocalizedString());
                    stringBuilder.AppendLine("Name (Short):  " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Short));
                    stringBuilder.AppendLine("Name (Medium):  " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                    stringBuilder.AppendLine("Name (Long):  " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Long));
                    stringBuilder.AppendLine("Name (Abbreviated):  " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));
                }
            }
            stringBuilder.AppendLine("----------------------------------------------------------------");
            appleSignIn.CallBackForAppleSignIn(stringBuilder.ToString());
        }
        else if (passwordCredential != null)
        {
            stringBuilder.AppendLine("USERNAME/PASSWORD RECEIVED (iCloud?)");
            stringBuilder.AppendLine("Username: " + passwordCredential.User);
            stringBuilder.AppendLine("Password: " + passwordCredential.Password);
            appleSignIn.CallBackForAppleSignInError(stringBuilder.ToString());
        }
        else
        {
            stringBuilder.AppendLine("未知的用戶憑證 " + receivedCredential.User);
            appleSignIn.CallBackForAppleSignInError(stringBuilder.ToString());
        }
    }


}

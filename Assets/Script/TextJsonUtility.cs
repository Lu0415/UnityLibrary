using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Bean;
using UnityEngine;
using UnityEngine.UI;

public class TextJsonUtility : MonoBehaviour
{

    //序列化狀態
    Text SerializationStatus;

    //反序列化狀態
    Text DeserializationStatus;

    //序列化內容
    Text SerializationContent;

    //反序列化內容
    Text DeserializationContent;

    private void Awake()
    {
        if (GameObject.Find("/Canvas/Content/SerializationContent/StatusImage/SerializationStatus").TryGetComponent<Text>(out Text textSerialization))
        {
            SerializationStatus = textSerialization;
        }

        if (GameObject.Find("/Canvas/Content/DeserializationContent/StatusImage/DeserializationStatus").TryGetComponent<Text>(out Text textDeserialization))
        {
            DeserializationStatus = textDeserialization;
        }

        if (GameObject.Find("/Canvas/Content/Serialization/Content").TryGetComponent<Text>(out Text Serialization))
        {
            SerializationContent = Serialization;
        }

        if (GameObject.Find("/Canvas/Content/Deserialization/Content").TryGetComponent<Text>(out Text Deserialization))
        {
            DeserializationContent = Deserialization;
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        var jsonUtilityTestBean = new JsonUtilityTestBean();
        if (Application.platform == RuntimePlatform.Android)
        {
            jsonUtilityTestBean.Name = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            jsonUtilityTestBean.Name = "IOS";
        }
        else
        {
            jsonUtilityTestBean.Name = "Editor";
        }

        //開始序列化
        string json = "無法解析";
        bool isSerializationSuccess = true;
        try
        {
            json = JsonUtility.ToJson(jsonUtilityTestBean);

        }
        catch (Exception e)
        {
            Debug.Log("序列化錯誤");
            isSerializationSuccess = false;
        }

        if (isSerializationSuccess)
        {
            //序列化成功
            //更改狀態
            SetText(SerializationStatus, "成功");
            SetText(SerializationContent, json);
        }
        else
        {
            //序列化失敗
            //更改狀態
            SetText(SerializationStatus, "失敗");
            SetText(SerializationContent, json);
        }



        //開始反序列化
        if (String.IsNullOrEmpty(json)) return;

        JsonUtilityTestBean jsonToBean;
        bool isDeserializationSuccess = true;
        try
        {
            jsonToBean = JsonUtility.FromJson<JsonUtilityTestBean>(json);
        }
        catch (Exception e)
        {
            Debug.Log("反序列化錯誤");
            isDeserializationSuccess = false;
            jsonToBean = new JsonUtilityTestBean();
            jsonToBean.Name = "錯誤無法解析";
        }

        if (isDeserializationSuccess)
        {
            //反序列化成功
            //更改狀態
            SetText(DeserializationStatus, "成功");
            SetText(DeserializationContent, jsonToBean.Name);
        }
        else
        {
            //反序列化失敗
            //更改狀態
            SetText(DeserializationStatus, "失敗");
            SetText(DeserializationContent, jsonToBean.Name);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    //輸出文字
    private void SetText(Text text , string value)
    {
        if(text != null)
        {
            text.text = value;
        }
    }
}

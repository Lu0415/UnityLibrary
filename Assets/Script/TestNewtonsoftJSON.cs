using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Bean;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

public class TestNewtonsoftJSON : MonoBehaviour
{

    private void Awake()
    {
        //https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Fix-AOT-using-AotHelper
        //IOS AOT 需求(如果產生上述網址提到的問題，以下述程式解決)
        //AotHelper.EnsureType<EncryptionJSON>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
        var bean = new EncryptionJSON();
        bean.Foo = "test";
        var json = JsonConvert.SerializeObject(bean);
        Debug.Log("EncryptionJSON " + json);
        
        EncryptionJSON _jsonToBean = JsonConvert.DeserializeObject<EncryptionJSON>(json);
        Debug.Log("_jsonToBean: " + _jsonToBean);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

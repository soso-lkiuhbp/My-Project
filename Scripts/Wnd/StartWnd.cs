using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;

public class StartWnd : BaseWnd
{
    TMP_InputField _ip;
    public override void Initial()
    {
        Button startBtn = SelfTransform.Find("StartBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(OnStartClick);
        SelfTransform.gameObject.AddComponent<StartMono>();
    }

    private void OnStartClick()
    {
        
        if (ConfigManager.Instance.UserData != null)
        {
            Debug.Log("自动登录");
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData.Add("username", ConfigManager.Instance.UserData.username);
            formData.Add("token", ConfigManager.Instance.UserData.token);
            HTTPHandler.Instance.POST("user/auto-login", formData, OnResponse, OnError);
        }
        else
        {
            CloseWnd();
            WndManager.Instance.OpenWnd<AccountWnd>();
        }
    }

    void OnResponse(string msg)
    {
        Debug.Log(msg);
        ServiceResult<UserData> result = JsonConvert.DeserializeObject<ServiceResult<UserData>>(msg);
        switch (result.code)
        {
            case 0:
            case -1:
            case -2:
                {
                    CloseWnd();
                    WndManager.Instance.OpenWnd<AccountWnd>();
                }
                break;
            case 1001:
                {
                    Debug.Log("正式登录成功");
                    ConfigManager.Instance.SaveUserData(result.data[0]);
                    // 根据role启动服务器或客户端
                    if (ConfigManager.Instance.ConfigInfo.role == 0)
                    {
                        // 服务器模式
                        Debug.Log("启动服务器");
                        StartNet.instance.StartServer((ushort)ConfigManager.Instance.ConfigInfo.port);
                        // 服务器直接打开大厅
                        CloseWnd();
                        WndManager.Instance.OpenWnd<LobbyWnd>();
                    }
                    else
                    {
                        // 客户端模式
                        Debug.Log("启动客户端，连接服务器");
                        StartNet.instance.StartClient(ConfigManager.Instance.ConfigInfo.gameserver, (ushort)ConfigManager.Instance.ConfigInfo.port);
                    }
                }
                break;
        }
    }
    void OnError(string msg)
    {
        Debug.Log(msg);
    }
}

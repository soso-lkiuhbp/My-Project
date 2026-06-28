using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AccountWnd : BaseWnd
{
    TMP_InputField _loginUsername;
    TMP_InputField _loginPassword;
    TMP_InputField _registerUsername;
    TMP_InputField _registerCode;
    TMP_InputField _registerNickname;
    TMP_InputField _registerPassword;
    TMP_InputField _registerRepassword;

    GameObject _login;
    GameObject _register;

    TMP_Text _registerTip;

    AccountMono _mono;

    public override void Initial()
    {
        _mono = SelfTransform.gameObject.AddComponent<AccountMono>();
        _login = SelfTransform.Find("Login").gameObject;
        Button loginBtn = SelfTransform.Find("Login/LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        Button registerBtn = SelfTransform.Find("Login/RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OnRegisterClick);
        _loginUsername = SelfTransform.Find("Login/Username").GetComponentInChildren <TMP_InputField>();
        _loginPassword = SelfTransform.Find("Login/Password").GetComponentInChildren<TMP_InputField>();

        _register = SelfTransform.Find("Register").gameObject;
        Button confirmBtn = SelfTransform.Find("Register/ConfirmBtn").GetComponent<Button>();
        confirmBtn.onClick.AddListener(OnConfirmClick);
        Button backBtn = SelfTransform.Find("Register/BackBtn").GetComponent<Button>();
        backBtn.onClick.AddListener(OnBackClick);
        Button sendBtn = SelfTransform.Find("Register/SendBtn").GetComponent<Button>();
        sendBtn.onClick.AddListener(OnSendClick);
        _registerUsername = SelfTransform.Find("Register/Username").GetComponentInChildren<TMP_InputField>();
        _registerCode = SelfTransform.Find("Register/Code").GetComponentInChildren<TMP_InputField>();
        _registerNickname = SelfTransform.Find("Register/Nickname").GetComponentInChildren<TMP_InputField>();
        _registerPassword = SelfTransform.Find("Register/Password").GetComponentInChildren<TMP_InputField>();
        _registerRepassword = SelfTransform.Find("Register/Repassword").GetComponentInChildren<TMP_InputField>();
        _registerTip = SelfTransform.Find("Register/Tip").GetComponent<TMP_Text>();
    }

    private void OnSendClick()
    {
        if(string.IsNullOrEmpty (_registerUsername.text))
        {
            _mono.ShowRegisterTip("用户名不能为空");
            return;
        }
        bool isEmail = IsValidEmail(_registerUsername.text);

        if(!isEmail)
        {
            _mono.ShowRegisterTip("用户名必须是邮箱格式");
            return;
        }

        _mono.SendCode();
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", _registerUsername.text);
        HTTPHandler.Instance.POST("user/sendcode", formData, OnSendCodeResponse, OnSendCodeError);
    }

    void OnSendCodeResponse(string msg)
    {
        ServiceResult<string> result = JsonConvert.DeserializeObject<ServiceResult<string>>(msg);
        _mono.ShowRegisterTip(result.msg);
    }

    void OnSendCodeError(string msg)
    {
        _mono.ShowRegisterTip(msg);
    }

    bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(email);
    }

    private void OnBackClick()
    {
        _login.SetActive(true );
        _register.SetActive(false );
    }

    private void OnConfirmClick()
    {
        if (_registerPassword.text != _registerRepassword.text)
        {
            _registerTip.text = "两次密码不一致";
            return;
        }
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", _registerUsername.text);
        formData.Add("password", _registerPassword.text);
        formData.Add("code", _registerCode.text);
        formData.Add("nickname", _registerNickname.text);
        HTTPHandler.Instance.POST("user/register", formData, OnRegisterResponse, OnRegisterError);
    }

    void OnRegisterResponse(string msg)
    {
        ServiceResult<string> result = JsonConvert.DeserializeObject<ServiceResult<string>>(msg);
        switch (result.code)
        {
            case 1001:
            {
                _login.SetActive(true);
                _register.SetActive(false);
            }
            break;
            case 0:
            {
                _mono.ShowRegisterTip(result.msg);
            }
            break;
            case -1:
            {
                _mono.ShowRegisterTip(result.msg);
            }
            break;
        }
    }

    void OnRegisterError(string msg)
    {
        Debug.Log(msg);
    }

    private void OnRegisterClick()
    {
        _login.SetActive(false);
        _register.SetActive(true);
    }

    private void OnLoginClick()
    {
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("username", _loginUsername.text);
        formData.Add("password", _loginPassword.text);
        string resource = "user/login";
        HTTPHandler.Instance.POST(resource, formData,OnResponse ,OnError );
    }

    void OnResponse(string msg)
    {
        ServiceResult<UserData> result = JsonConvert.DeserializeObject<ServiceResult<UserData>>(msg);
        switch (result.code)
        {
            case 1001:
                {
                    Debug.Log("登录成功");
                    UserData userData = result.data[0];
                    ConfigManager.Instance.SaveUserData(userData);
                    
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
            case -1:
                {
                    _mono.ShowLoginTip(msg);
                }
                break;
        }
    }
    void OnError(string msg)
    {
        _mono .ShowLoginTip (msg);
    }
}

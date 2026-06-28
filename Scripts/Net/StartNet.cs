using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class StartNet : NetworkBehaviour
{
    public static StartNet instance;
    
    void Awake()
    {
        instance = this;
        Debug.Log("StartNet Awake - instance initialized");
    }
    
    public void StartServer(ushort port)
    {
        Debug.Log("Starting Server on port: " + port);
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData("0.0.0.0", port);
        NetworkManager.Singleton.StartServer();
        Debug.Log("Server started");
    }
    
    public void StartClient(string ip, ushort port)
    {
        Debug.Log("Starting Client, connecting to: " + ip + ":" + port);
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, port);
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started, waiting for connection...");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log("Client connected successfully! ClientId: " + obj);
        // 只有本地客户端连接成功时才打开大厅
        if (obj == NetworkManager.Singleton.LocalClientId)
        {
            WndManager.Instance.CloseWnd<StartWnd>();
            WndManager.Instance.CloseWnd<AccountWnd>();
            WndManager.Instance.OpenWnd<LobbyWnd>();
        }
    }
    
    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        Debug.LogError("Client disconnected! ClientId: " + obj);
        // 显示错误提示
        Debug.LogError("无法连接到服务器，请检查服务器是否启动，或者IP地址是否正确！");
    }
    
    void Update()
    {
        
    }
}

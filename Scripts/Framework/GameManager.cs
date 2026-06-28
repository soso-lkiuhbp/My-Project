using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1024, 768, false);
        ConfigManager.Instance.Initial();

        if (ConfigManager.Instance.ConfigInfo.role == 0)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData("0.0.0.0", (ushort)ConfigManager.Instance.ConfigInfo.port);
            NetworkManager.Singleton.StartServer();
        }
        else
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            WndManager.Instance.Initial(canvas);
            WndManager.Instance.OpenWnd<StartWnd>();
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomWnd : BaseWnd
{
    TMP_Text _hostPlayerID;
    TMP_Text _clientPlayerID;
    Button _startBtn;
    Button _closeBtn;
    Toggle _ready;
    TMP_Text _readyTip;
    TMP_Text _playerReadyTip;
    public override void Initial()
    {
        _hostPlayerID = SelfTransform.Find("HostPlayer/ID").GetComponent<TMP_Text>();
        _clientPlayerID = SelfTransform.Find("ClientPlayer/ID").GetComponent<TMP_Text>();
        _startBtn = SelfTransform.Find("StartBtn").GetComponent<Button>();
        _startBtn.onClick.AddListener(OnstartClick);
        _closeBtn = SelfTransform.Find("CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.AddListener(OnCloseClick);
        _ready = SelfTransform.Find("Ready").GetComponent<Toggle>();
        _ready.onValueChanged.AddListener(OnReadyToggle);
        _readyTip = SelfTransform.Find("Ready/Tip").GetComponent<TMP_Text>();
        _playerReadyTip = SelfTransform.Find("ClientPlayer/Tip").GetComponent<TMP_Text>();
    }

    private void OnReadyToggle(bool ready)
    {
        if (ready)
        {
            _readyTip.text = "取消准备";
            LobbyNet.instance.SyncRoomState(ready);
        }
        else
        {
            _readyTip.text = "准备";
            LobbyNet.instance.SyncRoomState(ready);
        }
        
    }
    public void UpdateRoomState(bool clientPlayerReady)
    {
        if (clientPlayerReady)
        {
            _startBtn.interactable = true;
            _playerReadyTip.text = "准备";
        }
        else
        {
            _startBtn.interactable = false;
            _playerReadyTip.text = "";
        }
    }
    
    private void OnCloseClick()
    {
        LobbyNet.instance.CloseRoom();
        CloseWnd();
        WndManager.Instance.OpenWnd<LobbyWnd>();
    }

    private void OnstartClick()
    {
        CloseWnd();
        WndManager.Instance.GetWnd<GameWnd>();
        LobbyNet.instance.SyncGameStart();
    }

    public void UpdateRoomInfo(RoomInfo roomInfo,PlayerInfo playerInfo)
    {
        _hostPlayerID.text = roomInfo.HostPlayerID;
        _clientPlayerID.text = roomInfo.ClientPlayerID;

        if(roomInfo .HostPlayerID ==playerInfo .PlayerID)
        {
            //是房主
            _startBtn.gameObject.SetActive(true);
        }
        else
        {
            _ready.gameObject.SetActive(true);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyWnd : BaseWnd
{
    public override void Initial()
    {
        Button roomBtn = SelfTransform.Find("RoomBtn").GetComponent<Button>();
        roomBtn.onClick.AddListener(OnRoomClick);
        Button matchBtn = SelfTransform.Find("MatchBtn").GetComponent<Button>();
        matchBtn.onClick.AddListener(OnMatchClick);
        TMP_Text username=SelfTransform.Find("Username").GetComponent<TMP_Text>();
        username.text = ConfigManager.Instance.UserData.nickname;
    }

    private void OnMatchClick()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<MatchWnd>();
    }

    private void OnRoomClick()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<RoomLisWnd>();
    }
}
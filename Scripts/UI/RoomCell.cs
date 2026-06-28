using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCell : MonoBehaviour
{
    public RoomInfo RoomInfo;
    Button _joinBtn;
    public void Initial(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        TMP_Text id = transform.Find("ID").GetComponent<TMP_Text>();
        id.text = roomInfo.RoomID;
        TMP_Text playerID = transform.Find("PlayerID").GetComponent<TMP_Text>();
        playerID.text = roomInfo.HostPlayerID;
       _joinBtn = transform.Find("JoinBtn").GetComponent<Button>();
       _joinBtn.onClick.AddListener(OnJoinClick);
    }

    private void OnJoinClick()
    {
        LobbyNet.instance.JoinRoom(RoomInfo);
    }
    public void UpdateCell(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        if(RoomInfo .HostPlayerID != "" && RoomInfo.ClientPlayerID != "")
        {
            _joinBtn.gameObject.SetActive(false);
        }
        else
        {
            _joinBtn.gameObject.SetActive(true);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

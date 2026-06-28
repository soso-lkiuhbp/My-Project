using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomLisWnd : BaseWnd
{
    GameObject _originCell;
    RectTransform _contentTransform;
    GridLayoutGroup _contentLayout;
    List<RoomCell> _cellList;
    public override void Initial()
    {
        Button createbutton = SelfTransform.Find("CreateBtn").GetComponent<Button>();
        createbutton.onClick.AddListener(OncreateClick);

        _contentTransform = SelfTransform.Find("RoomList/Viewport/Content").GetComponent<RectTransform>();
        _contentLayout = _contentTransform.GetComponent<GridLayoutGroup>();
        _originCell= _contentTransform.Find("RoomCell").gameObject;

        _cellList = new List<RoomCell>();

        LobbyNet.instance.SyncRoomList();
    }

    private void OncreateClick()
    {
        LobbyNet.instance.CreateRoom();
    }

    public void AddRoomCell(RoomInfo roomInfo)
    {
        GameObject clone = GameObject.Instantiate(_originCell);
        clone.transform.SetParent(_contentTransform, false);
        RoomCell roomCell = clone.GetComponent<RoomCell>();
        roomCell.Initial(roomInfo);
        _cellList.Add(roomCell);
        Vector2 size = _contentTransform.sizeDelta;
        size.y = _cellList.Count * _contentLayout.cellSize.y;
        _contentTransform.sizeDelta = size;
        clone.SetActive(true);
    }
    public void UpdateRoomCell(RoomInfo roomInfo)
    {
        var targetCell = _cellList.Find(tmp => tmp.RoomInfo.RoomID == roomInfo.RoomID);
        targetCell.UpdateCell(roomInfo);
    }
}

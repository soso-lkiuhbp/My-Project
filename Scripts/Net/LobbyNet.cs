using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyNet : NetworkBehaviour
{
    Dictionary<string, RoomInfo> _roomInfoDict;
    public static LobbyNet instance;
    public PlayerInfo LocalPlayerInfo;
    RoomInfo _localRoomInfo;
    Dictionary<string, RoomInfo> _matchInfoDict;

    void Start()
    {
        instance = this;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            _roomInfoDict = new Dictionary<string, RoomInfo>();
            _matchInfoDict = new Dictionary<string, RoomInfo>();
        }
        else
        {
            LocalPlayerInfo = new PlayerInfo();
            LocalPlayerInfo.PlayerID = Guid.NewGuid().ToString();
            LocalPlayerInfo.RoomID = "";
        }
    }
    public void StartMatch()
    {
        StartMatchRpc(LocalPlayerInfo);
    }
    [Rpc(SendTo.Server)]
    void StartMatchRpc(PlayerInfo playerInfo)
    {
        bool hasRoom = false;
        RoomInfo existRoom = new RoomInfo ();
        foreach(var item in _matchInfoDict)
        {
            if(item.Value .ClientPlayerID == "")
            {
                hasRoom = true;
                existRoom = item.Value;
                break;
            }
        }
        if (hasRoom)
        {
            JoinRoomLocalRpc(playerInfo, existRoom);

        }
        else
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.RoomID = Guid.NewGuid().ToString();
            roomInfo.HostPlayerID = playerInfo.PlayerID;
            roomInfo.ClientPlayerID = "";
            roomInfo.IsMatch = true;
            playerInfo.RoomID = roomInfo.RoomID;
            _matchInfoDict.Add(roomInfo.RoomID, roomInfo);
            CreateRoomLocalRpc(playerInfo, roomInfo);
        }
    }
    [Rpc(SendTo .ClientsAndHost)]
    void CreateRoomLocalRpc(PlayerInfo playerInfo,RoomInfo roomInfo)
    {
        if(LocalPlayerInfo .PlayerID ==playerInfo .PlayerID)
        {
            _localRoomInfo = roomInfo;
            LocalPlayerInfo = playerInfo;
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    void JoinRoomLocalRpc(PlayerInfo playerInfo,RoomInfo roomInfo)
    {
        if(playerInfo .PlayerID ==LocalPlayerInfo.PlayerID)
        {
            JoinRoom(roomInfo);
        }
    }
    public void CreateRoom()
    {
        _localRoomInfo= new RoomInfo();
        _localRoomInfo.RoomID = Guid.NewGuid().ToString();
        _localRoomInfo.HostPlayerID = LocalPlayerInfo.PlayerID;
        _localRoomInfo.ClientPlayerID = "";
        LocalPlayerInfo.RoomID = _localRoomInfo.RoomID;

        WndManager.Instance.CloseWnd<RoomLisWnd>();
        WndManager.Instance.OpenWnd<RoomWnd>();
        WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(_localRoomInfo,LocalPlayerInfo);

        CreateRoomRpc(_localRoomInfo);
    }
    [Rpc(SendTo.NotMe)]
    void CreateRoomRpc(RoomInfo roomInfo)
    {
        if (IsServer)
        {
            _roomInfoDict.Add(roomInfo.RoomID, roomInfo);
        }
        else
        {
            var wnd = WndManager.Instance.GetWnd<RoomLisWnd>();
            if (wnd != null)
            {
                wnd.AddRoomCell(roomInfo);
            }
        }

    }
    
    public void SyncRoomList()
    {
        SyncRoomListRpc(LocalPlayerInfo);
    }
    [Rpc(SendTo.Server)]
    public void SyncRoomListRpc(PlayerInfo playerInfo)
    {
        foreach (var item in _roomInfoDict)
        {
            SyncRoomListLocalRpc(item.Value, playerInfo.PlayerID);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    void SyncRoomListLocalRpc(RoomInfo roomInfo,string playerID)
    {
        if(playerID == LocalPlayerInfo.PlayerID)
        {
            var wnd = WndManager.Instance.GetWnd<RoomLisWnd>();
            wnd.AddRoomCell(roomInfo);
        }
    }
    public void JoinRoom(RoomInfo roomInfo)
    {
        roomInfo.ClientPlayerID = LocalPlayerInfo.PlayerID;
        LocalPlayerInfo.RoomID = roomInfo.RoomID;
        _localRoomInfo = roomInfo;
        if(roomInfo .IsMatch)
        {
            WndManager.Instance.GetWnd<MatchWnd>().UpdateMatchInfo(_localRoomInfo, LocalPlayerInfo);
        }
        else
        {
            WndManager.Instance.CloseWnd<RoomLisWnd>();
            WndManager.Instance.OpenWnd<RoomWnd>();
            WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(_localRoomInfo, LocalPlayerInfo);
        }
        JoinRoomRpc(roomInfo);
    }
    [Rpc(SendTo.NotMe)]
    void JoinRoomRpc(RoomInfo roomInfo)
    {
        if (IsServer)
        {
            if(!roomInfo .IsMatch)
            {
                _roomInfoDict[roomInfo.RoomID] = roomInfo;
            }
        }   
        else
        {
            if(roomInfo .IsMatch)
            {
                WndManager.Instance.GetWnd<MatchWnd>().UpdateMatchInfo(roomInfo,LocalPlayerInfo);
            }
            else
            {
                if (_localRoomInfo.RoomID == roomInfo.RoomID)
                {
                    //���·�����Ϣ
                    WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(roomInfo, LocalPlayerInfo);
                }
                else
                {
                    //���·����б�
                    WndManager.Instance.GetWnd<RoomLisWnd>().UpdateRoomCell(roomInfo);
                }
            }
        }
    }
    public void SyncRoomState(bool ready)
    {
        SyncRoomStateRpc(_localRoomInfo, ready);
    }
    [Rpc(SendTo.NotMe)]
    public void SyncRoomStateRpc(RoomInfo roomInfo,bool ready)
    {
        if(IsClient&&roomInfo .RoomID == _localRoomInfo.RoomID)
        {
            WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomState(ready);
        }
    }
    public void SyncGameStart()
    {
        SyncGameStartRpc(_localRoomInfo);
    }

    [Rpc(SendTo.NotMe)]
    void SyncGameStartRpc(RoomInfo roomInfo)
    {
        if(IsClient &&roomInfo .RoomID ==_localRoomInfo.RoomID)
        {
            if(roomInfo .IsMatch)
            {
                WndManager.Instance.DeleteWnd<MatchWnd>();
                WndManager.Instance.OpenWnd<GameWnd>();
            }
            else
            {
                WndManager.Instance.CloseWnd<RoomWnd>();
                WndManager.Instance.OpenWnd<GameWnd>();
            }

        }
    }

    public void CloseRoom()
    {
        if (_localRoomInfo.RoomID != "")
        {
            CloseRoomServerRpc(_localRoomInfo.RoomID);
            _localRoomInfo = default;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CloseRoomServerRpc(string roomID)
    {
        _roomInfoDict.Remove(roomID);
        CloseRoomClientRpc(roomID);
    }

    [ClientRpc]
    void CloseRoomClientRpc(string roomID)
    {
        if (_localRoomInfo.RoomID == roomID)
        {
            var wnd = WndManager.Instance.GetWnd<RoomWnd>();
            if (wnd != null)
            {
                wnd.CloseWnd();
                WndManager.Instance.OpenWnd<LobbyWnd>();
            }
            _localRoomInfo = default;
        }
    }

    public void CancelMatch()
    {
        if (_localRoomInfo.RoomID != "")
        {
            CancelMatchServerRpc(_localRoomInfo.RoomID, LocalPlayerInfo.PlayerID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CancelMatchServerRpc(string roomID, string playerID)
    {
        // 同时从两个字典里删除/清理
        if (_matchInfoDict.TryGetValue(roomID, out RoomInfo matchInfo))
        {
            if (matchInfo.HostPlayerID == playerID)
            {
                _matchInfoDict.Remove(roomID);
            }
            else
            {
                matchInfo.ClientPlayerID = "";
                _matchInfoDict[roomID] = matchInfo;
            }
        }

        if (_roomInfoDict.TryGetValue(roomID, out RoomInfo info))
        {
            if (info.HostPlayerID == playerID)
            {
                _roomInfoDict.Remove(roomID);
            }
            else
            {
                info.ClientPlayerID = "";
                _roomInfoDict[roomID] = info;
            }
        }
    }



    void Update()
    {
        
    }
}

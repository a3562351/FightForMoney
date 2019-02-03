using Common.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlayerCtrl : CtrlBase
{
    public static PlayerCtrl Instance = null;
    private int player_id;
    private string login_key;

    public static PlayerCtrl GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PlayerCtrl();
        }
        return Instance;
    }

    public override void Init()
    {
        base.Init();
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_Login, this.SCLogin);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_HeartBeat, this.SCHeartBeat);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_Notice, this.SCNotice);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_PlayerList, this.SCPlayerList);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_PlayerInfo, this.SCPlayerInfo);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_SceneEnter, this.SCSceneEnter);
        ClientSocket.GetInstance().AddSCHandler(MsgCode.SC_MapInfo, this.SCMapInfo);
    }

    public void ReConnect()
    {
        CSReconnect message = new CSReconnect();
        message.PlayerId = this.player_id;
        message.LoginKey = this.login_key;
        ClientSocket.GetInstance().SendMessage(message);
    }

    public void Notice(int code, params string[] param)
    {
        string str = string.Format(NoticeCode.GetStr(code), param);

        Debug.Log(NoticeCode.GetStr(code));
    }

    public void LoadMap(MapInfo map_info)
    {

    }

    public void CSLogin(bool is_login)
    {
        CSLogin protocol = new CSLogin()
        {
            Account = SystemInfo.deviceUniqueIdentifier,
            Password = "",
            IsLogin = is_login,
        };

        ClientSocket.GetInstance().SendMessage(protocol);
    }

    public void CSCreatePlayer(string player_name, string map_name)
    {
        CSCreatePlayer protocol = new CSCreatePlayer()
        {
            PlayerName = player_name,
            MapName = map_name,
        };

        ClientSocket.GetInstance().SendMessage(protocol);
    }

    public void CSLoadPlayer(int player_id)
    {
        CSLoadPlayer protocol = new CSLoadPlayer()
        {
            PlayerId = player_id,
        };

        ClientSocket.GetInstance().SendMessage(protocol);
    }

    private void SCLogin(IMessage data)
    {
        SCLogin protocol = data as SCLogin;
        this.Notice(protocol.ResultCode);

        if(protocol.ResultCode == NoticeCode.LoginSucc)
        {
            this.login_key = protocol.LoginKey;
        }
        else if(protocol.ResultCode == NoticeCode.NotExistAccount)
        {
            EventDispatcher.GetInstance().DispatchEvent(EventType.Register);
        }
    }

    private void SCHeartBeat(IMessage data)
    {

    }

    private void SCNotice(IMessage data)
    {
        SCNotice protocol = data as SCNotice;
        string[] param = new string[protocol.Param.Count];
        for (int i = 0; i < protocol.Param.Count; i++)
        {
            param[i] = protocol.Param[i];
        }
        this.Notice(protocol.NoticeCode, param);
    }

    private void SCPlayerList(IMessage data)
    {
        SCPlayerList protocol = data as SCPlayerList;
        
        EventData event_data = new EventData()
        {
            data = protocol
        };
        EventDispatcher.GetInstance().DispatchEvent(EventType.SelectPlayer, event_data);
    }

    private void SCPlayerInfo(IMessage data)
    {
        SCPlayerInfo protocol = data as SCPlayerInfo;
        PlayerStruct player_struct = protocol.PlayerStruct;

        Debug.Log(player_struct);
    }

    private void SCSceneEnter(IMessage data)
    {
        CSSceneEnter protocol = new CSSceneEnter();
        ClientSocket.GetInstance().SendMessage(protocol);
    }

    private void SCMapInfo(IMessage data)
    {
        SCMapInfo protocol = data as SCMapInfo;
        this.LoadMap(protocol.MapInfo);
    }
}

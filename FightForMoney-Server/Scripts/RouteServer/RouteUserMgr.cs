﻿using Common.Protobuf;
using Google.Protobuf;
using System.Collections.Generic;

class RoutePlayerState
{
    public const byte CONNECTED = 1 << 0;           //已连接
    public const byte LOGINING = 1 << 1;            //登陆中
    public const byte LOGINED = 1 << 2;             //已登录
    public const byte LOADED = 1 << 3;              //已加载角色
    public const byte CHANGE_SCENE = 1 << 4;        //切场景状态
}

class RoutePlayer
{
    public int UserId;
    public int PlayerId;
    public int ConnectId;
    public string LoginKey;
    public int State = 0;
    public Dictionary<int, int> StateStartTime = new Dictionary<int, int>();
    public int ActiveTime;
    public int CurServerId;
    public int CurSceneId;
    public CSLoadPlayer LoadMsg;                                //用于顶号
    private int LogoutTimerId;
    private List<IMessage> MsgCache = new List<IMessage>();     //用于断线重连

    public RoutePlayer(int connect_id)
    {
        this.ConnectId = connect_id;
        this.AddState(RoutePlayerState.CONNECTED);
    }

    public bool IsOriginState()
    {
        return this.State == RoutePlayerState.CONNECTED;
    }

    public void ResetState()
    {
        this.State = RoutePlayerState.CONNECTED;
    }

    public void SetActive()
    {
        this.ActiveTime = Global.GetCurTime();
    }

    public void ClearMsgCache()
    {
        this.MsgCache.Clear();
    }

    public bool CheckState(byte state)
    {
        return (this.State & state) != 0;
    }

    public void AddState(byte state)
    {
        if (!this.CheckState(state))
        {
            this.State = this.State + state;
        }
        this.StateStartTime[state] = Global.GetCurTime();
        Log.DebugFormat("RoleId:{0} ConnectId:{1} AddState:{2} StartTime:{3}", this.PlayerId, this.ConnectId, state, this.StateStartTime[state]);
    }

    public void CancelState(byte state)
    {
        if (this.CheckState(state))
        {
            this.State = this.State - state;
        }
        int after_time = Global.GetCurTime() - this.StateStartTime[state];
        Log.DebugFormat("RoleId:{0} ConnectId:{1} CancelState:{2} AfterTime:{3}", this.PlayerId, this.ConnectId, state, after_time);
    }

    public void Reconnect()
    {
        this.AddState(RoutePlayerState.CONNECTED);
        TimerMgr.GetInstance().DelTimer(this.LogoutTimerId);

        //重连时发送断线期间的信息
        for (int i = 0; i < this.MsgCache.Count; i++)
        {
            this.SendMsg(this.MsgCache[i]);
        }
    }

    public void Disconnect()
    {
        this.CancelState(RoutePlayerState.CONNECTED);

        //3分钟重连时间
        this.LogoutTimerId = TimerMgr.GetInstance().AddTimer(3 * 60 * 1000, data =>
        {
            this.Logout();
        }, null);
    }

    public void Logout()
    {
        RSPlayerLogout protocol = new RSPlayerLogout();
        protocol.PlayerId = PlayerId;
        Server.GetInstance().GetServer<RouteServer>().SendMsgToAllServer(protocol);
        RouteUserMgr.GetInstance().DelPlayer(this);
    }

    public void SendMsg(IMessage message)
    {
        if (this.CheckState(RoutePlayerState.CONNECTED))
        {
            Server.GetInstance().GetSocket().SendMsgToClient(message, this.ConnectId);
        }
        else
        {
            this.MsgCache.Add(message);
        }
    }
}

class RouteUserMgr
{
    private static RouteUserMgr Instance = null;
    private Dictionary<int, RoutePlayer> connect_id_to_player = new Dictionary<int, RoutePlayer>();
    private Dictionary<int, RoutePlayer> player_id_to_player = new Dictionary<int, RoutePlayer>();

    public static RouteUserMgr GetInstance()
    {
        if (Instance == null)
        {
            Instance = new RouteUserMgr();
        }
        return Instance;
    }

    public void Update(int dt)
    {

    }

    public void AddPlayerByConnectId(RoutePlayer player)
    {
        this.connect_id_to_player[player.ConnectId] = player;
    }

    public RoutePlayer GetPlayerByConnectId(int connect_id)
    {
        if (!this.connect_id_to_player.ContainsKey(connect_id))
        {
            return null;
        }
        return this.connect_id_to_player[connect_id];
    }

    public void AddPlayerByPlayerId(RoutePlayer player)
    {
        this.player_id_to_player[player.PlayerId] = player;
    }

    public RoutePlayer GetPlayerByPlayerId(int player_id)
    {
        if (!this.player_id_to_player.ContainsKey(player_id))
        {
            return null;
        }
        return this.player_id_to_player[player_id];
    }

    public void DelPlayer(RoutePlayer player)
    {
        if (this.connect_id_to_player.ContainsKey(player.ConnectId))
        {
            this.connect_id_to_player.Remove(player.ConnectId);
        }

        if (this.player_id_to_player.ContainsKey(player.PlayerId))
        {
            this.player_id_to_player.Remove(player.PlayerId);
        }

        Log.DebugFormat("Del RoutePlayer Id:{0}", player.PlayerId);
    }

    //重连去掉旧连接
    public void DelPlayerByConnectId(int connect_id)
    {
        if (this.connect_id_to_player.ContainsKey(connect_id))
        {
            this.connect_id_to_player.Remove(connect_id);
        }
    }
}

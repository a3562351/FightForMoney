using Common.Protobuf;
using Google.Protobuf;
using System.Collections.Generic;

class UserState
{
    public const byte LOGINING = 1 << 0;            //登陆中
    public const byte LOGINED = 1 << 1;             //登录已成功
    public const byte CHANGE_SCENE = 1 << 2;        //切场景状态
}

class User
{
    public RouteServer server;
    public int ConnectId;
    public int UserId;
    public string LoginKey;
    public int State = 0;
    public Dictionary<int, int> StateStartTime = new Dictionary<int, int>();
    public int ActiveTime;
    public int CurServerId;
    public int CurSceneId;
    public IMessage LoginMsg;
    private bool IsConnect;
    private int LogoutTimerId;
    private List<IMessage> msg_cache_list = new List<IMessage>();     //用于断线重连

    public User(RouteServer server, int connect_id)
    {
        this.server = server;
        this.ConnectId = connect_id;
        this.IsConnect = true;
    }

    public void SetActive()
    {
        this.ActiveTime = Global.GetCurTime();
    }

    public void ClearMsgCache()
    {
        this.msg_cache_list.Clear();
    }

    public void AddState(byte state)
    {
        if (!this.CheckState(state))
        {
            this.State = this.State + (1 << state);
        }
        this.StateStartTime[state] = Global.GetCurTime();
        Log.Debug(string.Format("UserId:{0} ConnectId:{1} AddState:{2} StartTime:{3}", this.UserId, this.ConnectId, state, this.StateStartTime[state]));
    }

    public void CancelState(byte state)
    {
        if (this.CheckState(state))
        {
            this.State = this.State - (1 << state);
        }
        int after_time = Global.GetCurTime() - this.StateStartTime[state];
        Log.Debug(string.Format("UserId:{0} ConnectId:{1} CancelState:{2} AfterTime:{3}", this.UserId, this.ConnectId, state, after_time));
    }

    public bool CheckState(byte state)
    {
        return (this.State & (1 << state)) != 0;
    }

    public void Reconnect()
    {
        this.IsConnect = true;
        TimerMgr.GetInstance().DelTimer(this.LogoutTimerId);

        //重连时发送断线期间的信息
        for (int i = 0; i < this.msg_cache_list.Count; i++)
        {
            this.SendMsg(this.msg_cache_list[i]);
        }
    }

    public void Disconnect()
    {
        this.IsConnect = false;
        
        //3分钟重连时间
        this.LogoutTimerId = TimerMgr.GetInstance().AddTimer(3 * 60 * 1000, data =>
        {
            this.LoginOut();
        }, null);
    }

    public void LoginOut()
    {
        RSUserLoginout protocol = new RSUserLoginout();
        protocol.UserId = UserId;
        this.server.SendMsgToAllServer(protocol);
        this.server.GetUserMgr().DelUser(this);
    }

    public void SendMsg(IMessage message)
    {
        if (this.IsConnect)
        {
            this.server.GetSocket().SendMsgToClient(message, this.ConnectId);
        }
        else
        {
            this.msg_cache_list.Add(message);
        }
    }

    private int GetStateTime(byte state)
    {
        if (this.StateStartTime.ContainsKey(state))
        {
            return this.StateStartTime[state];
        }
        return 0;
    }
}

class UserMgr
{
    private ServerBase server;
    private Dictionary<int, User> connect_id_to_user = new Dictionary<int, User>();
    private Dictionary<int, User> user_id_to_user = new Dictionary<int, User>();

    public UserMgr(ServerBase server)
    {
        this.server = server;
    }

    public void Update(int dt)
    {

    }

    public void AddUserByConnectId(User user)
    {
        this.connect_id_to_user[user.ConnectId] = user;
    }

    public User GetUserByConnectId(int connect_id)
    {
        if (!this.connect_id_to_user.ContainsKey(connect_id))
        {
            return null;
        }
        return this.connect_id_to_user[connect_id];
    }

    public void AddUserByUserId(User user)
    {
        this.user_id_to_user[user.UserId] = user;
    }

    public User GetUserByUserId(int user_id)
    {
        if (!this.user_id_to_user.ContainsKey(user_id))
        {
            return null;
        }
        return this.user_id_to_user[user_id];
    }

    public void DelUser(User user)
    {
        if (this.connect_id_to_user.ContainsKey(user.ConnectId))
        {
            this.connect_id_to_user.Remove(user.ConnectId);
        }

        if (this.user_id_to_user.ContainsKey(user.UserId))
        {
            this.user_id_to_user.Remove(user.UserId);
        }

        Log.Debug("Del User Id:" + user.UserId);
    }

    //重连去掉旧连接
    public void DelUserByConnectId(int connect_id)
    {
        if (this.connect_id_to_user.ContainsKey(connect_id))
        {
            this.connect_id_to_user.Remove(connect_id);
        }
    }
}

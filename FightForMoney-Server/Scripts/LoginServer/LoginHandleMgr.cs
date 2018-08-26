using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class LoginHandleMgr
{
    private ServerBase server;
    private LoginServerData server_data;
    private Dictionary<int, bool> user_map = new Dictionary<int, bool>();

    public LoginHandleMgr(ServerBase server)
    {
        this.server = server;
    }

    public void Init()
    {
        this.server.GetSocket().AddHandler(typeof(CSLogin), this.CSLogin);
        this.server.GetSocket().AddHandler(typeof(CSCreatePlayer), this.CSCreatePlayer);
        this.server.GetSocket().AddHandler(typeof(CSLoadPlayer), this.CSLoadPlayer);

        this.server_data = DataTool.LoadLServerData() ?? new LoginServerData();
        if (this.server_data.MaxUserId < Const.MIN_USER_ID)
        {
            this.server_data.MaxUserId = Const.MIN_USER_ID;
        }

        if (this.server_data.MaxPlayerId < Const.MIN_PLAYER_ID)
        {
            this.server_data.MaxPlayerId = Const.MIN_PLAYER_ID;
        }
    }

    public void Save()
    {
        DataTool.SaveLServerData(this.server_data);
    }

    public void SendPlayerList(List<int> player_id_list, int user_id)
    {
        SCPlayerList message = new SCPlayerList();
        foreach (int player_id in player_id_list)
        {
            PlayerStruct player_struct = DataTool.LoadPlayer(player_id);
            PlayerInfo player_info = new PlayerInfo();
            player_info.PlayerId = player_struct.PlayerData.Id;
            player_info.PlayerName = player_struct.PlayerData.Name;
            player_info.MapName = player_struct.PlayerData.MapName;
            message.PlayerList.Add(player_info);
        }
        this.server.GetSocket().SendMsgToRoute(message, user_id, null);
    }

    private void CSLogin(IMessage data, int connect_id, int addition, List<int> user_id_list)
    {
        CSLogin protocol = data as CSLogin;
        string account = protocol.Account;
        string password = protocol.Password;
        bool is_login = protocol.IsLogin;
        int connect_id_in_route = addition;

        AccountInfo account_info = DataTool.LoadAccountData(account);

        //登陆
        if (is_login)
        {
            if (account_info != null)
            {
                if (this.user_map.ContainsKey(account_info.UserId))
                {
                    LRLoginResult message = new LRLoginResult();
                    message.ResultCode = NoticeCode.RepeatLogin;
                    message.UserId = account_info.UserId;
                    this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route, null);
                }
                else
                {
                    UserInfo user_info = DataTool.LoadUser(account_info.UserId);
                    user_info.LoginTime = Global.GetCurTime();
                    DataTool.SaveUser(user_info);

                    LRLoginResult message = new LRLoginResult();
                    message.ResultCode = NoticeCode.LoginSucc;
                    message.UserId = user_info.UserId;
                    this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route, null);

                    this.user_map[account_info.UserId] = true;

                    List<int> player_id_list = new List<int>();
                    foreach (int player_id in user_info.PlayerIdList)
                    {
                        player_id_list.Add(player_id);
                    }
                    this.SendPlayerList(player_id_list, user_info.UserId);
                }
            }
            else
            {
                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.NotExistAccount;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route, null);
            }
        }
        //注册
        else
        {
            if (account_info != null)
            {
                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.ExistAccount;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route, null);
            }
            else
            {
                account_info = new AccountInfo();
                account_info.Account = account;
                account_info.Password = password;
                account_info.CreateTime = Global.GetCurTime();
                account_info.UserId = ++this.server_data.MaxUserId;
                DataTool.SaveAccount(account_info);

                UserInfo user_info = new UserInfo();
                user_info.UserId = account_info.UserId;
                user_info.LoginTime = Global.GetCurTime();
                DataTool.SaveUser(user_info);

                this.Save();

                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.LoginSucc;
                message.UserId = user_info.UserId;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route, null);

                this.SendPlayerList(new List<int>(), user_info.UserId);
            }
        }
    }

    private void CSCreatePlayer(IMessage data, int connect_id, int addition, List<int> user_id_list)
    {
        CSCreatePlayer protocol = data as CSCreatePlayer;
        int user_id = addition;
        string player_name = protocol.PlayerName;
        string map_name = protocol.MapName;

        if (this.server_data.PlayerNameMap.ContainsKey(player_name))
        {
            return;
        }

        UserInfo user_info = DataTool.LoadUser(user_id);
        if (user_info == null)
        {
            return;
        }

        int player_id = ++this.server_data.MaxPlayerId;
        Player player = new Player(null, 0);
        player.GetPlayerData().Id = player_id;
        player.GetPlayerData().Name = player_name;
        player.GetPlayerData().MapName = map_name;
        player.Save();

        user_info.PlayerIdList.Add(player_id);
        DataTool.SaveUser(user_info);

        this.server_data.PlayerNameMap[player_name] = player_id;
        this.Save();

        JObject json = new JObject();
        json["UserId"] = user_id;
        json["PlayerId"] = player_id;
        this.server.RemoteCall(RemoteId.LS_LoadPlayer, json, SceneId.MAP);
    }

    private void CSLoadPlayer(IMessage data, int connect_id, int addition, List<int> user_id_list)
    {
        CSLoadPlayer protocol = data as CSLoadPlayer;
        int user_id = addition;
        int player_id = protocol.PlayerId;

        UserInfo user_info = DataTool.LoadUser(user_id);
        if (user_info == null)
        {
            return;
        }

        if (!user_info.PlayerIdList.Contains(player_id))
        {
            return;
        }

        JObject json = new JObject();
        json["UserId"] = user_id;
        json["PlayerId"] = player_id;
        this.server.RemoteCall(RemoteId.LS_LoadPlayer, json, SceneId.MAP);
    }
}

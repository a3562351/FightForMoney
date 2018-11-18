using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class LoginHandleMgr
{
    private ServerBase server;
    private LoginServerData server_data;
    private Dictionary<int, bool> player_map = new Dictionary<int, bool>();

    public LoginHandleMgr(ServerBase server)
    {
        this.server = server;
    }

    public void Init()
    {
        this.server.GetSocket().AddCSHandler(typeof(CSLogin), this.CSLogin);
        this.server.GetSocket().AddCSHandler(typeof(CSCreatePlayer), this.CSCreatePlayer);
        this.server.GetSocket().AddCSHandler(typeof(CSLoadPlayer), this.CSLoadPlayer);
        this.server.GetSocket().AddCSHandler(typeof(RSPlayerLogout), this.RSPlayerLogout);

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

    public void SendPlayerList(List<int> player_id_list, int connect_id_in_route)
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
        this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);
    }

    private void CSLogin(IMessage data, int connect_id, int addition)
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
                UserInfo user_info = DataTool.LoadUser(account_info.UserId);
                user_info.LoginTime = Global.GetCurTime();
                DataTool.SaveUser(user_info);

                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.LoginSucc;
                message.UserId = user_info.UserId;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);

                List<int> player_id_list = new List<int>();
                foreach (int player_id in user_info.PlayerIdList)
                {
                    player_id_list.Add(player_id);
                }
                this.SendPlayerList(player_id_list, connect_id_in_route);
            }
            else
            {
                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.NotExistAccount;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);
            }
        }
        //注册
        else
        {
            if (account_info != null)
            {
                LRLoginResult message = new LRLoginResult();
                message.ResultCode = NoticeCode.ExistAccount;
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);
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
                this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);

                this.SendPlayerList(new List<int>(), connect_id_in_route);
            }
        }
    }

    private void CSCreatePlayer(IMessage data, int connect_id, int addition)
    {
        CSCreatePlayer protocol = data as CSCreatePlayer;
        int user_id = protocol.UserId;
        string player_name = protocol.PlayerName;
        string map_name = protocol.MapName;
        int connect_id_in_route = addition;

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
        Player player = new Player();
        player.GetPlayerData().Id = player_id;
        player.GetPlayerData().Name = player_name;
        player.GetPlayerData().MapName = map_name;
        player.Save();

        user_info.PlayerIdList.Add(player_id);
        DataTool.SaveUser(user_info);

        this.server_data.PlayerNameMap[player_name] = player_id;
        this.Save();

        this.player_map[player_id] = true;

        JObject json = new JObject();
        json["UserId"] = user_id;
        json["PlayerId"] = player_id;
        json["ConnectIdInRoute"] = connect_id_in_route;
        this.server.RemoteCall(RemoteId.LS_LoadPlayer, json, SceneId.MAP);
    }

    private void CSLoadPlayer(IMessage data, int connect_id, int addition)
    {
        CSLoadPlayer protocol = data as CSLoadPlayer;
        int user_id = protocol.UserId;
        int player_id = protocol.PlayerId;
        int connect_id_in_route = addition;

        if (this.player_map.ContainsKey(player_id))
        {
            LRPlayerRepeat message = new LRPlayerRepeat();
            message.PlayerId = player_id;
            this.server.GetSocket().SendMsgToRoute(message, connect_id_in_route);
        }

        UserInfo user_info = DataTool.LoadUser(user_id);
        if (user_info == null)
        {
            return;
        }

        if (!user_info.PlayerIdList.Contains(player_id))
        {
            return;
        }

        this.player_map[player_id] = true;

        JObject json = new JObject();
        json["UserId"] = user_id;
        json["PlayerId"] = player_id;
        json["ConnectIdInRoute"] = connect_id_in_route;
        this.server.RemoteCall(RemoteId.LS_LoadPlayer, json, SceneId.MAP);
    }

    private void RSPlayerLogout(IMessage data, int connect_id, int addition)
    {
        if (this.player_map.ContainsKey(addition))
        {
            this.player_map.Remove(addition);
        }
    }
}

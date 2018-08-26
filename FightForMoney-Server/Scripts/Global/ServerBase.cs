using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

delegate void RemoteCallBack(JObject json);

class ServerInfo
{
    public int ConnectId;   //在Route服的连接ID
    public int ServerId;
    public int ServerType;
    public List<int> SceneIdList = new List<int>();
}

class ServerBase
{
    protected ServerInfo server_info;
    protected string route_ip;
    protected int route_port;
    protected ServerSocket socket;
    protected Dictionary<int, ServerInfo> server_id_to_server_info = new Dictionary<int, ServerInfo>();
    protected Dictionary<int, ServerInfo> connect_id_to_server_info = new Dictionary<int, ServerInfo>();
    protected Dictionary<int, List<ServerInfo>> server_type_to_server_list = new Dictionary<int, List<ServerInfo>>();
    protected Dictionary<int, List<ServerInfo>> scene_id_to_server_list = new Dictionary<int, List<ServerInfo>>();
    protected Dictionary<int, RemoteCallBack> remote_callback = new Dictionary<int, RemoteCallBack>();
    protected int MAX_CALLBACK_ID = 0;

    protected ServerBase(int server_id, int server_type)
    {
        this.server_info = new ServerInfo();
        this.server_info.ServerId = server_id;
        this.server_info.ServerType = server_type;
    }

    public ServerInfo GetServerInfo()
    {
        return this.server_info;
    }

    public virtual void Init()
    {

    }

    public virtual void Update(int dt)
    {

    }

    public virtual void Release()
    {
        this.socket.Release();
    }

    public ServerSocket GetSocket()
    {
        return this.socket;
    }

    public string GetServerStr()
    {
        Dictionary<int, string> map = new Dictionary<int, string>()
        {
            {ServerType.ROUTE, "RouteServer"},
            {ServerType.LOGIN, "LoginServer"},
            {ServerType.CHAT, "ChatServer"},
            {ServerType.MAIN, "SceneServer"},
            {ServerType.INSTANCE, "InstanceServer"},
            {ServerType.COMMON, "CommonServer"},
        };

        return map[this.server_info.ServerType] + "_" + this.server_info.ServerId;
    }

    public ServerInfo GetServerInfoByServerId(int server_id)
    {
        if (!this.server_id_to_server_info.ContainsKey(server_id))
        {
            return null;
        }
        return this.server_id_to_server_info[server_id];
    }

    public ServerInfo GetServerInfoByConnectId(int connect_id)
    {
        if (!this.connect_id_to_server_info.ContainsKey(connect_id))
        {
            return null;
        }
        return this.connect_id_to_server_info[connect_id];
    }

    public ServerInfo GetServerInfoByServerType(int server_type)
    {
        if (!this.server_type_to_server_list.ContainsKey(server_type))
        {
            return null;
        }

        List<ServerInfo> server_info_list = this.server_type_to_server_list[server_type];
        if (server_info_list.Count <= 0)
        {
            return null;
        }

        int random = new Random().Next(0, server_info_list.Count);
        return server_info_list[random];
    }

    public ServerInfo GetServerInfoBySceneId(int scene_id, int server_id = 0)
    {
        if (!this.scene_id_to_server_list.ContainsKey(scene_id))
        {
            return null;
        }

        List<ServerInfo> server_info_list = this.scene_id_to_server_list[scene_id];
        if (server_info_list.Count <= 0)
        {
            return null;
        }

        //指定服务器上场景
        if (server_id > 0)
        {
            foreach (ServerInfo server_info in server_info_list)
            {
                if (server_info.ServerId == server_id)
                {
                    return server_info;
                }
            }
        }
        else
        {
            int random = new Random().Next(0, server_info_list.Count);
            return server_info_list[random];
        }

        return null;
    }

    //把自身信息注册到路由服
    protected void RegisterToRoute()
    {
        if (this.server_info.ServerType != ServerType.ROUTE)
        {
            SRRegisterServer protocol = new SRRegisterServer();
            protocol.ServerId = this.server_info.ServerId;
            protocol.ServerType = this.server_info.ServerType;
            foreach (int scene_id in this.server_info.SceneIdList)
            {
                protocol.SceneIdList.Add(scene_id);
            }
            this.socket.SendMsgToRoute(protocol);
        }
    }

    //记录其他服务器信息
    public void RegisterServer(int connect_id, int server_id, int server_type, List<int> scene_id_list)
    {
        ServerInfo server_info = new ServerInfo();
        server_info.ConnectId = connect_id;
        server_info.ServerId = server_id;
        server_info.ServerType = server_type;
        server_info.SceneIdList = scene_id_list;
        this.server_id_to_server_info[server_id] = server_info;
        this.connect_id_to_server_info[connect_id] = server_info;

        if (!this.server_type_to_server_list.ContainsKey(server_type))
        {
            this.server_type_to_server_list[server_type] = new List<ServerInfo>();
        }
        this.server_type_to_server_list[server_type].Add(server_info);

        foreach (int scene_id in scene_id_list)
        {
            if (!this.scene_id_to_server_list.ContainsKey(scene_id))
            {
                this.scene_id_to_server_list[scene_id] = new List<ServerInfo>();
            }
            this.scene_id_to_server_list[scene_id].Add(server_info);
        }
        
        Log.InfoFormat("RegisterServer Succ connect_id:{0} server_id:{1} server_type:{2} scene_id_list:{3}", 
            connect_id, server_id, server_type, DataTool.SaveAsJSON<List<int>>(scene_id_list));
    }

    //远程调用
    public void RemoteCall(int remote_id, JObject json, int scene_id, int server_id = 0, RemoteCallBack callback = null)
    {
        SSRemoteCall protocol = new SSRemoteCall();
        protocol.RemoteId = remote_id;
        protocol.Data = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(json));
        protocol.ToServerId = server_id;
        protocol.ToSceneId = scene_id;
        protocol.FromServerId = this.server_info.ServerId;
        protocol.CallbackId = 0;

        if (callback != null)
        {
            int callback_id = ++MAX_CALLBACK_ID;
            this.remote_callback[callback_id] = callback;
            protocol.CallbackId = callback_id;
        }
        this.socket.SendMsgToRoute(protocol);
    }

    //远程调用返回
    public void RemoteResult(JObject json, int server_id, int callback_id)
    {
        if (this.remote_callback.ContainsKey(callback_id))
        {
            this.remote_callback[callback_id](json);
        }
        else
        {
            Log.Error("Not Remote CallBack Id:" + callback_id);
        }
    }
}

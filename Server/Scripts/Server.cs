using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

delegate void RemoteCallBack(JObject json);

class ServerInfo
{
    public int ConnectId;
    public int ServerId;
    public int ServerType;
    public List<int> SceneIdList = new List<int>();
}

class Server {
    private static Server Instance;
    private ServerInfo server_info;
    private Dictionary<Type, ServiceBase> service_map = new Dictionary<Type, ServiceBase>();
    private Dictionary<int, ServerInfo> server_id_to_server_info = new Dictionary<int, ServerInfo>();
    private Dictionary<int, ServerInfo> connect_id_to_server_info = new Dictionary<int, ServerInfo>();
    private Dictionary<int, List<ServerInfo>> server_type_to_server_list = new Dictionary<int, List<ServerInfo>>();
    private Dictionary<int, List<ServerInfo>> scene_id_to_server_list = new Dictionary<int, List<ServerInfo>>();
    private Dictionary<int, RemoteCallBack> remote_callback = new Dictionary<int, RemoteCallBack>();
    private int MAX_CALLBACK_ID = 0;
    private ProtocolDispatcher dispatcher = new ProtocolDispatcher();
    private ServerSocket socket;

    public static Server GetInstance()
    {
        if (Instance == null)
        {
            Instance = new Server();
        }
        return Instance;
    }

    public void Init(int server_id, int server_type, List<int> scene_list)
    {
        this.server_info = new ServerInfo();
        this.server_info.ServerId = server_id;
        this.server_info.ServerType = server_type;
        this.server_info.SceneIdList = scene_list;

        this.InitPath();
        this.InitTimer();
        this.InitService();
    }

    private void InitPath()
    {
        PathTool.SetRootPath(Environment.CurrentDirectory + "/Scripts");
        PathTool.SetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/FightForMoney");
    }

    private void InitTimer()
    {
        System.Timers.Timer timer = new System.Timers.Timer(50);
        timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => {
            TimerMgr.GetInstance().Update(50);
        });
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    public void InitService()
    {
        this.socket = new ServerSocket();

        foreach (var scene_id in this.server_info.SceneIdList)
        {
            switch (scene_id)
            {
                case SceneId.ROUTE:
                    {
                        this.service_map[typeof(RouteService)] = new RouteService();
                    }
                    break;
                case SceneId.LOGIN:
                    {
                        this.service_map[typeof(LoginService)] = new LoginService();
                    }
                    break;
                case SceneId.CHAT:
                    {
                        this.service_map[typeof(ChatService)] = new ChatService();
                    }
                    break;
                case SceneId.MAIL:
                    {
                        this.service_map[typeof(MailService)] = new MailService();
                    }
                    break;
                case SceneId.TEAM:
                    {

                    }
                    break;
                case SceneId.TRADE:
                    {

                    }
                    break;
                case SceneId.RELATION:
                    {

                    }
                    break;
                case SceneId.HOME:
                    {

                    }
                    break;
                default:
                    {
                        if (!this.service_map.ContainsKey(typeof(SceneService)))
                        {
                            this.service_map[typeof(SceneService)] = new SceneService();
                        }
                    }
                    break;
            }
        }

        foreach (ServiceBase service in this.service_map.Values)
        {
            service.Init();
        }
    }

    public void Listen(string ip, int port)
    {
        this.GetSocket().Listen(ip, port);
    }

    public void Connect(string ip, int port)
    {
        this.GetSocket().Connect(ip, port);
    }

    public void Release()
    {
        this.socket.Release();
    }

    public void AddHandler(int msg_id, Handler handler)
    {
        this.dispatcher.AddHandler(msg_id, handler);
    }

    public void AddCSHandler(int msg_id, CSHandler handler)
    {
        this.dispatcher.AddCSHandler(msg_id, handler);
    }

    public void HandleMsg(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        foreach (ServiceBase service in this.service_map.Values)
        {
            service.HandleMsg(protocol, connect_id, addition, player_id_list);
        }
    }

    public void DispatchMsg(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        this.dispatcher.Dispatch(protocol, connect_id, addition, player_id_list);
    }

    public ServerInfo GetServerInfo()
    {
        return this.server_info;
    }

    public T GetService<T>() where T : ServiceBase
    {
        return this.service_map.ContainsKey(typeof(T)) ? this.service_map[typeof(T)] as T : null;
    }

    public ServerSocket GetSocket()
    {
        return this.socket;
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
    public void RegisterToRoute()
    {
        SRRegisterServer protocol = new SRRegisterServer();
        protocol.ServerId = this.server_info.ServerId;
        protocol.ServerType = this.server_info.ServerType;
        foreach (int scene_id in this.server_info.SceneIdList)
        {
            protocol.SceneIdList.Add(scene_id);
        }

        List<int> connect_list = CustomNet.GetInstance().GetConnectList();
        foreach (int connect_id in connect_list)
        {
            Log.DebugFormat("Send To Route ConnectId:{0}", connect_id);
            this.socket.SendToServer(protocol, connect_id);
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

    public void DispatchToServer(int connect_id)
    {
        ServerInfo server_info = this.connect_id_to_server_info[connect_id];

        foreach (KeyValuePair<int, ServerInfo> pair in this.connect_id_to_server_info)
        {
            RSDispatchServer protocol;
            if (pair.Key != connect_id)
            {
                //把刚注册的服务器信息发送给其他服务器
                protocol = new RSDispatchServer();
                protocol.ConnectId = server_info.ConnectId;
                protocol.ServerId = server_info.ServerId;
                protocol.ServerType = server_info.ServerType;
                foreach (int scene_id in server_info.SceneIdList)
                {
                    protocol.SceneIdList.Add(scene_id);
                }
                this.socket.SendToServer(protocol, pair.Key);
            }

            //把其他服务器信息发送给刚注册的服务器
            protocol = new RSDispatchServer();
            protocol.ConnectId = pair.Value.ConnectId;
            protocol.ServerId = pair.Value.ServerId;
            protocol.ServerType = pair.Value.ServerType;
            foreach (int scene_id in pair.Value.SceneIdList)
            {
                protocol.SceneIdList.Add(scene_id);
            }
            this.socket.SendToServer(protocol, connect_id);
        }
    }

    public void SendMsgToAllServer(IMessage protocol)
    {
        foreach (KeyValuePair<int, ServerInfo> pair in this.connect_id_to_server_info)
        {
            this.socket.SendToServer(protocol, pair.Key);
        }
    }

    //远程调用
    public void RemoteCall(int remote_id, JObject json, int scene_id, int server_id = 0, RemoteCallBack callback = null)
    {
        ServerInfo server_info = this.GetServerInfoBySceneId(scene_id, server_id);
        if(server_info != null)
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
            this.socket.SendToServer(protocol, server_info.ConnectId);
        }
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

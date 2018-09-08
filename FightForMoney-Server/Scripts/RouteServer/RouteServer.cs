using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class RouteServer : ServerBase
{
    public RouteServer(int server_id, JToken config) : base(server_id, ServerType.ROUTE)
    {
        this.route_ip = config["Ip"].ToString();
        this.route_port = int.Parse(config["Port"].ToString());
        this.socket = new ServerSocket(this, new RouteMsgHandler(this));
    }

    public override void Init()
    {
        this.socket.RouteListen(this.route_ip, this.route_port);
    }

    public override void Update(int dt)
    {

    }

    public override void Release()
    {

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
                this.socket.SendMsgToServer(protocol, pair.Key);
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
            this.socket.SendMsgToServer(protocol, connect_id);
        }
    }

    public void SendMsgToAllServer(IMessage protocol)
    {
        foreach (KeyValuePair<int, ServerInfo> pair in this.connect_id_to_server_info)
        {
            this.socket.SendMsgToServer(protocol, pair.Key);
        }
    }
}
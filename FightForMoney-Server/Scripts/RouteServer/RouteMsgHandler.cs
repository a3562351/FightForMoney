using Common.Protobuf;
using Google.Protobuf;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

class RouteMsgHandler : MsgHandler
{
    private RouteServer server;

    public RouteMsgHandler(RouteServer server)
    {
        this.server = server;
    }

    public override void Handle(IMessage data, int connect_id, int addition, List<int> player_id_list)
    {
        short msg_id = Protocol.GetMsgCode(data);
        switch (msg_id)
        {
            //服务器注册到Route
            case MsgCode.SR_RegisterServer:
                {
                    SRRegisterServer protocol = data as SRRegisterServer;
                    List<int> scene_id_list = new List<int>();
                    foreach (int scene_id in protocol.SceneIdList)
                    {
                        scene_id_list.Add(scene_id);
                    }
                    this.server.RegisterServer(connect_id, protocol.ServerId, protocol.ServerType, scene_id_list);
                    this.server.DispatchToServer(connect_id);
                }
                break;
            //客户端登陆
            case MsgCode.CS_Login:
                {
                    ServerInfo server_info = this.server.GetServerInfoBySceneId(SceneId.LOGIN);
                    if (server_info == null)
                    {
                        Log.Debug("CS_Login LoginScene Not Exist");
                        return;
                    }

                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(connect_id);
                    if(player != null)
                    {
                        if (!player.IsOriginState())
                        {
                            Log.DebugFormat("CS_Login RouteRoleState Not OriginState UserId:{0} State:{1}", player.PlayerId, player.State);
                            return;
                        }
                    }
                    else
                    {
                        player = new RoutePlayer(connect_id);
                        RouteUserMgr.GetInstance().AddPlayerByConnectId(player);
                    }

                    //转发到登陆服验证账号信息
                    player.AddState(RoutePlayerState.LOGINING);
                    this.server.GetSocket().SendMsgToServer(data, server_info.ConnectId, connect_id, null);
                }
                break;
            //登陆服登陆结果返回
            case MsgCode.LR_LoginResult:
                {
                    LRLoginResult protocol = data as LRLoginResult;
                    SCLogin message = new SCLogin();

                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(addition);
                    if (protocol.ResultCode == NoticeCode.LoginSucc)
                    {
                        //登陆成功
                        player.UserId = protocol.UserId;
                        player.LoginKey = this.CreateLoginKey();

                        //切换成已登陆状态
                        player.CancelState(RoutePlayerState.LOGINING);
                        player.AddState(RoutePlayerState.LOGINED);

                        message.LoginKey = player.LoginKey;
                    }
                    else
                    {
                        //登陆失败重置状态
                        player.ResetState();
                    }

                    //登陆返回
                    message.ResultCode = protocol.ResultCode;
                    this.server.GetSocket().SendMsgToClient(message, addition);
                }
                break;
            //玩家列表返回
            case MsgCode.SC_PlayerList:
                {
                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(addition);
                    player.SendMsg(data);
                }
                break;
            //创建玩家
            case MsgCode.CS_CreatePlayer:
                {
                    CSCreatePlayer protocol = data as CSCreatePlayer;
                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(connect_id);
                    if (player == null || !player.CheckState(RoutePlayerState.LOGINED) || player.CheckState(RoutePlayerState.LOADED))
                    {
                        return;
                    }

                    protocol.UserId = player.UserId;
                    ServerInfo server_info = this.server.GetServerInfoBySceneId(SceneId.LOGIN);
                    this.server.GetSocket().SendMsgToServer(protocol, server_info.ConnectId, connect_id, null);
                }
                break;
            //加载玩家
            case MsgCode.CS_LoadPlayer:
                {
                    CSLoadPlayer protocol = data as CSLoadPlayer;
                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(connect_id);
                    if(player == null || !player.CheckState(RoutePlayerState.LOGINED) || player.CheckState(RoutePlayerState.LOADED))
                    {
                        return;
                    }

                    protocol.UserId = player.UserId;
                    player.LoadMsg = protocol;
                    ServerInfo server_info = this.server.GetServerInfoBySceneId(SceneId.LOGIN);
                    this.server.GetSocket().SendMsgToServer(protocol, server_info.ConnectId, connect_id, null);
                }
                break;
            //玩家顶号
            case MsgCode.LR_PlayerRepeat:
                {
                    LRPlayerRepeat protocol = data as LRPlayerRepeat;
                    RoutePlayer old_player = RouteUserMgr.GetInstance().GetPlayerByConnectId(protocol.PlayerId);
                    old_player.Logout();

                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(addition);
                    ServerInfo server_info = this.server.GetServerInfoBySceneId(SceneId.LOGIN);
                    this.server.GetSocket().SendMsgToServer(player.LoadMsg, server_info.ConnectId, connect_id, null);
                }
                break;
            //场景加载玩家数据完成
            case MsgCode.SR_LoadPlayerComplete:
                {
                    SRLoadPlayerComplete protocol = data as SRLoadPlayerComplete;

                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(addition);
                    player.PlayerId = protocol.PlayerId;
                    player.AddState(RoutePlayerState.LOADED);
                    player.LoadMsg = null;
                    RouteUserMgr.GetInstance().AddPlayerByPlayerId(player);
                }
                break;
            //客户端重连
            case MsgCode.CS_Reconnect:
                {
                    CSReconnect protocol = data as CSReconnect;
                    SCReconnect message = new SCReconnect();

                    RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByPlayerId(protocol.PlayerId);
                    if(player == null)
                    {
                        Log.DebugFormat("CS_Reconnect Not RoutePlayer Id:{0}", protocol.PlayerId);
                        message.ResultCode = NoticeCode.ReConnectFail;
                        this.server.GetSocket().SendMsgToClient(message, connect_id);
                        return;
                    }

                    if (!player.CheckState(RoutePlayerState.LOADED))
                    {
                        Log.DebugFormat("CS_Reconnect RouteUser Not Loaded Id:{0}", protocol.PlayerId);
                        message.ResultCode = NoticeCode.ReConnectFail;
                        this.server.GetSocket().SendMsgToClient(message, connect_id);
                        return;
                    }

                    if (player.LoginKey != protocol.LoginKey)
                    {
                        Log.DebugFormat("CS_Reconnect LoginKey Not Equal {0} {1}", player.LoginKey, protocol.LoginKey);
                        message.ResultCode = NoticeCode.ReConnectFail;
                        this.server.GetSocket().SendMsgToClient(message, connect_id);
                        return;
                    }

                    message.ResultCode = NoticeCode.ReconnectSucc;
                    this.server.GetSocket().SendMsgToClient(message, connect_id);

                    //重连更新用户连接
                    player.ConnectId = connect_id;
                    RouteUserMgr.GetInstance().AddPlayerByConnectId(player);
                    player.Reconnect();
                }
                break;
            //服务器间远程调用
            case MsgCode.SS_RemoteCall:
                {
                    SSRemoteCall protocol = data as SSRemoteCall;
                    ServerInfo from_server_info = this.server.GetServerInfoByConnectId(connect_id);
                    if(from_server_info != null)
                    {
                        protocol.FromServerId = from_server_info.ServerId;
                        ServerInfo to_server_info = this.server.GetServerInfoBySceneId(protocol.ToSceneId, protocol.ToServerId);
                        if (to_server_info != null)
                        {
                            this.server.GetSocket().SendMsgToServer(protocol, to_server_info.ConnectId);
                        }
                        else
                        {
                            Log.Debug("SS_RemoteCall ToServerInfo Not Exist SceneId:" + protocol.ToSceneId + " ServerId:" + protocol.ToServerId);
                        }
                    }
                    else
                    {
                        Log.Debug("SS_RemoteCall FromServerInfo Not Exist ConnectId:" + addition);
                    }
                }
                break;
            //服务器间远程调用返回
            case MsgCode.SS_RemoteResult:
                {
                    SSRemoteResult protocol = data as SSRemoteResult;
                    ServerInfo from_server_info = this.server.GetServerInfoByConnectId(addition);
                    if (from_server_info != null)
                    {
                        protocol.FromServerId = from_server_info.ServerId;
                        ServerInfo to_server_info = this.server.GetServerInfoByServerId(protocol.ToServerId);
                        if (to_server_info != null)
                        {
                            this.server.GetSocket().SendMsgToServer(protocol, to_server_info.ConnectId);
                        }
                    }
                }
                break;
            //切出原场景
            case MsgCode.SR_ChangeOutScene:
                {
                    SRChangeOutScene protocol = data as SRChangeOutScene;
                    ServerInfo to_server_info = this.server.GetServerInfoBySceneId(protocol.ToSceneId, protocol.ToServerId);
                    if(to_server_info != null)
                    {
                        RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByPlayerId(addition);
                        player.CurServerId = protocol.ToServerId;
                        player.CurSceneId = protocol.ToSceneId;

                        RSChangeInScene message = new RSChangeInScene();
                        message.PlayerStruct = protocol.PlayerStruct;
                        this.server.GetSocket().SendMsgToServer(message, to_server_info.ConnectId);
                    }
                    else
                    {
                        Log.Debug("SR_ChangeOutScene ToServerInfo Not Exist SceneId:" + protocol.ToSceneId + " ServerId:" + protocol.ToServerId);
                    }
                }
                break;

            default:
                {
                    ServerInfo server_info = this.server.GetServerInfoByConnectId(connect_id);
                    if(server_info != null)
                    {
                        //来自服务器的信息，转发给客户端
                        RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByPlayerId(addition);
                        player.SendMsg(data);
                    }
                    else
                    {
                        //来自客户端的信息，转发给服务器
                        RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(connect_id);
                        if (player == null || !player.CheckState(RoutePlayerState.LOADED))
                        {
                            return;
                        }

                        if (msg_id == MsgCode.SC_HeartBeat)
                        {
                            player.SetActive();
                            SCHeartBeat message = new SCHeartBeat();
                            player.SendMsg(message);
                            return;
                        }

                        //存在指定转发的场景ID
                        if (MsgCode.ProtocolSceneId.ContainsKey(msg_id))
                        {
                            int to_scene_id = MsgCode.ProtocolSceneId[msg_id];
                            ServerInfo to_server_info = this.server.GetServerInfoBySceneId(to_scene_id);
                            this.server.GetSocket().SendMsgToServer(data, to_server_info.ConnectId, player.PlayerId);
                        }
                        else
                        {
                            //默认转发到用户所在场景服
                            ServerInfo to_server_info = this.server.GetServerInfoByServerId(player.CurServerId);
                            this.server.GetSocket().SendMsgToServer(data, to_server_info.ConnectId, player.PlayerId);
                        }
                    }
                }
                break;
        }
    }

    public override void OnDisConnect(int connect_id)
    {
        RoutePlayer player = RouteUserMgr.GetInstance().GetPlayerByConnectId(connect_id);
        if(player != null)
        {
            RouteUserMgr.GetInstance().DelPlayerByConnectId(connect_id);
            if (player.CheckState(RoutePlayerState.LOGINED))
            {
                player.Disconnect();
                RSPlayerLogout message = new RSPlayerLogout();
                message.PlayerId = player.PlayerId;
                this.server.SendMsgToAllServer(message);
            }
        }
    }

    private string CreateLoginKey()
    {
        string key = (new Random().Next().GetHashCode()).ToString();
        byte[] bytes = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key));
        return BitConverter.ToString(bytes);
    }
}

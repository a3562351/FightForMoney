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

    public override void Handle(IMessage data, int connect_id, int addition, List<int> user_id_list)
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
                    User user = this.server.GetUserMgr().GetUserByConnectId(connect_id);
                    if(user != null)
                    {
                        if (user.State != 0)
                        {
                            Log.Debug("CS_Login UserState Not Invalid UserId:" + user.UserId + " State:" + user.State);
                            return;
                        }
                    }
                    else
                    {
                        user = new User(this.server, connect_id);
                        this.server.GetUserMgr().AddUserByConnectId(user);
                    }

                    ServerInfo server_info = this.server.GetServerInfoBySceneId(SceneId.LOGIN);
                    if(server_info == null)
                    {
                        Log.Debug("CS_Login LoginScene Not Exist");
                        return;
                    }

                    //转发到登陆服验证账号信息
                    user.AddState(UserState.LOGINING);
                    user.LoginMsg = data;   //用于顶号登陆
                    this.server.GetSocket().SendMsgToServer(data, server_info.ConnectId, connect_id, null);
                }
                break;
            //登陆服登陆结果返回
            case MsgCode.LR_LoginResult:
                {
                    LRLoginResult protocol = data as LRLoginResult;
                    SCLogin message = new SCLogin();

                    if (protocol.ResultCode == NoticeCode.LoginSucc)
                    {
                        //登陆成功
                        User user = this.server.GetUserMgr().GetUserByConnectId(addition);
                        user.UserId = protocol.UserId;
                        user.LoginKey = this.CreateLoginKey();
                        this.server.GetUserMgr().AddUserByUserId(user);

                        //切换成已登陆状态
                        user.CancelState(UserState.LOGINING);
                        user.AddState(UserState.LOGINED);

                        message.UserId = user.UserId;
                        message.LoginKey = user.LoginKey;
                    }
                    else if(protocol.ResultCode == NoticeCode.RepeatLogin)
                    {
                        //重复登陆
                        User user = this.server.GetUserMgr().GetUserByUserId(protocol.UserId);
                        if(user != null)
                        {
                            //被顶号用户立即登出
                            SCNotice msg = new SCNotice();
                            msg.NoticeCode = NoticeCode.BeRepeatLogin;
                            msg.Param.Add(this.server.GetSocket().GetConnectInfo(addition));
                            user.SendMsg(msg);
                            user.LoginOut();
                        }
                        else
                        {
                            //通知登陆服玩家早已离线
                            RSUserLoginout msg = new RSUserLoginout();
                            msg.UserId = user.UserId;
                            this.server.GetSocket().SendMsgToServer(msg, addition);
                        }

                        //顶号用户重新登陆
                        user = this.server.GetUserMgr().GetUserByConnectId(addition);
                        this.server.GetSocket().SendMsgToServer(user.LoginMsg, connect_id, user.ConnectId, null);
                    }
                    else
                    {
                        //登陆失败重置用户状态为空
                        User user = this.server.GetUserMgr().GetUserByConnectId(addition);
                        user.State = 0;
                    }

                    //登陆返回
                    message.ResultCode = protocol.ResultCode;
                    this.server.GetSocket().SendMsgToClient(message, addition);
                }
                break;
            //场景加载玩家数据完成
            case MsgCode.SR_LoadPlayerComplete:
                {
                    SRLoadPlayerComplete protocol = new SRLoadPlayerComplete();

                    User user = this.server.GetUserMgr().GetUserByUserId(addition);
                    user.CurServerId = protocol.ServerId;
                    user.CurSceneId = protocol.SceneId;
                }
                break;
            //客户端重连
            case MsgCode.CS_Reconnect:
                {
                    CSReconnect protocol = data as CSReconnect;
                    SCReconnect message = new SCReconnect();

                    User user = this.server.GetUserMgr().GetUserByUserId(protocol.UserId);
                    if(user == null)
                    {
                        Log.Debug("CS_Reconnect Not User Id:" + protocol.UserId);
                        return;
                    }

                    if (!user.CheckState(UserState.LOGINED))
                    {
                        Log.Debug("CS_Reconnect User Not Logined Id:" + protocol.UserId);
                        return;
                    }

                    if (user.LoginKey != protocol.LoginKey)
                    {
                        Log.DebugFormat("CS_Reconnect LoginKey Not Equal {0} {1}", user.LoginKey, protocol.LoginKey);
                        return;
                    }

                    //重连更新用户连接
                    user.ConnectId = connect_id;
                    this.server.GetUserMgr().AddUserByConnectId(user);
                    this.server.GetSocket().SendMsgToClient(message, connect_id);
                    user.Reconnect();
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
                        User user = this.server.GetUserMgr().GetUserByUserId(addition);
                        user.CurServerId = protocol.ToServerId;
                        user.CurSceneId = protocol.ToSceneId;

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
                        User user = this.server.GetUserMgr().GetUserByUserId(addition);
                        user.SendMsg(data);
                    }
                    else
                    {
                        //来自客户端的信息，转发给服务器
                        User user = this.server.GetUserMgr().GetUserByConnectId(connect_id);
                        if(user == null)
                        {
                            return;
                        }

                        if(msg_id == MsgCode.SC_HeartBeat)
                        {
                            user.SetActive();
                            SCHeartBeat message = new SCHeartBeat();
                            user.SendMsg(message);
                            return;
                        }

                        //存在指定转发的场景ID
                        if (MsgCode.ProtocolSceneId.ContainsKey(msg_id))
                        {
                            int to_scene_id = MsgCode.ProtocolSceneId[msg_id];
                            ServerInfo to_server_info = this.server.GetServerInfoBySceneId(to_scene_id);
                            this.server.GetSocket().SendMsgToServer(data, to_server_info.ConnectId, user.UserId);
                        }
                        else
                        {
                            //默认转发到用户所在场景服
                            ServerInfo to_server_info = this.server.GetServerInfoByServerId(user.CurServerId);
                            this.server.GetSocket().SendMsgToServer(data, to_server_info.ConnectId, user.UserId);
                        }
                    }
                }
                break;
        }
    }

    public override void OnDisConnect(int connect_id)
    {
        User user = this.server.GetUserMgr().GetUserByConnectId(connect_id);
        if(user != null)
        {
            this.server.GetUserMgr().DelUserByConnectId(connect_id);
            if (user.CheckState(UserState.LOGINED))
            {
                user.Disconnect();
                RSUserDisconnect message = new RSUserDisconnect();
                message.UserId = user.UserId;
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

using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class ProtocolInfo
{
    public IMessage Protocol;
    public int ConnectId;
    public int Addition;
    public List<int> UserIdList;
}

class ServerSocket
{
    private ServerBase server;
    private MsgHandler msg_handler;
    private Socket route_socket;    //路由服用的Socket
    private Socket server_socket;   //其他服用的Socket
    private ByteCache byte_cache = new ByteCache();
    private Dictionary<Socket, ByteCache> cache_map = new Dictionary<Socket, ByteCache>();
    private CustomDictionary<int, Socket> socket_map = new CustomDictionary<int, Socket>();
    private int max_connect_id = 0;
    private Object msg_lock = new Object();
    private List<ProtocolInfo> protocol_list = new List<ProtocolInfo>();

    public ServerSocket(ServerBase server, MsgHandler msg_handler)
    {
        this.server = server;
        this.msg_handler = msg_handler;

        TimerMgr.GetInstance().AddTimer(50, this.DispatchProtocol, null, true);
    }

    public void RouteListen(string ip, int port)
    {
        this.route_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.route_socket.ReceiveBufferSize = Const.BUFFSIZE;
        this.route_socket.SendBufferSize = Const.BUFFSIZE;
        this.route_socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        this.route_socket.Listen(10);
        Log.Debug("Route Socket创建成功:" + this.route_socket.LocalEndPoint.ToString());

        Thread thread = new Thread(ConnectListen);
        thread.IsBackground = true;
        thread.Start();
    }

    public void ConnectToRoute(string ip, int port)
    {
        this.server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.server_socket.ReceiveBufferSize = Const.BUFFSIZE;
        this.server_socket.SendBufferSize = Const.BUFFSIZE;
        this.server_socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

        Thread thread = new Thread(ReceiveRouteMsg);
        thread.IsBackground = true;
        thread.Start();
    }

    public void Release()
    {
        if (this.route_socket != null) this.route_socket.Close();
        if (this.server_socket != null) this.server_socket.Close();
    }

    public void AddHandler(Type type, Handler handler)
    {
        this.msg_handler.AddHandler(type, handler);
    }

    public void AddCSHandler(Type type, CSHandler handler)
    {
        this.msg_handler.AddCSHandler(type, handler);
    }

    private Socket GetSocket(int connect_id)
    {
        Socket socket = null;
        if (this.socket_map.HaveKey(connect_id))
        {
            socket = this.socket_map.GetValue(connect_id);
        }
        return socket;
    }

    private int CreateConnectId()
    {
        if(++this.max_connect_id > Const.MAX_CONNECT_ID)
        {
            this.max_connect_id = 1;
        }
        return this.max_connect_id;
    }

    private void ConnectListen()
    {
        while (true)
        {
            Socket accept_socket = this.route_socket.Accept();
            accept_socket.ReceiveBufferSize = Const.BUFFSIZE;
            accept_socket.SendBufferSize = Const.BUFFSIZE;
            this.socket_map.Add(this.CreateConnectId(), accept_socket);
            Log.Debug("Socket连接成功:" + accept_socket.RemoteEndPoint.ToString());

            Thread thread = new Thread(ReceiveMsg);
            thread.IsBackground = true;
            thread.Start(accept_socket);
        }
    }

    private void ReceiveRouteMsg()
    {
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = this.server_socket.Receive(data);
                //Log.Debug("服务端接收消息:" + this.socket.RemoteEndPoint.ToString() + "长度为:" + length);

                if (length > 0)
                {
                    this.byte_cache.AddBytes(data, length);

                    while (true)
                    {
                        byte[] msg_bytes = this.byte_cache.DivideMessage();
                        if (msg_bytes == null)
                        {
                            break;
                        }
                        this.UnPackProtocol(msg_bytes, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                this.server_socket.Shutdown(SocketShutdown.Both);
                this.server_socket.Close();
                break;
            }
        }
    }

    private void ReceiveMsg(object client)
    {
        Socket accept_socket = accept_socket = (Socket)client;
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = accept_socket.Receive(data);
                //Log.Debug("服务端接收消息:" + accept_socket.RemoteEndPoint.ToString() + "长度为:" + length);

                int connect_id;
                if (this.socket_map.HaveValue(accept_socket))
                {
                    connect_id = this.socket_map.GetKey(accept_socket);
                }
                else
                {
                    Exception e = new Exception("Socket对应的连接ID不存在!!!");
                    throw e;
                }

                if (length > 0)
                {
                    if (!this.cache_map.ContainsKey(accept_socket))
                    {
                        this.cache_map[accept_socket] = new ByteCache();
                    }
                    ByteCache cache = this.cache_map[accept_socket];
                    cache.AddBytes(data, length);

                    while (true)
                    {
                        byte[] msg_bytes = cache.DivideMessage();
                        if (msg_bytes == null)
                        {
                            break;
                        }
                        this.UnPackProtocol(msg_bytes, connect_id);
                    }
                }
                else
                {
                    this.msg_handler.OnDisConnect(connect_id);
                    Exception e = new Exception(string.Format("Socket已断开连接 ConnectId:{0}!!!", connect_id));
                    throw e;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                accept_socket.Shutdown(SocketShutdown.Both);
                accept_socket.Close();
                break;
            }
        }
    }

    private byte[] PackProtocol(IMessage protocol, int addition = 0, List<int> player_id_list = null)
    {
        ByteBuffer buffer = new ByteBuffer();

        //写入附加位
        buffer.WriteInt(addition);

        //写入玩家ID列表
        if(player_id_list != null)
        {
            buffer.WriteInt(player_id_list.Count);
            foreach (int user_id in player_id_list)
            {
                buffer.WriteInt(user_id);
            }
        }
        else
        {
            buffer.WriteInt(0);
        }

        //写入协议体
        byte[] protocol_bytes = Protocol.Encode(protocol);
        buffer.WriteBytes(protocol_bytes);

        byte[] msg_bytes = buffer.ToBytes();
        buffer.Clear();
        buffer.WriteInt(msg_bytes.Length);
        buffer.WriteBytes(Encrypt.Encode(msg_bytes, Encrypt.DefaultKey));
        msg_bytes = buffer.ToBytes();
        buffer.Close();

        return msg_bytes;
    }

    private void UnPackProtocol(byte[] msg_bytes, int connect_id)
    {
        ByteBuffer buffer = new ByteBuffer(Encrypt.Decode(msg_bytes, Encrypt.DefaultKey));

        //读出附加位
        int addition = buffer.ReadInt();

        //读出用户ID列表
        List<int> player_id_list = new List<int>();
        int list_lenght = buffer.ReadInt();
        for (int i = 1; i <= list_lenght; i++)
        {
            int user_id = buffer.ReadInt();
            player_id_list.Add(user_id);
        }

        //读出协议体
        byte[] protocol_bytes = buffer.ReadBytes((int)buffer.RemainingBytes());
        IMessage protocol = Protocol.Decode(protocol_bytes);
        buffer.Close();

        if (protocol != null)
        {
            Log.Debug("服务端接收消息:" + protocol.GetType() + " 数据:" + protocol.ToString());

            ProtocolInfo mProtocolInfo = new ProtocolInfo();
            mProtocolInfo.Protocol = protocol;
            mProtocolInfo.ConnectId = connect_id;
            mProtocolInfo.Addition = addition;
            mProtocolInfo.UserIdList = player_id_list;

            lock (msg_lock)
            {
                this.protocol_list.Add(mProtocolInfo);
            }
        }
    }

    public void DispatchProtocol(object data)
    {
        lock (msg_lock)
        {
            for (int i = 0; i < this.protocol_list.Count; i++)
            {
                try
                {
                    ProtocolInfo mProtocolInfo = this.protocol_list[i];
                    this.msg_handler.Handle(mProtocolInfo.Protocol, mProtocolInfo.ConnectId, mProtocolInfo.Addition, mProtocolInfo.UserIdList);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            this.protocol_list.Clear();
        }
    }

    //发往路由
    public void SendMsgToRoute(IMessage protocol, int addition = 0, List<int> player_id_list = null)
    {
        Log.Debug("服务端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
        this.server_socket.Send(this.PackProtocol(protocol, addition, player_id_list));
    }

    //发往服务器
    public void SendMsgToServer(IMessage protocol, int connect_id, int addition = 0, List<int> player_id_list = null)
    {
        Socket socket = this.GetSocket(connect_id);
        if(socket != null)
        {
            Log.Debug("服务端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
            socket.Send(this.PackProtocol(protocol, addition, player_id_list));
        }
    }

    //发往客户端
    public void SendMsgToClient(IMessage protocol, int connect_id)
    {
        Socket socket = this.GetSocket(connect_id);
        if (socket == null)
        {
            Log.Debug("玩家Socket不存在:" + connect_id);
            return;
        }
        Log.Debug("服务端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
        socket.Send(this.PackProtocol(protocol));
    }

    public string GetConnectInfo(int connect_id)
    {
        if (this.socket_map.HaveKey(connect_id))
        {
            Socket socket = this.socket_map.GetValue(connect_id);
            return socket.LocalEndPoint.ToString();
        }
        else
        {
            return "";
        }
    }
}

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
    public List<int> PlayerIdList;
}

class ServerSocket
{
    private Object msg_lock = new Object();
    private List<ProtocolInfo> protocol_list = new List<ProtocolInfo>();

    public ServerSocket()
    {
        TimerMgr.GetInstance().AddTimer(50, this.HandleMsg, null, true);
    }

    public void Listen(string ip, int port)
    {
        CustomNet.GetInstance().Listen(ip, port, ReceiveMsg);
    }

    public void Connect(string ip, int port)
    {
        CustomNet.GetInstance().Connect(ip, port, ReceiveMsg);
    }

    public void Release()
    {
        CustomNet.GetInstance().Release();
    }

    private void ReceiveMsg(object obj)
    {
        Socket socket = obj as Socket;
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = socket.Receive(data);
                //Log.Debug("服务端接收消息:" + socket.RemoteEndPoint.ToString() + "长度为:" + length);

                int connect_id = CustomNet.GetInstance().GetConnectId(socket);
                if (connect_id == 0)
                {
                    Exception e = new Exception("Socket对应的连接ID不存在!!!");
                    throw e;
                }

                if (length > 0)
                {
                    CustomNet.GetInstance().AddCacheByte(socket, data, length);
                    while (true)
                    {
                        byte[] msg_bytes = CustomNet.GetInstance().DivideMsg(socket);
                        if (msg_bytes == null)
                        {
                            break;
                        }
                        this.UnPackProtocol(msg_bytes, connect_id);
                    }
                }
                else
                {
                    Exception e = new Exception(string.Format("Socket已断开连接 ConnectId:{0}!!!", connect_id));
                    throw e;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
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
            mProtocolInfo.PlayerIdList = player_id_list;

            lock (msg_lock)
            {
                this.protocol_list.Add(mProtocolInfo);
            }
        }
    }

    public void HandleMsg(object data)
    {
        lock (msg_lock)
        {
            for (int i = 0; i < this.protocol_list.Count; i++)
            {
                try
                {
                    ProtocolInfo mProtocolInfo = this.protocol_list[i];
                    Server.GetInstance().HandleMsg(mProtocolInfo.Protocol, mProtocolInfo.ConnectId, mProtocolInfo.Addition, mProtocolInfo.PlayerIdList);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            this.protocol_list.Clear();
        }
    }

    //发往服务器
    public void SendToServer(IMessage protocol, int connect_id, int addition = 0, List<int> player_id_list = null)
    {
        if (!CustomNet.GetInstance().ExistConnect(connect_id))
        {
            Log.Debug("服务器Socket不存在:" + connect_id);
            return;
        }
        Log.Debug("服务端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
        CustomNet.GetInstance().SendData(connect_id, this.PackProtocol(protocol, addition, player_id_list));
    }

    //发往客户端
    public void SendToClient(IMessage protocol, int connect_id)
    {
        if (!CustomNet.GetInstance().ExistConnect(connect_id))
        {
            Log.Debug("玩家Socket不存在:" + connect_id);
            return;
        }
        Log.Debug("服务端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
        CustomNet.GetInstance().SendData(connect_id, this.PackProtocol(protocol));
    }
}

using UnityEngine;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ClientSocket
{
    private static ClientSocket Instance = null;
    private Socket socket;
    private ByteCache byte_cache = new ByteCache();
    private System.Object msg_lock = new System.Object();
    private List<IMessage> protocol_list = new List<IMessage>();
    private ProtocolDispatcher dispatcher = new ProtocolDispatcher();

    public static ClientSocket GetInstance()
    {
        if (Instance == null)
        {
            Instance = new ClientSocket();
        }
        return Instance;
    }

    public bool IsActive()
    {
        return this.socket != null;
    }

    public void Init()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Connect(new IPEndPoint(IPAddress.Parse(Const.IPSTR), Const.PORT));

        Thread thread = new Thread(ReceiveMsg);
        thread.IsBackground = true;
        thread.Start();
    }

    public void Release()
    {
        if (this.socket != null) this.socket.Close();
    }

    public void AddHandler(Type type, ClientHandler handler)
    {
        this.dispatcher.AddHandler(type, delegate(IMessage protocol, int connect_id, int addition, List<int> user_id_list) {
            handler(protocol);
        });
    }

    private void ReceiveMsg()
    {
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = this.socket.Receive(data);
                //Log.Debug("客户端接收消息:" + this.socket.RemoteEndPoint.ToString() + "长度为:" + length);

                if (length > 0)
                {
                    byte_cache.AddBytes(data, length);

                    while (true)
                    {
                        byte[] msg_bytes = byte_cache.DivideMessage();
                        if (msg_bytes == null)
                        {
                            break;
                        }
                        this.UnPackProtocol(msg_bytes);
                    }
                }
                else
                {
                    Exception e = new Exception(string.Format("Socket已断开连接!!!"));
                    throw e;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                break;
            }
        }
    }

    private byte[] PackProtocol(IMessage protocol)
    {
        ByteBuffer buffer = new ByteBuffer();

        //写入附加位
        buffer.WriteInt(0);

        //写入用户ID列表
        buffer.WriteInt(0);

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

    private void UnPackProtocol(byte[] msg_bytes)
    {
        ByteBuffer buffer = new ByteBuffer(Encrypt.Decode(msg_bytes, Encrypt.DefaultKey));

        //读出附加位
        int addition = buffer.ReadInt();

        //读出用户ID列表
        int list_lenght = buffer.ReadInt();

        //读出协议体
        byte[] protocol_bytes = buffer.ReadBytes((int)buffer.RemainingBytes());
        IMessage protocol = Protocol.Decode(protocol_bytes);
        buffer.Close();

        if (protocol != null)
        {
            Debug.Log("客户端接收消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
            lock (msg_lock)
            {
                this.protocol_list.Add(protocol);
            }
        }
    }

    public void DispatchProtocol()
    {
        lock (msg_lock)
        {
            foreach (IMessage protocol in this.protocol_list)
            {
                this.dispatcher.Dispatch(protocol, Const.UNDEFINED_CONNECT_ID, 0, null);
            }
            this.protocol_list.Clear();
        }
    }

    public void SendMessage(IMessage protocol)
    {
        Debug.Log("客户端发送消息:" + protocol.GetType() + " 数据:" + protocol.ToString());
        this.socket.Send(this.PackProtocol(protocol));
    }
}

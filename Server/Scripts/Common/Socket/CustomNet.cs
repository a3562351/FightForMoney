using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

delegate void NetCallBack(Socket socket);

class CustomNet
{
    private static CustomNet Instance;
    private RevDictionary<int, Socket> socket_map = new RevDictionary<int, Socket>();
    private Dictionary<Socket, ByteCache> cache_map = new Dictionary<Socket, ByteCache>();
    private List<int> connect_list = new List<int>();
    private ParameterizedThreadStart connect_callback;
    private int max_connect_id = 0;

    public static CustomNet GetInstance()
    {
        if (Instance == null)
        {
            Instance = new CustomNet();
        }
        return Instance;
    }

    private int CreateConnectId()
    {
        if (++this.max_connect_id > Const.MAX_CONNECT_ID)
        {
            this.max_connect_id = 1;
        }
        return this.max_connect_id;
    }

    public void Listen(string ip, int port, ParameterizedThreadStart callback)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.ReceiveBufferSize = Const.BUFFSIZE;
        socket.SendBufferSize = Const.BUFFSIZE;
        socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        socket.Listen(10);
        this.AddSocket(socket);
        this.connect_callback = callback;

        Thread thread = new Thread(ConnectListen);
        thread.IsBackground = true;
        thread.Start(socket);

        Log.Debug("Socket监听:" + socket.LocalEndPoint.ToString());
    }

    private void ConnectListen(object obj)
    {
        Socket socket = obj as Socket;
        while (true)
        {
            Socket accept_socket = socket.Accept();
            accept_socket.ReceiveBufferSize = Const.BUFFSIZE;
            accept_socket.SendBufferSize = Const.BUFFSIZE;
            CustomNet.GetInstance().AddSocket(accept_socket);
            Log.Debug("Socket连接成功:" + accept_socket.RemoteEndPoint.ToString());

            Thread thread = new Thread(this.connect_callback);
            thread.IsBackground = true;
            thread.Start(accept_socket);
        }
    }

    public void Connect(string ip, int port, ParameterizedThreadStart run)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.ReceiveBufferSize = Const.BUFFSIZE;
        socket.SendBufferSize = Const.BUFFSIZE;
        socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        this.connect_list.Add(this.AddSocket(socket));

        Thread thread = new Thread(run);
        thread.IsBackground = true;
        thread.Start(socket);

        Log.Debug("Socket连接:" + socket.LocalEndPoint.ToString());
    }

    public void Release()
    {

    }

    public int AddSocket(Socket socket)
    {
        int connect_id = this.CreateConnectId();
        this.socket_map.Add(connect_id, socket);
        return connect_id;
    }

    public void DelSocket(Socket socket)
    {
        this.socket_map.Remove(socket);
    }

    public bool ExistConnect(int connect_id)
    {
        return this.socket_map.HaveKey(connect_id);
    }

    public bool ExistSocket(Socket socket)
    {
        return this.socket_map.HaveValue(socket);
    }

    public int GetConnectId(Socket socket)
    {
        if (this.socket_map.HaveValue(socket))
        {
            return this.socket_map.GetKey(socket);
        }
        else
        {
            return 0;
        }
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

    public void AddCacheByte(Socket socket, byte[] data, int length)
    {
        if (!this.cache_map.ContainsKey(socket))
        {
            this.cache_map[socket] = new ByteCache();
        }
        this.cache_map[socket].AddBytes(data, length);
    }

    public byte[] DivideMsg(Socket socket)
    {
        if (this.cache_map.ContainsKey(socket))
        {
            return this.cache_map[socket].DivideMessage();
        }
        else
        {
            return null;
        }
    }

    public List<int> GetConnectList()
    {
        return this.connect_list;
    }

    public void SendData(int connect_id, byte[] buffer)
    {
        Socket socket = this.socket_map.GetValue(connect_id);
        if(socket != null)
        {
            socket.Send(buffer);
        }
    }
}

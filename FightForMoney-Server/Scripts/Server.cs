using Common.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Server {
    private static Server Instance = null;
    private ServerBase server;

    public static Server GetInstance()
    {
        if (Instance == null)
        {
            Instance = new Server();
        }
        return Instance;
    }

    public T GetServer<T>() where T : ServerBase
    {
        return this.server as T;
    }

    public ServerSocket GetSocket()
    {
        return this.server.GetSocket();
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

    public void Main(params string[] args)
    {
        this.InitPath();
        this.InitTimer();

        int server_id = int.Parse(args[0]);
        int server_type = int.Parse(args[1]);
        string config_name = args[2];

        string json = Global.ParseFileToJson(PathTool.GetServerConfigPath(config_name));
        JObject config = JObject.Parse(json);

        switch (server_type)
        {
            case ServerType.ROUTE:
                {
                    this.server = new RouteServer(server_id, config);
                    Log.Info("RunTime", "RouteServer!!!");
                }
                break;

            case ServerType.LOGIN:
                {
                    this.server = new LoginServer(server_id, config);
                    Log.Info("RunTime", "LoginServer!!!");
                }
                break;

            case ServerType.CHAT:
                {
                    this.server = new ChatServer(server_id, config);
                    Log.Info("RunTime", "ChatServer!!!");
                }
                break;
            case ServerType.MAIN:
                {
                    this.server = new SceneServer(server_id, config);
                    Log.Info("RunTime", "MainServer!!!");
                }
                break;
            case ServerType.INSTANCE:
                {
                    this.server = new SceneServer(server_id, config);
                    Log.Info("RunTime", "InstanceServer!!!");
                }
                break;
            case ServerType.COMMON:
                {
                    this.server = new CommonServer(server_id, config);
                    Log.Info("RunTime", "CommonServer!!!");
                }
                break;
        }

        this.server.Init();
    }
}

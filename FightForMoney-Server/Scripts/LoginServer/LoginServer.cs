using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LoginServer : ServerBase
{
    private LoginHandleMgr handle_mgr;

    public LoginServer(int server_id, JToken config) : base(server_id, ServerType.LOGIN)
    {
        foreach (int scene_id in config["SceneList"])
        {
            this.server_info.SceneIdList.Add(scene_id);
        }

        this.route_ip = config["Connect"]["Ip"].ToString();
        this.route_port = int.Parse(config["Connect"]["Port"].ToString());
        this.socket = new ServerSocket(this, new GameMsgHandler(this, new LoginRemoteHandler(this)));
        this.handle_mgr = new LoginHandleMgr(this);
    }

    public override void Init()
    {
        this.handle_mgr.Init();

        this.socket.ConnectToRoute(this.route_ip, this.route_port);
        this.RegisterToRoute();
    }

    public override void Update(int dt)
    {

    }

    public override void Release()
    {

    }
}

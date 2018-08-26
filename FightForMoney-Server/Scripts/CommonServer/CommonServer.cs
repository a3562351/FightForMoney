using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CommonServer : ServerBase
{
    public CommonServer(int server_id, JToken config) : base(server_id, ServerType.COMMON)
    {
        foreach (int scene_id in config["SceneList"])
        {
            this.server_info.SceneIdList.Add(scene_id);
        }

        this.route_ip = config["Connect"]["Ip"].ToString();
        this.route_port = int.Parse(config["Connect"]["Port"].ToString());
        this.socket = new ServerSocket(this, new GameMsgHandler(this, new CommonRemoteHandler(this)));
    }

    public override void Init()
    {
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

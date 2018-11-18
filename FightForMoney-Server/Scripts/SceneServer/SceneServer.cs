using Newtonsoft.Json.Linq;

class SceneServer : ServerBase
{
    private PlayerMgr player_mgr;
    private PlayerHandleMgr handle_mgr;

    public SceneServer(int server_id, JToken config) : base(server_id, ServerType.MAIN)
    {
        foreach (int scene_id in config["SceneList"])
        {
            this.server_info.SceneIdList.Add(scene_id);
        }

        this.route_ip = config["Connect"]["Ip"].ToString();
        this.route_port = int.Parse(config["Connect"]["Port"].ToString());
        this.socket = new ServerSocket(this, new GameMsgHandler(this, new SceneRemoteHandler(this)));
    }

    public override void Init()
    {
        this.player_mgr = new PlayerMgr();
        this.handle_mgr = new PlayerHandleMgr();

        this.socket.ConnectToRoute(this.route_ip, this.route_port);
        this.RegisterToRoute();
    }

    public override void Update(int dt)
    {
        this.player_mgr.Update(dt);
        SceneMgr.GetInstance().Update(dt);
    }

    public override void Release()
    {

    }
}

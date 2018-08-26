using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;

delegate void ChatServerHandle(IMessage data, int player_id);

class ChatServer : ServerBase
{
    private ChatMgr chat_mgr;

    public ChatServer(int server_id, JToken config) : base(server_id, ServerType.CHAT)
    {
        foreach (int scene_id in config["SceneList"])
        {
            this.server_info.SceneIdList.Add(scene_id);
        }

        this.route_ip = config["Connect"]["Ip"].ToString();
        this.route_port = int.Parse(config["Connect"]["Port"].ToString());
        this.socket = new ServerSocket(this, new GameMsgHandler(this, new ChatRemoteHandler(this)));
        this.chat_mgr = new ChatMgr(this);
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

    public void AddHandler(Type type, ChatServerHandle handle)
    {

    }

    public ChatMgr GetChatMgr()
    {
        return this.chat_mgr;
    }
}

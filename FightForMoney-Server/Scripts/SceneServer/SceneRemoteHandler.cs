using Common.Protobuf;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class SceneRemoteHandler : RemoteHandler
{
    private SceneServer server;

    public SceneRemoteHandler(SceneServer server) : base()
    {
        this.server = server;
    }

    public override JObject Handle(int remote_id, JObject data)
    {
        JObject json = new JObject();

        switch (remote_id)
        {
            case RemoteId.LS_LoadPlayer:
                {
                    int user_id = int.Parse(data["UserId"].ToString());
                    int player_id = int.Parse(data["PlayerId"].ToString());
                    int connect_id_in_route = int.Parse(data["ConnectIdInRoute"].ToString());
                    this.LoadPlayer(user_id, player_id, connect_id_in_route);
                }
                break;

            default:
                {

                }
                break;
        }

        return json;
    }

    private void LoadPlayer(int user_id, int player_id, int connect_id_in_route)
    {
        Player player = new Player();
        player.Init(DataTool.LoadPlayer(player_id));
        PlayerMgr.GetInstance().AddPlayer(player);
        Log.Debug("Load PlayerId:" + player.GetId());

        //通知路由服绑定玩家ID，后续可用player_id发信息
        {
            SRLoadPlayerComplete protocol = new SRLoadPlayerComplete();
            protocol.PlayerId = player.GetId();
            protocol.ServerId = Server.GetInstance().GetServerId();
            protocol.SceneId = SceneId.MAP;
            Server.GetInstance().GetSocket().SendMsgToRoute(protocol, connect_id_in_route);
        }

        //通知客户端进入场景
        {
            SCSceneEnter protocol = new SCSceneEnter();
            protocol.MapName = player.GetPlayerData().MapName;
            Server.GetInstance().GetSocket().SendMsgToRoute(protocol, player_id);
        }
    }
}

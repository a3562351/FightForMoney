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
                    this.LoadPlayer(user_id, player_id);
                }
                break;

            default:
                {

                }
                break;
        }

        return json;
    }

    private void LoadPlayer(int user_id, int player_id)
    {
        Player player = new Player(this.server, user_id);
        player.Init(DataTool.LoadPlayer(player_id));
        PlayerMgr.GetInstance().AddPlayer(player);
        Log.Debug("Load PlayerId:" + player_id);

        player.OnLogin();
    }
}

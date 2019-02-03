using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class SceneService : ServiceBase
{
    private SceneMgr scene_mgr;
    private PlayerMgr player_mgr;

    public override void Init()
    {
        this.scene_mgr = new SceneMgr();
        this.player_mgr = new PlayerMgr();
        this.msg_handler = new GameMsgHandler(new SceneRemoteHandler());
    }

    public override void HandleMsg(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        this.msg_handler.Handle(protocol, connect_id, addition, player_id_list);
    }

    public SceneMgr GetSceneMgr()
    {
        return this.scene_mgr;
    }

    public PlayerMgr GetPlayerMgr()
    {
        return this.player_mgr;
    }
}

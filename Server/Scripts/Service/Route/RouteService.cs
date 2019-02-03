using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class RouteService : ServiceBase
{
    private RouteUserMgr user_mgr;

    public override void Init()
    {
        this.user_mgr = new RouteUserMgr();
        this.msg_handler = new RouteMsgHandler();
    }

    public override void HandleMsg(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        this.msg_handler.Handle(protocol, connect_id, addition, player_id_list);
    }

    public RouteUserMgr GetUserMgr()
    {
        return this.user_mgr;
    }
}
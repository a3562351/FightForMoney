using Google.Protobuf;
using System;
using System.Collections.Generic;

//Server-To-Server Handler
delegate void Handler(IMessage protocol, int connect_id, int addition, List<int> player_id_list);
//Server-To-Client Handler
delegate void SCHandler(IMessage protocol);
//Client-To-Server Handler
delegate void CSHandler(IMessage protocol, int connect_id, int addition);

class ProtocolDispatcher
{
    private Dictionary<int, Handler> handle_map = new Dictionary<int, Handler>();

    public void AddHandler(int msg_id, Handler handler)
    {
        this.handle_map[msg_id] = handler;
    }

    public void AddSCHandle(int msg_id, SCHandler handler)
    {
        this.AddHandler(msg_id, delegate (IMessage protocol, int connect_id, int addition, List<int> player_id_list) {
            handler(protocol);
        });
    }

    public void AddCSHandler(int msg_id, CSHandler handler)
    {
        this.AddHandler(msg_id, delegate (IMessage protocol, int connect_id, int addition, List<int> player_id_list) {
            handler(protocol, connect_id, addition);
        });
    }

    public void Dispatch(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        int msg_id = Protocol.GetMsgId(protocol);
        if (!handle_map.ContainsKey(msg_id))
        {
            Log.WarnFormat("协议类型没有注册处理函数:{0}", msg_id);
            return;
        }

        Handler handler = handle_map[msg_id];
        handler(protocol, connect_id, addition, player_id_list);
    }
}
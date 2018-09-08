﻿using Google.Protobuf;
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
    private Dictionary<Type, Handler> handle_map = new Dictionary<Type, Handler>();

    public void AddHandler(Type type, Handler handler)
    {
        this.handle_map[type] = handler;
    }

    public void AddSCHandle(Type type, SCHandler handler)
    {
        this.AddHandler(type, delegate (IMessage protocol, int connect_id, int addition, List<int> player_id_list) {
            handler(protocol);
        });
    }

    public void AddCSHandler(Type type, CSHandler handler)
    {
        this.AddHandler(type, delegate (IMessage protocol, int connect_id, int addition, List<int> player_id_list) {
            handler(protocol, connect_id, addition);
        });
    }

    public void Dispatch(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        Type type = protocol.GetType();
        if (!handle_map.ContainsKey(type))
        {
            Log.Debug("协议类型没有注册处理函数:" + type.ToString());
            return;
        }

        Handler handler = handle_map[type];
        handler(protocol, connect_id, addition, player_id_list);
    }
}
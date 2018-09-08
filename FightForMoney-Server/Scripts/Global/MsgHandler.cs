using Google.Protobuf;
using System;
using System.Collections.Generic;

class MsgHandler
{
    protected ProtocolDispatcher dispatcher = new ProtocolDispatcher();

    public void AddHandler(Type type, Handler handler)
    {
        this.dispatcher.AddHandler(type, handler);
    }

    public void AddCSHandler(Type type, CSHandler handler)
    {
        this.dispatcher.AddCSHandler(type, handler);
    }

    public virtual void Handle(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {

    }

    public virtual void OnDisConnect(int connect_id)
    {

    }
}

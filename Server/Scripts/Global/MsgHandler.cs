using Google.Protobuf;
using System;
using System.Collections.Generic;

class MsgHandler
{
    public virtual void Handle(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {

    }

    public virtual void OnDisConnect(int connect_id)
    {

    }
}
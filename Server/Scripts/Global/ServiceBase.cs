using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ServiceBase
{
    protected MsgHandler msg_handler = new GameMsgHandler(new RemoteHandler());

    public virtual void Init()
    {

    }

    public virtual void Update(int dt)
    {

    }

    public virtual void Release()
    {

    }

    public virtual void HandleMsg(IMessage protocol, int connect_id, int addition, List<int> player_id_list)
    {
        this.msg_handler.Handle(protocol, connect_id, addition, player_id_list);
    }
}

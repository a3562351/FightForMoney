using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ChatRemoteHandler : RemoteHandler
{
    private ChatServer server;

    public ChatRemoteHandler(ChatServer server) : base()
    {
        this.server = server;
    }
}

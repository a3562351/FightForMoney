using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LoginRemoteHandler : RemoteHandler
{
    private LoginServer server;

    public LoginRemoteHandler(LoginServer server) : base()
    {
        this.server = server;
    }
}

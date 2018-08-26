using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CommonRemoteHandler : RemoteHandler
{
    private CommonServer server;

    public CommonRemoteHandler(CommonServer server) : base()
    {
        this.server = server;
    }
}

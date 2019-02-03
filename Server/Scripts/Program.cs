using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            int server_id = int.Parse(args[0]);

            Config cServer = ConfigPool.Load("Server");
            ConfigItem cServerItem = cServer[server_id];

            int server_type = int.Parse(cServerItem["ServerType"].ToString());
            List<int> scene_list = new List<int>();
            foreach (var scene_id in cServerItem["SceneList"])
            {
                scene_list.Add(int.Parse(scene_id.ToString()));
            }

            Server.GetInstance().Init(server_id, server_type, scene_list);
            if (server_type == ServerType.ROUTE)
            {
                string ip = cServerItem["IP"].ToString();
                int port = int.Parse(cServerItem["Port"].ToString());
                Server.GetInstance().Listen(ip, port);
            }
            else
            {
                foreach (var info in cServerItem["Connect"])
                {
                    string ip = info[0].ToString();
                    int port = int.Parse(info[1].ToString());
                    Server.GetInstance().Connect(ip, port);
                }
            }

            if (server_type != ServerType.ROUTE)
            {
                Server.GetInstance().RegisterToRoute();
            }
        }
        catch(Exception e)
        {
            Log.Error(e.ToString());
        }

        Thread.CurrentThread.Join();
        Server.GetInstance().Release();
    }
}
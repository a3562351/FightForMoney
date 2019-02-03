using Common.Protobuf;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class GameMsgHandler : MsgHandler
{
    private RemoteHandler remote_handler;

    public GameMsgHandler(RemoteHandler remote_handler)
    {
        this.remote_handler = remote_handler != null ? remote_handler : new RemoteHandler();
    }

    public override void Handle(IMessage data, int connect_id, int addition, List<int> player_id_list)
    {
        short msg_id = Protocol.GetMsgId(data);
        switch (msg_id)
        {
            case MsgCode.RS_DispatchServer:
                {
                    RSDispatchServer protocol = data as RSDispatchServer;
                    List<int> scene_id_list = new List<int>();
                    foreach (int scene_id in protocol.SceneIdList)
                    {
                        scene_id_list.Add(scene_id);
                    }
                    Server.GetInstance().RegisterServer(connect_id, protocol.ServerId, protocol.ServerType, scene_id_list);
                }
                break;

            case MsgCode.SS_RemoteCall:
                {
                    SSRemoteCall protocol = data as SSRemoteCall;
                    JObject json = JObject.Parse(protocol.Data.ToStringUtf8());
                    JObject result_json = this.remote_handler.Handle(protocol.RemoteId, json);
                    if(protocol.CallbackId > 0)
                    {
                        SSRemoteResult message = new SSRemoteResult();
                        message.Data = ByteString.CopyFromUtf8(result_json.ToString());
                        message.ToServerId = protocol.FromServerId;
                        message.FromServerId = Server.GetInstance().GetServerInfo().ServerId;
                        message.CallbackId = protocol.CallbackId;
                        Server.GetInstance().GetSocket().SendToServer(message, connect_id);
                    }
                }
                break;

            case MsgCode.SS_RemoteResult:
                {
                    SSRemoteResult protocol = data as SSRemoteResult;
                    JObject json = JsonConvert.DeserializeObject<JObject>(protocol.Data.ToStringUtf8());
                    Server.GetInstance().RemoteResult(json, protocol.FromServerId, protocol.CallbackId);
                }
                break;

            default:
                {
                    Server.GetInstance().DispatchMsg(data, connect_id, addition, player_id_list);
                }
                break;
        }
    }
}

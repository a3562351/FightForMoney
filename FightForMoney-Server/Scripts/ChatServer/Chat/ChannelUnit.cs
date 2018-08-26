using Common.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ChannelUnit
{
    private ChatServer server;
    private int channel_type;
    private int unit_no;
    private Dictionary<int, bool> player_id_map = new Dictionary<int, bool>();
    private List<ChatData> data_list = new List<ChatData>();

    public ChannelUnit(ChatServer server, int channel_type, int unit_no)
    {
        this.server = server;
        this.channel_type = channel_type;
        this.unit_no = unit_no;
    }

    public void Enter(int player_id)
    {
        if (this.player_id_map.ContainsKey(player_id))
        {
            return;
        }

        this.player_id_map[player_id] = true;
    }

    public void Leave(int player_id)
    {
        if (!this.player_id_map.ContainsKey(player_id))
        {
            return;
        }

        this.player_id_map.Remove(player_id);
    }

    public void AddChatContent(int player_id, string content)
    {
        ChatData data = new ChatData();
        data.ChatPlayer = this.server.GetChatMgr().GetPlayer(player_id);
        data.Content = content;
        data.Time = Global.GetCurTime();
        this.data_list.Add(data);

        this.Dispatch(data);
    }

    public void Dispatch(ChatData data)
    {
        SCPublicChat protocol = new SCPublicChat();
        protocol.ChannelType = this.channel_type;
        protocol.UnitNo = this.unit_no;
        protocol.Data = data;

        foreach (KeyValuePair<int, bool> pair in this.player_id_map)
        {
            this.server.GetSocket().SendMsgToClient(protocol, pair.Key);
        }
    }
}

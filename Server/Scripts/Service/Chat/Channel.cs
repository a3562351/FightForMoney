using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Channel
{
    private int channel_type;
    private Dictionary<int, ChannelUnit> unit_map = new Dictionary<int, ChannelUnit>();
    private Dictionary<int, int> player_id_to_unit_no = new Dictionary<int, int>();

    public Channel(int channel_type)
    {
        this.channel_type = channel_type;
    }

    public void Enter(int player_id, int unit_no)
    {
        if (this.player_id_to_unit_no.ContainsKey(player_id))
        {
            //不需要重复进入
            if (this.player_id_to_unit_no[player_id] == unit_no)
            {
                return;
            }
        }

        ChannelUnit channel_unit = this.GetChannelUnit(unit_no);
        if (channel_unit == null)
        {
            return;
        }
        this.Leave(player_id);
        channel_unit.Enter(player_id);
        this.player_id_to_unit_no[player_id] = unit_no;
    }

    public void Leave(int player_id)
    {
        if (!this.player_id_to_unit_no.ContainsKey(player_id))
        {
            return;
        }

        int unit_no = this.player_id_to_unit_no[player_id];
        this.player_id_to_unit_no.Remove(player_id);
        ChannelUnit channel_unit = this.GetChannelUnit(unit_no);
        if(channel_unit == null)
        {
            return;
        }
        channel_unit.Leave(player_id);
    }

    public void PublicChat(int player_id, string content)
    {
        if (!this.player_id_to_unit_no.ContainsKey(player_id))
        {
            return;
        }

        int unit_no = this.player_id_to_unit_no[player_id];
        ChannelUnit channel_unit = this.GetChannelUnit(unit_no);
        if (channel_unit == null)
        {
            return;
        }
        channel_unit.AddChatContent(player_id, content);
    }

    public ChannelUnit GetChannelUnit(int unit_no)
    {
        if (!this.unit_map.ContainsKey(unit_no))
        {
            if(unit_no <= ChannelUnitCount.MAX[this.channel_type])
            {
                this.unit_map[unit_no] = new ChannelUnit(this.channel_type, unit_no);
            }
            else
            {
                return null;
            }
        }
        return this.unit_map[unit_no];
    }
}

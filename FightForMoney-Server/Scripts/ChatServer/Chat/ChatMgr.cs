using Common.Protobuf;
using Google.Protobuf;
using System.Collections.Generic;

class ChatMgr
{
    private ChatServer server;
    private Dictionary<int, ChatPlayer> player_map = new Dictionary<int, ChatPlayer>();
    private Dictionary<int, Channel> channel_map = new Dictionary<int, Channel>();

    public ChatMgr(ChatServer server)
    {
        this.server = server;
    }

    public void Init()
    {
        this.server.AddHandler(typeof(CSEnterChannel), delegate(IMessage data, int player_id)
        {
            if (this.GetPlayer(player_id) == null)
            {
                return;
            }
            CSEnterChannel protocol = data as CSEnterChannel;
            this.EnterChannel(player_id, protocol.ChannelType, protocol.UnitNo);
        });

        this.server.AddHandler(typeof(CSLeaveChannel), delegate (IMessage data, int player_id)
        {
            if (this.GetPlayer(player_id) == null)
            {
                return;
            }
            CSLeaveChannel protocol = data as CSLeaveChannel;
            this.LeaveChannel(player_id, protocol.ChannelType);
        });

        this.server.AddHandler(typeof(CSPublicChat), delegate (IMessage data, int player_id)
        {
            if (this.GetPlayer(player_id) == null)
            {
                return;
            }
            CSPublicChat protocol = data as CSPublicChat;
            this.PublicChat(player_id, protocol.ChannelType, protocol.Content);
        });

        this.server.AddHandler(typeof(CSPrivateChat), delegate (IMessage data, int player_id)
        {
            if (this.GetPlayer(player_id) == null)
            {
                return;
            }
            CSPrivateChat protocol = data as CSPrivateChat;
            this.PrivateChat(player_id, protocol.PlayerId, protocol.Content);
        });
    }

    public ChatPlayer GetPlayer(int player_id)
    {
        if (!this.player_map.ContainsKey(player_id))
        {
            return null;
        }
        return this.player_map[player_id];
    }

    //玩家上线
    public void AddPlayer(ChatPlayer player)
    {
        this.player_map[player.Id] = player;
    }

    //玩家下线
    public void DelPlayer(int player_id)
    {
        if (this.player_map.ContainsKey(player_id))
        {
            this.player_map.Remove(player_id);
        }

        for(int i = ChannelType.START + 1; i < ChannelType.END; i++)
        {
            this.LeaveChannel(player_id, i);
        }
    }

    //进入频道
    public void EnterChannel(int player_id, int channel_type, int unit_no)
    {
        Channel chat_channel = this.GetChannel(channel_type);
        if (chat_channel == null)
        {
            return;
        }
        chat_channel.Enter(player_id, unit_no);
    }

    //离开频道
    public void LeaveChannel(int player_id, int channel_type)
    {
        Channel chat_channel = this.GetChannel(channel_type);
        if (chat_channel == null)
        {
            return;
        }
        chat_channel.Leave(player_id);
    }

    //公聊
    public void PublicChat(int player_id, int channel_type, string content)
    {
        if (!ChannelOpen.LIST.Contains(channel_type))
        {
            return;
        }

        Channel chat_channel = this.GetChannel(channel_type);
        if (chat_channel == null)
        {
            return;
        }

        chat_channel.PublicChat(player_id, content);
    }

    //私聊，目前只支持在线发送
    public void PrivateChat(int player_id, int to_player_id, string content)
    {
        ChatPlayer player = this.GetPlayer(player_id);
        ChatPlayer target_player = this.GetPlayer(to_player_id);
        if(player == null || target_player == null)
        {
            return;
        }

        ChatData data = new ChatData();
        data.ChatPlayer = player;
        data.Content = content;
        data.Time = Global.GetCurTime();

        SCPrivateChat protocol = new SCPrivateChat();
        protocol.Data = data;
        this.server.GetSocket().SendMsgToClient(protocol, target_player.Id);
    }

    private Channel GetChannel(int channel_type)
    {
        if (!this.channel_map.ContainsKey(channel_type))
        {
            this.channel_map[channel_type] = new Channel(this.server, channel_type);
        }
        return this.channel_map[channel_type];
    }
}

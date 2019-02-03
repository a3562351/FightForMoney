using Common.Protobuf;
using Google.Protobuf;
using System.Collections.Generic;

class ChatService : ServiceBase
{
    private Dictionary<int, ChatPlayer> player_map = new Dictionary<int, ChatPlayer>();
    private Dictionary<int, Channel> channel_map = new Dictionary<int, Channel>();

    public override void Init()
    {
        this.msg_handler = new GameMsgHandler(new LoginRemoteHandler());

        Server.GetInstance().AddCSHandler(MsgCode.CS_EnterChannel, this.CSEnterChannel);
        Server.GetInstance().AddCSHandler(MsgCode.CS_LeaveChannel, this.CSLeaveChannel);
        Server.GetInstance().AddCSHandler(MsgCode.CS_PublicChat, this.CSPublicChat);
        Server.GetInstance().AddCSHandler(MsgCode.CS_PrivateChat, this.CSPrivateChat);
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
    }

    //进入频道
    public void CSEnterChannel(IMessage data, int player_id, int addition)
    {
        CSEnterChannel protocol = data as CSEnterChannel;
        Channel chat_channel = this.GetChannel(protocol.ChannelType);
        if (chat_channel == null)
        {
            return;
        }
        chat_channel.Enter(player_id, protocol.UnitNo);
    }

    //离开频道
    public void CSLeaveChannel(IMessage data, int player_id, int addition)
    {
        CSLeaveChannel protocol = data as CSLeaveChannel;
        Channel chat_channel = this.GetChannel(protocol.ChannelType);
        if (chat_channel == null)
        {
            return;
        }
        chat_channel.Leave(player_id);
    }

    //公聊
    public void CSPublicChat(IMessage data, int player_id, int addition)
    {
        CSPublicChat protocol = data as CSPublicChat;
        if (!ChannelOpen.LIST.Contains(protocol.ChannelType))
        {
            return;
        }

        Channel chat_channel = this.GetChannel(protocol.ChannelType);
        if (chat_channel == null)
        {
            return;
        }

        chat_channel.PublicChat(player_id, protocol.Content);
    }

    //私聊，目前只支持在线发送
    public void CSPrivateChat(IMessage data, int player_id, int addition)
    {
        CSPrivateChat protocol = data as CSPrivateChat;

        ChatPlayer player = this.GetPlayer(player_id);
        ChatPlayer target_player = this.GetPlayer(protocol.PlayerId);
        if(player == null || target_player == null)
        {
            return;
        }

        ChatData chat_data = new ChatData();
        chat_data.ChatPlayer = player;
        chat_data.Content = protocol.Content;
        chat_data.Time = Global.GetCurTime();

        SCPrivateChat msg = new SCPrivateChat();
        msg.Data = chat_data;
        Server.GetInstance().GetSocket().SendToClient(msg, target_player.Id);
    }

    private Channel GetChannel(int channel_type)
    {
        if (!this.channel_map.ContainsKey(channel_type))
        {
            this.channel_map[channel_type] = new Channel(channel_type);
        }
        return this.channel_map[channel_type];
    }
}

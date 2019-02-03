using Common.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;

class MsgCode
{
	public const short CS_BattleBehavior = 10001;
	public const short SC_BattleBehavior = 10002;
	public const short SC_BattleStateUpdate = 10003;
	public const short CS_EnterChannel = 10004;
	public const short CS_LeaveChannel = 10005;
	public const short SC_ChangeChannel = 10006;
	public const short CS_PublicChat = 10007;
	public const short SC_PublicChat = 10008;
	public const short CS_PrivateChat = 10009;
	public const short SC_PrivateChat = 10010;
	public const short SC_ItemData = 10011;
	public const short CS_ItemUse = 10012;
	public const short SC_MapData = 10013;
	public const short SC_MapInfoChange = 10014;
	public const short CS_TerrainBuy = 10015;
	public const short CS_BuildAdd = 10016;
	public const short CS_BuildRemove = 10017;
	public const short CS_BuildUpgrade = 10018;
	public const short SC_MapInfo = 10019;
	public const short CS_CreatePlayer = 10020;
	public const short CS_LoadPlayer = 10021;
	public const short SC_PlayerList = 10022;
	public const short SC_PlayerInfo = 10023;
	public const short SC_SceneEnter = 10024;
	public const short CS_SceneEnter = 10025;
	public const short SR_RegisterServer = 10026;
	public const short RS_DispatchServer = 10027;
	public const short LR_LoginResult = 10028;
	public const short LR_PlayerRepeat = 10029;
	public const short SS_RemoteCall = 10030;
	public const short SS_RemoteResult = 10031;
	public const short RS_PlayerLogin = 10032;
	public const short RS_PlayerDisconnect = 10033;
	public const short RS_PlayerLogout = 10034;
	public const short SR_LoadPlayerComplete = 10035;
	public const short SR_ChangeOutScene = 10036;
	public const short RS_ChangeInScene = 10037;
	public const short CS_Login = 10038;
	public const short SC_Login = 10039;
	public const short CS_Reconnect = 10040;
	public const short SC_Reconnect = 10041;
	public const short CS_HeartBeat = 10042;
	public const short SC_HeartBeat = 10043;
	public const short SC_Notice = 10044;

	public static Dictionary<short, MessageParser> ProtocolParser = new Dictionary<short, MessageParser>() {
		{MsgCode.CS_BattleBehavior, CSBattleBehavior.Parser},
		{MsgCode.SC_BattleBehavior, SCBattleBehavior.Parser},
		{MsgCode.SC_BattleStateUpdate, SCBattleStateUpdate.Parser},
		{MsgCode.CS_EnterChannel, CSEnterChannel.Parser},
		{MsgCode.CS_LeaveChannel, CSLeaveChannel.Parser},
		{MsgCode.SC_ChangeChannel, SCChangeChannel.Parser},
		{MsgCode.CS_PublicChat, CSPublicChat.Parser},
		{MsgCode.SC_PublicChat, SCPublicChat.Parser},
		{MsgCode.CS_PrivateChat, CSPrivateChat.Parser},
		{MsgCode.SC_PrivateChat, SCPrivateChat.Parser},
		{MsgCode.SC_ItemData, SCItemData.Parser},
		{MsgCode.CS_ItemUse, CSItemUse.Parser},
		{MsgCode.SC_MapData, SCMapData.Parser},
		{MsgCode.SC_MapInfoChange, SCMapInfoChange.Parser},
		{MsgCode.CS_TerrainBuy, CSTerrainBuy.Parser},
		{MsgCode.CS_BuildAdd, CSBuildAdd.Parser},
		{MsgCode.CS_BuildRemove, CSBuildRemove.Parser},
		{MsgCode.CS_BuildUpgrade, CSBuildUpgrade.Parser},
		{MsgCode.SC_MapInfo, SCMapInfo.Parser},
		{MsgCode.CS_CreatePlayer, CSCreatePlayer.Parser},
		{MsgCode.CS_LoadPlayer, CSLoadPlayer.Parser},
		{MsgCode.SC_PlayerList, SCPlayerList.Parser},
		{MsgCode.SC_PlayerInfo, SCPlayerInfo.Parser},
		{MsgCode.SC_SceneEnter, SCSceneEnter.Parser},
		{MsgCode.CS_SceneEnter, CSSceneEnter.Parser},
		{MsgCode.SR_RegisterServer, SRRegisterServer.Parser},
		{MsgCode.RS_DispatchServer, RSDispatchServer.Parser},
		{MsgCode.LR_LoginResult, LRLoginResult.Parser},
		{MsgCode.LR_PlayerRepeat, LRPlayerRepeat.Parser},
		{MsgCode.SS_RemoteCall, SSRemoteCall.Parser},
		{MsgCode.SS_RemoteResult, SSRemoteResult.Parser},
		{MsgCode.RS_PlayerLogin, RSPlayerLogin.Parser},
		{MsgCode.RS_PlayerDisconnect, RSPlayerDisconnect.Parser},
		{MsgCode.RS_PlayerLogout, RSPlayerLogout.Parser},
		{MsgCode.SR_LoadPlayerComplete, SRLoadPlayerComplete.Parser},
		{MsgCode.SR_ChangeOutScene, SRChangeOutScene.Parser},
		{MsgCode.RS_ChangeInScene, RSChangeInScene.Parser},
		{MsgCode.CS_Login, CSLogin.Parser},
		{MsgCode.SC_Login, SCLogin.Parser},
		{MsgCode.CS_Reconnect, CSReconnect.Parser},
		{MsgCode.SC_Reconnect, SCReconnect.Parser},
		{MsgCode.CS_HeartBeat, CSHeartBeat.Parser},
		{MsgCode.SC_HeartBeat, SCHeartBeat.Parser},
	};

	public static Dictionary<Type, short> ProtocolMap = new Dictionary<Type, short>(){
		{typeof(CSBattleBehavior), MsgCode.CS_BattleBehavior},
		{typeof(SCBattleBehavior), MsgCode.SC_BattleBehavior},
		{typeof(SCBattleStateUpdate), MsgCode.SC_BattleStateUpdate},
		{typeof(CSEnterChannel), MsgCode.CS_EnterChannel},
		{typeof(CSLeaveChannel), MsgCode.CS_LeaveChannel},
		{typeof(SCChangeChannel), MsgCode.SC_ChangeChannel},
		{typeof(CSPublicChat), MsgCode.CS_PublicChat},
		{typeof(SCPublicChat), MsgCode.SC_PublicChat},
		{typeof(CSPrivateChat), MsgCode.CS_PrivateChat},
		{typeof(SCPrivateChat), MsgCode.SC_PrivateChat},
		{typeof(SCItemData), MsgCode.SC_ItemData},
		{typeof(CSItemUse), MsgCode.CS_ItemUse},
		{typeof(SCMapData), MsgCode.SC_MapData},
		{typeof(SCMapInfoChange), MsgCode.SC_MapInfoChange},
		{typeof(CSTerrainBuy), MsgCode.CS_TerrainBuy},
		{typeof(CSBuildAdd), MsgCode.CS_BuildAdd},
		{typeof(CSBuildRemove), MsgCode.CS_BuildRemove},
		{typeof(CSBuildUpgrade), MsgCode.CS_BuildUpgrade},
		{typeof(SCMapInfo), MsgCode.SC_MapInfo},
		{typeof(CSCreatePlayer), MsgCode.CS_CreatePlayer},
		{typeof(CSLoadPlayer), MsgCode.CS_LoadPlayer},
		{typeof(SCPlayerList), MsgCode.SC_PlayerList},
		{typeof(SCPlayerInfo), MsgCode.SC_PlayerInfo},
		{typeof(SCSceneEnter), MsgCode.SC_SceneEnter},
		{typeof(CSSceneEnter), MsgCode.CS_SceneEnter},
		{typeof(SRRegisterServer), MsgCode.SR_RegisterServer},
		{typeof(RSDispatchServer), MsgCode.RS_DispatchServer},
		{typeof(LRLoginResult), MsgCode.LR_LoginResult},
		{typeof(LRPlayerRepeat), MsgCode.LR_PlayerRepeat},
		{typeof(SSRemoteCall), MsgCode.SS_RemoteCall},
		{typeof(SSRemoteResult), MsgCode.SS_RemoteResult},
		{typeof(RSPlayerLogin), MsgCode.RS_PlayerLogin},
		{typeof(RSPlayerDisconnect), MsgCode.RS_PlayerDisconnect},
		{typeof(RSPlayerLogout), MsgCode.RS_PlayerLogout},
		{typeof(SRLoadPlayerComplete), MsgCode.SR_LoadPlayerComplete},
		{typeof(SRChangeOutScene), MsgCode.SR_ChangeOutScene},
		{typeof(RSChangeInScene), MsgCode.RS_ChangeInScene},
		{typeof(CSLogin), MsgCode.CS_Login},
		{typeof(SCLogin), MsgCode.SC_Login},
		{typeof(CSReconnect), MsgCode.CS_Reconnect},
		{typeof(SCReconnect), MsgCode.SC_Reconnect},
		{typeof(CSHeartBeat), MsgCode.CS_HeartBeat},
		{typeof(SCHeartBeat), MsgCode.SC_HeartBeat},
	};

	public static Dictionary<short, int> ProtocolSceneId = new Dictionary<short, int>() {
		{10004, 30001},
		{10009, 30001},
		{10005, 30001},
		{10007, 30001},
	};
};

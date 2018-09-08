﻿using System.Collections.Generic;

class Const
{
    /// <summary>
    /// 服务器IP地址
    /// </summary>
    public const string IPSTR = "127.0.0.1";

    /// <summary>
    /// 服务器端口
    /// </summary>
    public const int PORT = 6324;

    /// <summary>
    /// TCP协议缓冲区大小
    /// </summary>
    public const int BUFFSIZE = 8192;

    /// <summary>
    /// 协议号占用字节
    /// </summary>
    public const int MSG_CODE_BYTES = 2;

    /// <summary>
    /// 协议长度占用字节
    /// </summary>
    public const int MSG_LEN_BYTES = 4;

    /// <summary>
    /// 最大连接ID
    /// </summary>
    public const int MAX_CONNECT_ID = 100000;

    /// <summary>
    /// 无效连接ID
    /// </summary>
    public const int UNDEFINED_CONNECT_ID = -1;

    /// <summary>
    /// 本地连接ID
    /// </summary>
    public const int LOCAL_CONNECT_ID = 0;

    /// <summary>
    /// 起始用户ID
    /// </summary>
    public const int MIN_USER_ID = 100000;

    /// <summary>
    /// 起始玩家ID
    /// </summary>
    public const int MIN_PLAYER_ID = 100000;
}

/// <summary>
/// 服务器类型
/// </summary>
public class ServerType
{
    public const int ROUTE = 1;
    public const int LOGIN = 2;
    public const int CHAT = 3;
    public const int MAIN = 4;
    public const int INSTANCE = 5;
    public const int COMMON = 6;
}

/// <summary>
/// 功能场景ID
/// </summary>
public class SceneId
{
    public const int LOGIN = 20001;         //登陆
    public const int CHAT = 30001;          //聊天
    public const int MAP = 40001;           //地图
    public const int MAIL = 60001;          //邮件
    public const int TEAM = 60002;          //组队
    public const int TRADE = 60003;         //交易
    public const int CROSSFRIEND = 70001;   //跨服好友
    public const int CROSSCHAT = 70002;     //跨服聊天
}

/// <summary>
/// Layer层编号
/// </summary>
public class LayerNum
{
    public const int EDIT = 8;
    public const int TERRAIN = 9;
    public const int BUILD = 10;
}

/// <summary>
/// 地形Tag识别字符串
/// </summary>
public class GridTag
{
    public const string TERRAIN = "Terrain";
    public const string BUILD = "Build";
}

/// <summary>
/// 地形类型
/// </summary>
class TerrainType
{
    public const int GROUND = 1;
    public const int SEA = 2;
}

/// <summary>
/// 地形常量
/// </summary>
class Terrain
{
    public const int BASE = 10101;
}

/// <summary>
/// UI常量
/// </summary>
class UI
{
    /// <summary>
    /// 设计分辨率宽度
    /// </summary>
    public const float DESIGN_WIDTH = 1920f;

    /// <summary>
    /// 设计分辨率高度
    /// </summary>
    public const float DESIGN_HEIGHT = 1080f;

    /// <summary>
    /// 设计分辨率到屏幕分辨率宽度修正缩放系数
    /// </summary>
    public static float WIDTH_SCALE = 1f;

    /// <summary>
    /// 设计分辨率到屏幕分辨率高度修正缩放系数
    /// </summary>
    public static float HEIGHT_SCALE = 1f;
}

/// <summary>
/// 场景类型
/// </summary>
class SceneType
{
    /// <summary>
    /// 主场景
    /// </summary>
    public const int MAIN = 1;
    /// <summary>
    /// 副本场景
    /// </summary>
    public const int INSTANCE = 2;
}

/// <summary>
/// 效果常量
/// </summary>
class EffectType
{
    public const int INVALID = 0;
}

/// <summary>
/// Buff常量
/// </summary>
class BuffType
{
    public const int INVALID = 0;
}

/// <summary>
/// 攻击类型
/// </summary>
class FightType
{
    public const int NORMAL = 1;
    public const int MISS = 2;
    public const int CRIT = 3;
}

/// <summary>
/// 目标类型
/// </summary>
class TargetType
{
    public const int SELF = 1;
    public const int FRIEDN = 2;
    public const int ENEMY = 3;
    public const int ALL = 4;
}

/// <summary>
/// 实体类型
/// </summary>
class EntityType
{
    public const int INVALID = 0;
    /// <summary>
    /// 角色
    /// </summary>
    public const int ROLE = 1;
    /// <summary>
    /// 掉落
    /// </summary>
    public const int DROP = 2;
}

/// <summary>
/// 状态类型
/// </summary>
class StateType
{
    /// <summary>
    /// 无效状态
    /// </summary>
    public const int INVALID = 0;
    /// <summary>
    /// 待机状态
    /// </summary>
    public const int IDLE = 1;
    /// <summary>
    /// 跑动状态
    /// </summary>
    public const int RUN = 2;
    /// <summary>
    /// 攻击状态
    /// </summary>
    public const int ATTACK = 3;
    /// <summary>
    /// 被击状态
    /// </summary>
    public const int BEATTACK = 4;
    /// <summary>
    /// 眩晕状态
    /// </summary>
    public const int DAZE = 5;
    /// <summary>
    /// 死亡状态
    /// </summary>
    public const int DIE = 6;
}

/// <summary>
/// 聊天频道类型
/// </summary>
class ChannelType
{
    public const int START = 1;     //只是用于声明起始变量
    public const int SYSTEM = 2;
    public const int WORLD = 3;
    public const int REGION = 4;
    public const int END = 5;       //只是用于声明结束变量
}

/// <summary>
/// 开放玩家聊天的频道
/// </summary>
class ChannelOpen
{
    public static List<int> LIST = new List<int>()
    {
        ChannelType.WORLD, ChannelType.REGION,
    };
}

/// <summary>
/// 各频道支持的最大单元数
/// </summary>
class ChannelUnitCount
{
    public static Dictionary<int, int> MAX = new Dictionary<int, int>()
    {
        {ChannelType.SYSTEM, 1},
        {ChannelType.WORLD, 1},
        {ChannelType.REGION, 1},
    };
}

/// <summary>
/// 物品空间
/// </summary>
class ItemRoom
{
    public const int BAG = 1;
}

/// <summary>
/// 基础属性
/// </summary>
class BaseAttr
{
    public const int Money = 1;
    public const int Praise = 2;
    public const int MaxId = 2;
}

/// <summary>
/// 战斗属性
/// </summary>
class BattleAttr
{

}

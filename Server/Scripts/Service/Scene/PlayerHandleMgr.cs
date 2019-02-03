using Common.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;

delegate void PlayerHandler(IMessage protocol, Player player);

class PlayerHandleMgr
{
    public PlayerHandleMgr()
    {
        this.Init();
    }

    private void Init()
    {
        this.AddHandler(MsgCode.CS_SceneEnter, this.CSSceneEnter);

        this.AddHandler(MsgCode.CS_TerrainBuy, this.CSTerrainBuy);
        this.AddHandler(MsgCode.CS_BuildAdd, this.CSBuildAdd);
        this.AddHandler(MsgCode.CS_BuildRemove, this.CSBuildRemove);
        this.AddHandler(MsgCode.CS_BuildUpgrade, this.CSBuildUpgrade);

        this.AddHandler(MsgCode.CS_ItemUse, this.CSItemUse);

        this.AddHandler(MsgCode.CS_BattleBehavior, this.CSBattleBehavior);
    }

    private void AddHandler(int msg_id, PlayerHandler handler)
    {
        Server.GetInstance().AddHandler(msg_id, delegate(IMessage data, int connect_id, int addition, List<int> player_id_list)
        {
            Player player = Server.GetInstance().GetService<SceneService>().GetPlayerMgr().GetPlayer(addition);
            if (player != null)
            {
                handler(data, player);
            }
        });
    }

    #region 场景
    private void CSSceneEnter(IMessage data, Player player)
    {
        player.OnLogin();

        RSPlayerLogin protocol = new RSPlayerLogin();
        protocol.PlayerId = player.GetId();
        Server.GetInstance().GetSocket().SendToServer(protocol, player.GetId(), TranType.ALL_SERVER);
    }

    #endregion

    #region 地图
    private void CSTerrainBuy(IMessage data, Player player)
    {
        CSTerrainBuy protocol = data as CSTerrainBuy;
        ModMap mod = player.GetModule<ModMap>(ModuleIdx.Map);
        mod.TerrainBuy(protocol.MapName, protocol.GridId);
    }

    private void CSBuildAdd(IMessage data, Player player)
    {
        CSBuildAdd protocol = data as CSBuildAdd;
        ModMap mod = player.GetModule<ModMap>(ModuleIdx.Map);
        mod.BuildAdd(protocol.MapName, protocol.GridId, protocol.DataId, protocol.Direction);
    }

    private void CSBuildRemove(IMessage data, Player player)
    {
        CSBuildRemove protocol = data as CSBuildRemove;
        ModMap mod = player.GetModule<ModMap>(ModuleIdx.Map);
        mod.BuildRemove(protocol.MapName, protocol.GridId);
    }

    private void CSBuildUpgrade(IMessage data, Player player)
    {
        CSBuildUpgrade protocol = data as CSBuildUpgrade;
        ModMap mod = player.GetModule<ModMap>(ModuleIdx.Map);
        mod.BuildUpgrade(protocol.MapName, protocol.GridId);
    }
    #endregion

    #region 物品
    private void CSItemUse(IMessage data, Player player)
    {
        CSItemUse protocol = data as CSItemUse;
        ModItem mod = player.GetModule(ModuleIdx.Item) as ModItem;
    }
    #endregion

    #region 战斗
    private void CSBattleBehavior(IMessage data, Player player)
    {
        CSBattleBehavior protocol = data as CSBattleBehavior;
        Role role = player.GetRole(protocol.EntityId);
        if(role != null){
            Pos pos = new Pos();
            pos.SetPos(protocol.Pos.X, protocol.Pos.Y, protocol.Pos.Z);

            Face face = new Face();
            face.SetFace(protocol.Face.X, protocol.Face.Y, protocol.Face.Z);

            int skill_id = protocol.SkillId;
            int target_id = protocol.TargetId;

            role.BattleBehavior(pos, face, skill_id, target_id);
        }
    }
    #endregion
}

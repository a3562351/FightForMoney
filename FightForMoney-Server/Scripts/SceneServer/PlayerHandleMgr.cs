using Common.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;

delegate void PlayerHandler(object protocol, Player player);

class PlayerHandleMgr
{
    private SceneServer server;

    public PlayerHandleMgr(SceneServer server)
    {
        this.server = server;
    }

    public void Init()
    {
        this.AddHandler(typeof(CSTerrainBuy), this.CSTerrainBuy);
        this.AddHandler(typeof(CSBuildAdd), this.CSBuildAdd);
        this.AddHandler(typeof(CSBuildRemove), this.CSBuildRemove);
        this.AddHandler(typeof(CSBuildUpgrade), this.CSBuildUpgrade);

        this.AddHandler(typeof(CSItemUse), this.CSItemUse);

        this.AddHandler(typeof(CSBattleBehavior), this.CSBattleBehavior);
    }

    private void AddHandler(Type type, PlayerHandler handler)
    {
        this.server.GetSocket().AddHandler(type, delegate(IMessage data, int connect_id, int addition, List<int> user_id_list)
        {
            Player player = PlayerMgr.GetInstance().GetPlayerByUserId(addition);
            if (player != null)
            {
                handler(data, player);
            }
        });
    }

    #region 地图
    private void CSTerrainBuy(object data, Player player)
    {
        CSTerrainBuy protocol = data as CSTerrainBuy;
        ModMap mod = player.GetModule(ModuleIdx.Map) as ModMap;
        mod.TerrainBuy(protocol.MapName, protocol.GridId);
    }

    private void CSBuildAdd(object data, Player player)
    {
        CSBuildAdd protocol = data as CSBuildAdd;
        ModMap mod = player.GetModule(ModuleIdx.Map) as ModMap;
        mod.BuildAdd(protocol.MapName, protocol.GridId, protocol.DataId, protocol.Direction);
    }

    private void CSBuildRemove(object data, Player player)
    {
        CSBuildRemove protocol = data as CSBuildRemove;
        ModMap mod = player.GetModule(ModuleIdx.Map) as ModMap;
        mod.BuildRemove(protocol.MapName, protocol.GridId);
    }

    private void CSBuildUpgrade(object data, Player player)
    {
        CSBuildUpgrade protocol = data as CSBuildUpgrade;
        ModMap mod = player.GetModule(ModuleIdx.Map) as ModMap;
        mod.BuildUpgrade(protocol.MapName, protocol.GridId);
    }
    #endregion

    #region 物品
    private void CSItemUse(object data, Player player)
    {
        CSItemUse protocol = data as CSItemUse;
        ModItem mod = player.GetModule(ModuleIdx.Item) as ModItem;
        mod.UseItem(protocol.ItemId, protocol.Num);
    }
    #endregion

    #region 战斗
    private void CSBattleBehavior(object data, Player player)
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

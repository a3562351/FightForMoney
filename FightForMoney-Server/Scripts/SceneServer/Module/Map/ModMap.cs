using Common.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ModMap : ModuleBase
{
    private MapData map_data = new MapData();

    public ModMap(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.map_data = player_struct.MapData ?? this.map_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.MapData = this.map_data;
    }

    public override void OnLogin()
    {
        this.SendMapData();
    }

    public void SendMapData()
    {
        SCMapData protocal = new SCMapData()
        {
            MapData = this.map_data,
        };
        this.player.SendMsg(protocal);
    }

    public void TerrainBuy(string map_name, int grid_id)
    {

    }

    public void BuildAdd(string map_name, int grid_id, int data_id, float direction)
    {

    }

    public void BuildRemove(string map_name, int grid_id)
    {

    }

    public void BuildUpgrade(string map_name, int grid_id)
    {

    }
}

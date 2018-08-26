using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModTruck : ModuleBase
{
    private TruckData truck_data = new TruckData();

    public ModTruck(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.truck_data = player_struct.TruckData ?? this.truck_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.TruckData = this.truck_data;
    }
}

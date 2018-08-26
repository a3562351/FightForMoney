using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModEquip : ModuleBase
{
    private EquipData equip_data = new EquipData();

    public ModEquip(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.equip_data = player_struct.EquipData ?? this.equip_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.EquipData = this.equip_data;
    }
}

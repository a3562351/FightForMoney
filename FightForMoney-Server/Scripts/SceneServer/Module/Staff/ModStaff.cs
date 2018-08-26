using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModStaff : ModuleBase
{
    private StaffData staff_data = new StaffData();

    public ModStaff(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.staff_data = player_struct.StaffData ?? this.staff_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.StaffData = this.staff_data;
    }
}

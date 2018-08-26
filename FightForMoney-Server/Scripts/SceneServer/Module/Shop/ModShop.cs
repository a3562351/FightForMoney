using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModShop : ModuleBase
{
    private ShopData shop_data = new ShopData();

    public ModShop(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.shop_data = player_struct.ShopData ?? this.shop_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.ShopData = this.shop_data;
        this.SetDirty(false);
    }
}
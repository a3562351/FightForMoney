using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModItem : ModuleBase
{
    private ItemData item_data = new ItemData();

    public ModItem(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.item_data = player_struct.ItemData ?? this.item_data;
    }

    public override void Save(PlayerStruct player_struct)
    {
        player_struct.ItemData = this.item_data;
        this.SetDirty(false);
    }

    public override void OnLogin()
    {
        this.SendItemData();
    }

    public void SendItemData()
    {
        SCItemData protocal = new SCItemData()
        {
            ItemData = this.item_data,
        };
        this.player.SendMsg(protocal);
    }

    private int GetItem(int item_id)
    {
        if (!this.item_data.ItemList.ContainsKey(item_id))
        {
            return 0;
        }
        return this.item_data.ItemList[item_id];
    }

    private bool SetItem(int item_id, int num)
    {
        if (!this.item_data.ItemList.ContainsKey(item_id) && !this.CheckGrid())
        {
            return false;
        }

        this.item_data.ItemList[item_id] = num;
        if (num == 0)
        {
            this.item_data.ItemList.Remove(item_id);
        }
        this.SetDirty(true);
        return true;
    }

    private bool HaveItem(int item_id, int num)
    {
        if (!this.item_data.ItemList.ContainsKey(item_id))
        {
            return false;
        }

        int have_num = item_data.ItemList[item_id];
        return have_num >= num;
    }

    private bool CheckGrid()
    {
        return this.item_data.Grid > this.item_data.ItemList.Count;
    }

    private bool EffectHandle(int item_id, int num)
    {
        return true;
    }

    public bool PutIn(int item_id, int num)
    {
        if (num <= 0) return false;

        int have_num = this.GetItem(item_id);
        int new_num = have_num + num;
        if (!this.SetItem(item_id, new_num))
        {
            return false;
        }

        this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("item_id:{0} have_num:{1} new_num:{2}", item_id, have_num, new_num));

        return true;
    }

    public bool UseItem(int item_id, int num)
    {
        if (num < 0) return false;

        if (!this.HaveItem(item_id, num))
        {
            return false;
        }

        int have_num = this.GetItem(item_id);
        int new_num = have_num - num;
        if (!this.SetItem(item_id, new_num))
        {
            return false;
        }

        if (!this.EffectHandle(item_id, num))
        {
            this.SetItem(item_id, have_num);
            return false;
        }

        this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("item_id:{0} have:{1} new:{2}", item_id, have_num, new_num));

        return true;
    }

    public bool ConsumeItem(int item_id, int num)
    {
        if (num <= 0) return false;

        if (!this.HaveItem(item_id, num))
        {
            return false;
        }

        int have_num = this.GetItem(item_id);
        int new_num = have_num - num;
        this.SetItem(item_id, new_num);

        this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("item_id:{0} have:{1} new:{2}", item_id, have_num, new_num));

        return true;
    }
}

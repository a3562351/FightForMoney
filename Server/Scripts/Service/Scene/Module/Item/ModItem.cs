using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;

class ModItem : ModuleBase
{
    private ItemData item_data = new ItemData();
    private TwoKeyDictionary<int, int, long> item_id_to_uid = new TwoKeyDictionary<int, int, long>();

    public ModItem(Module module, Player player) : base(module, player) { }

    public override void Init(PlayerStruct player_struct)
    {
        this.item_data = player_struct.ItemData ?? this.item_data;

        foreach (KeyValuePair<long, ItemInfo> pair in this.item_data.ItemList)
        {
            this.item_id_to_uid.Set(pair.Value.ItemId, pair.Value.Bind, pair.Key);
        }
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

    private void SendItemChange(long uid)
    {

    }

    private bool ChangeItem(int item_id, int count, int bind, bool is_check = false)
    {
        if(count > 0)
        {
            return this.AddItem(item_id, count, bind, is_check);
        }
        else if(count < 0)
        {
            return this.ReduceItem(item_id, count, bind, is_check);
        }
        else
        {
            return true;
        }
    }

    private bool AddItem(int item_id, int count, int bind, bool is_check = false)
    {
        if(count < 0)
        {
            return false;
        }

        if(count == 0)
        {
            return true;
        }

        long uid = this.item_id_to_uid.Get(item_id, bind);
        ItemInfo item_info = this.item_data.ItemList[uid];
        if (item_info == null)
        {
            item_info = new ItemInfo();
        }

        int have_count = item_info.Count;
        int now_count = have_count + count;

        if(now_count > int.MaxValue)
        {
            return false;
        }

        if (is_check)
        {
            return true;
        }

        if(uid <= 0)
        {
            this.item_data.ItemList[uid] = item_info;
        }
        item_info.Count = now_count;

        this.SetDirty(true);
        this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("uid:{0} item_id:{1} have_count:{2} now_count:{3}", uid, item_id, have_count, now_count));

        return true;
    }

    private bool ReduceItem(int item_id, int count, int bind = 0, bool is_check = false)
    {
        if (count > 0)
        {
            return false;
        }

        if (count == 0)
        {
            return true;
        }

        //绑定和非绑都算进去，优先消耗绑定
        if (bind == 0)
        {
            int use_bind_count = 0;
            int use_free_count = 0;
            long uid = this.item_id_to_uid.Get(item_id, BindType.BIND);
            if (uid > 0)
            {
                ItemInfo item_info = this.item_data.ItemList[uid];
                if (count <= item_info.Count)
                {
                    use_bind_count = count;
                }
                else
                {
                    use_bind_count = item_info.Count;
                    count -= use_bind_count;

                    uid = this.item_id_to_uid.Get(item_id, BindType.FREE);
                    if (uid <= 0)
                    {
                        return false;
                    }

                    item_info = this.item_data.ItemList[uid];
                    if (count > item_info.Count)
                    {
                        return false;
                    }

                    use_free_count = count;
                }
            }

            if (is_check)
            {
                return true;
            }

            if(use_bind_count > 0)
            {
                uid = this.item_id_to_uid.Get(item_id, BindType.BIND);
                ItemInfo item_info = this.item_data.ItemList[uid];
                int have_count = item_info.Count;
                int now_count = have_count - use_bind_count;
                item_info.Count = now_count;

                this.SetDirty(true);
                this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    string.Format("uid:{0} item_id:{1} have_count:{2} now_count:{3}", uid, item_id, have_count, now_count));
            }

            if(use_free_count > 0)
            {
                uid = this.item_id_to_uid.Get(item_id, BindType.BIND);
                ItemInfo item_info = this.item_data.ItemList[uid];
                int have_count = item_info.Count;
                int now_count = have_count - use_free_count;
                item_info.Count = now_count;

                this.SetDirty(true);
                this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    string.Format("uid:{0} item_id:{1} have_count:{2} now_count:{3}", uid, item_id, have_count, now_count));
            }

            return true;
        }
        else
        {
            long uid = this.item_id_to_uid.Get(item_id, bind);
            if (uid <= 0)
            {
                return false;
            }

            ItemInfo item_info = this.item_data.ItemList[uid];
            int have_count = item_info.Count;
            int now_count = have_count - count;
            if (now_count < 0)
            {
                return false;
            }

            if (is_check)
            {
                return true;
            }

            item_info.Count = now_count;
            this.SetDirty(true);
            this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
                string.Format("uid:{0} item_id:{1} have_count:{2} now_count:{3}", uid, item_id, have_count, now_count));

            return true;
        }
    }

    private bool HaveItem(int item_id, int count, int bind = 0)
    {
        return this.GetItemCount(item_id, bind) >= count;
    }

    private bool CheckGrid()
    {
        return this.item_data.Grid > this.item_data.ItemList.Count;
    }

    private bool EffectHandle(int item_id, int num)
    {
        return true;
    }

    private List<List<int>> MergeItem(List<List<int>> item_list)
    {
        TwoKeyDictionary<int, int, List<int>> map = new TwoKeyDictionary<int, int, List<int>>();
        foreach (List<int> item_info in item_list)
        {
            int item_id = 0, count = 0, bind = 0, addition = 0;
            bool result = CustomFormat.ParseItemInfo(item_info, ref item_id, ref count, ref bind, ref addition);
            if (!result)
            {
                return null;
            }

            List<int> info = map.Get(item_id, bind);
            if(info == null)
            {
                info = new List<int> {item_id, 0, bind, addition};
            }
            info[2] += count;
            map.Set(item_id, bind, info);
        }
        return map.ToList();
    }

    public int GetItemCount(int item_id, int bind = 0)
    {
        int have_count = 0;

        //绑定和非绑都算进去
        if (bind == 0)
        {
            long uid = this.item_id_to_uid.Get(item_id, BindType.FREE);
            if (this.item_data.ItemList.ContainsKey(uid))
            {
                have_count += this.item_data.ItemList[uid].Count;
            }

            uid = this.item_id_to_uid.Get(item_id, BindType.BIND);
            if (this.item_data.ItemList.ContainsKey(uid))
            {
                have_count += this.item_data.ItemList[uid].Count;
            }
        }
        else
        {
            long uid = this.item_id_to_uid.Get(item_id, bind);
            if (this.item_data.ItemList.ContainsKey(uid))
            {
                have_count += this.item_data.ItemList[uid].Count;
            }
        }
        return have_count;
    }

    public bool CanAddItemList(List<List<int>> item_list)
    {
        return true;
    }

    public bool AddItemList(List<List<int>> item_list)
    {
        item_list = this.MergeItem(item_list);
        if (item_list == null)
        {
            return false;
        }

        foreach (List<int> item_info in item_list)
        {
            int item_id = 0, count = 0, bind = 0, addition = 0;
            CustomFormat.ParseItemInfo(item_info, ref item_id, ref count, ref bind, ref addition);
            if(!this.AddItem(item_id, count, bind, is_check))
            {
                return false;
            }
        }
        return true;
    }

    public bool CanConsumeItemList(List<List<int>> item_list)
    {
        return this.ConsumeItemList(item_list, true);
    }

    public bool ConsumeItemList(List<List<int>> item_list, bool is_check = false)
    {
        item_list = this.MergeItem(item_list);
        if (item_list == null)
        {
            return false;
        }

        foreach (List<int> item_info in item_list)
        {
            int item_id = 0, count = 0, bind = 0, addition = 0;
            CustomFormat.ParseItemInfo(item_info, ref item_id, ref count, ref bind, ref addition);
            if (!this.ReduceItem(item_id, count, bind, is_check))
            {
                return false;
            }
        }
        return true;
    }
}

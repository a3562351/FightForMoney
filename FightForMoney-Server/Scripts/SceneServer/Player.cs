using Common.Protobuf;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;

delegate void ModuleIterAction(ModuleIdx idx, ModuleBase module);

class Player {
    private PlayerData player_data = new PlayerData();
    private Dictionary<ModuleIdx, ModuleBase> module_map = new Dictionary<ModuleIdx, ModuleBase>();
    private bool is_online = true;
    private bool is_dirty = false;
    private Dictionary<int, Role> role_map = new Dictionary<int, Role>();

    private void ModuleIteration(ModuleIterAction action)
    {
        foreach (KeyValuePair<ModuleIdx, ModuleBase> pair in this.module_map)
        {
            action(pair.Key, pair.Value);
        }
    }

    public Player(){
        this.module_map[ModuleIdx.Map] = new ModMap(Module.Map, this);
        this.module_map[ModuleIdx.Item] = new ModItem(Module.Item, this);
        this.module_map[ModuleIdx.Shop] = new ModShop(Module.Shop, this);
        this.module_map[ModuleIdx.Staff] = new ModStaff(Module.Staff, this);
        this.module_map[ModuleIdx.Equip] = new ModEquip(Module.Equip, this);
        this.module_map[ModuleIdx.Truck] = new ModTruck(Module.Truck, this);
    }

    public void Init(PlayerStruct player_struct)
    {
        this.player_data = player_struct.PlayerData ?? this.player_data;
        this.ModuleIteration(delegate(ModuleIdx idx, ModuleBase module)
        {
            module.Init(player_struct);
            Log.Debug("Module Init : " + module.ToString());
        });
    }

    public void Save()
    {
        DataTool.SavePlayer(this.GetId(), this.GetPlayerStruct());
    }

    public void Update(int dt)
    {
        this.ModuleIteration(delegate (ModuleIdx idx, ModuleBase module)
        {
            module.Update(dt);
        });
    }

    public void OnLogin()
    {
        this.SendPlayerInfo();
        this.ModuleIteration(delegate (ModuleIdx idx, ModuleBase module)
        {
            module.OnLogin();
        });
    }

    public void OnLogout()
    {
        this.ModuleIteration(delegate (ModuleIdx idx, ModuleBase module)
        {
            module.OnLogout();
        });
    }

    private PlayerStruct GetPlayerStruct()
    {
        PlayerStruct player_struct = new PlayerStruct();
        player_struct.PlayerData = this.player_data;
        this.ModuleIteration(delegate (ModuleIdx idx, ModuleBase module)
        {
            module.Save(player_struct);
        });
        return player_struct;
    }

    public PlayerData GetPlayerData()
    {
        return this.player_data;
    }

    public bool IsOnline()
    {
        return this.is_online;
    }

    public Role GetRole(int entity_id)
    {
        if (!this.role_map.ContainsKey(entity_id))
        {
            return null;
        }
        return this.role_map[entity_id];
    }

    public void ChangeScene(int scene_idx)
    {

    }

    public void SetDirty(bool flag)
    {
        this.is_dirty = flag;
    }

    public bool IsDirty()
    {
        return this.is_dirty;
    }

    public int GetId()
    {
        return this.player_data.Id;
    }

    public string GetName()
    {
        return this.player_data.Name;
    }

    public long GetAttr(int attr_id)
    {
        if (!this.player_data.AttrMap.ContainsKey(attr_id))
        {
            return 0;
        }
        return this.player_data.AttrMap[attr_id];
    }

    public bool CheckAttr(int attr_id, long value)
    {
        return this.GetAttr(attr_id) + value >= 0;
    }

    public void ChangeAttr(int attr_id, long value)
    {
        if(attr_id < 1 || attr_id > BaseAttr.MaxId)
        {
            Log.ErrorFormat("ChangeAttr Not AttrId:{0} Value:{1}", attr_id, value);
            return;
        }

        long old_value = this.GetAttr(attr_id);
        long new_value = old_value + value;
        this.player_data.AttrMap[attr_id] = new_value;
        this.ModuleLog(Module.Player, System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("attr_id:{0} old:{1} new:{2}", attr_id, old_value, new_value));
    }

    public ModuleBase GetModule(ModuleIdx idx)
    {
        if (!this.module_map.ContainsKey(idx))
        {
            Log.Error("ModuleIdx Not Exist:" + idx.ToString());
            return null;
        }
        return this.module_map[idx];
    }

    //格式[模块名:方法名][player[玩家ID 玩家名] 详情]
    public void ModuleLog(Module module, string func_name, string desc)
    {
        string str = string.Format("[{0}:{1}][player[{2} {3}] {4}]", module.ToString(), func_name, this.GetId(), this.GetName(), desc);
        Log.Info(module.ToString(), str);
    }

    public void SendMsg(IMessage protocol)
    {
        if (this.is_online)
        {
            Server.GetInstance().GetServer<SceneServer>().GetSocket().SendMsgToRoute(protocol, this.GetId());
        }
    }

    public void Notice(int notice_code)
    {
        SCNotice protocol = new SCNotice()
        {
            NoticeCode = notice_code,
        };

        this.SendMsg(protocol);
    }

    public void SendPlayerInfo()
    {
        SCPlayerInfo protocol = new SCPlayerInfo() {
            PlayerStruct = this.GetPlayerStruct(),
        };

        this.SendMsg(protocol);
    }
}

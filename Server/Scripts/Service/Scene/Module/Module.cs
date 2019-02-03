using Common.Protobuf;
using System.Collections;
using System.Collections.Generic;

enum ModuleIdx : int
{
    Map = 1,
    Item = 2,
    Shop = 3,
    Staff = 4,
    Equip = 5,
    Truck = 6,
}

enum Module
{
    Player,
    Map,
    Item,
    Shop,
    Staff,
    Equip,
    Truck,
}

class ModuleBase
{
    protected Module module;
    protected Player player;
    protected bool dirty;

    public ModuleBase(Module module, Player player)
    {
        this.module = module;
        this.player = player;
    }

    public virtual void Init(PlayerStruct player_struct) { }
    public virtual void Save(PlayerStruct player_struct) { }
    public virtual void Update(int dt) { }
    public virtual void OnLogin() { }
    public virtual void OnLogout() { }
    public virtual void OnEnterScene() { }
    public virtual void OnLeaveScene() { }
    public virtual void OnZeroTime() { }

    protected void Log(string func_name, string desc)
    {
        this.player.ModuleLog(this.module, func_name, desc);
    }

    protected void SetDirty(bool flag)
    {
        this.dirty = flag;
    }

    protected bool IsDirty()
    {
        return this.dirty;
    }
}

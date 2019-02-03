using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ChangeAttr : EffectBase
{
    private Dictionary<int, float> change_attr_map;
    private int module_id;

    public ChangeAttr(int owner_id, int attacker_id, Dictionary<int, float> change_attr_map, int module_id) : base(owner_id, attacker_id)
    {
        this.change_attr_map = change_attr_map;
        this.module_id = module_id;
    }

    public override void Load()
    {
        Entity entity = Server.GetInstance().GetService<SceneService>().GetSceneMgr().GetEntityById(this.owner_id);
        if(entity == null || entity.GetEntityType() != EntityType.ROLE)
        {
            return;
        }

        Role target = entity as Role;
        foreach(KeyValuePair<int, float> pair in this.change_attr_map)
        {
            target.GetAttr().ChangeBaseAttr(pair.Key, pair.Value, this.module_id);
        }
    }

    public override void UnLoad()
    {
        Entity entity = Server.GetInstance().GetService<SceneService>().GetSceneMgr().GetEntityById(this.owner_id);
        if (entity == null || entity.GetEntityType() != EntityType.ROLE)
        {
            return;
        }

        Role target = entity as Role;
        foreach (KeyValuePair<int, float> pair in this.change_attr_map)
        {
            target.GetAttr().ChangeBaseAttr(pair.Key, -pair.Value, this.module_id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ChangeTmpAttr : EffectBase
{
    private Dictionary<int, float> change_attr_map;

    public ChangeTmpAttr(int owner_id, int attacker_id, Dictionary<int, float> change_attr_map) : base(owner_id, attacker_id)
    {
        this.change_attr_map = change_attr_map;
    }

    public override void Load()
    {
        Entity entity = Server.GetInstance().GetService<SceneService>().GetSceneMgr().GetEntityById(this.owner_id);
        if (entity == null || entity.GetEntityType() != EntityType.ROLE)
        {
            return;
        }

        Role target = entity as Role;
        foreach (KeyValuePair<int, float> pair in this.change_attr_map)
        {
            target.GetAttr().ChangeTmpAttr(pair.Key, pair.Value);
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
            target.GetAttr().ChangeTmpAttr(pair.Key, -pair.Value);
        }
    }
}

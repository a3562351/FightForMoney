using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AttrItem
{
    private Dictionary<int, float> attr_map = new Dictionary<int, float>();

    public void ChangeAttr(int attr_id, float value)
    {
        if (!this.attr_map.ContainsKey(attr_id))
        {
            this.attr_map.Add(attr_id, 0);
        }

        float old_value = this.attr_map[attr_id];
        float new_value = old_value + value;
        this.attr_map[attr_id] = new_value;
    }

    public float GetAttr(int attr_id)
    {
        if (!this.attr_map.ContainsKey(attr_id))
        {
            return 0;
        }
        return this.attr_map[attr_id];
    }
}

class EntityAttr
{
    private Dictionary<int, AttrItem> module_attr_map = new Dictionary<int, AttrItem>();
    private AttrItem tmp_attr_map = new AttrItem();

    public void ChangeBaseAttr(int attr_id, float value, int module_id)
    {
        if (!this.module_attr_map.ContainsKey(module_id))
        {
            this.module_attr_map.Add(module_id, new AttrItem());
        }
        this.module_attr_map[module_id].ChangeAttr(attr_id, value);
    }

    public float GetBaseAttr(int attr_id, int module_id)
    {
        if (!this.module_attr_map.ContainsKey(module_id))
        {
            return 0;
        }
        return this.module_attr_map[module_id].GetAttr(attr_id);
    }

    public void ChangeTmpAttr(int attr_id, float value)
    {
        this.tmp_attr_map.ChangeAttr(attr_id, value);
    }
}

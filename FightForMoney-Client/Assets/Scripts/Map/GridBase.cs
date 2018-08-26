using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GridType
{
    Invalid,
    Terrain,
    Build,
}

class GridBase : MonoBehaviour {
    protected GridType grid_type = GridType.Invalid;
    protected int grid_id;
    protected ConfigItem data;

    public GridType GetGridType()
    {
        return this.grid_type;
    }

    public int GetGridId()
    {
        return this.grid_id;
    }

    public int GetDataId()
    {
        return int.Parse(this.data["ID"].ToString());
    }

    public ConfigItem GetData()
    {
        return this.data;
    }

    public virtual void SetSelect(bool flag)
    {

    }
}

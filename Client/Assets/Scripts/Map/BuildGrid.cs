using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BuildGrid : GridBase{
    private float direction;

    private void Awake()
    {
        this.grid_type = GridType.Build;
    }

    public void RefreshData(int grid_id, ConfigItem data, float direction)
    {
        this.grid_id = grid_id;
        this.data = data;
        this.direction = direction;
        this.RefreshView();
    }

    protected void RefreshView()
    {
        this.transform.Find("Text").GetComponent<TextMesh>().text = this.data["Name"].ToString();
    }
}

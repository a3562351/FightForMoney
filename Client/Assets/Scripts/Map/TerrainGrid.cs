using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TerrainGrid : GridBase{
    private Material material;
    private GameObject SelectEffect;

    private void Awake()
    {
        this.grid_type = GridType.Terrain;
        this.material = this.GetComponent<MeshRenderer>().material;
        this.SelectEffect = this.transform.Find("SelectEffect").gameObject;
    }

    public void RefreshData(int grid_id, ConfigItem data)
    {
        this.grid_id = grid_id;
        this.data = data;
        this.RefreshView();
    }

    protected void RefreshView()
    {
        string avatar = "Textures/" + this.data["Avatar"];
        this.material.mainTexture = (Texture)Resources.Load(avatar);
    }

    public override void SetSelect(bool flag)
    {
        this.SelectEffect.SetActive(flag);
    }
}

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

class SelectView : ViewBase
{
    private GameObject TabView;
    private MapEditor map_editor;

    protected override void Awake()
    {
        base.Awake();
        this.map_editor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
    }

    protected override void FindChildren()
    {
        base.FindChildren();
        this.TabView = this.transform.Find("TabView").gameObject;
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    protected override void CustomInit()
    {
        Config terrain_config = ConfigPool.Load("Terrain");
        Config build_config = ConfigPool.Load("Build");

        GameObject terrain_tab = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomScrollView"));
        foreach (KeyValuePair<object, ConfigItem> pair in terrain_config)
        {
            GameObject item = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomEditItem"));
            item.tag = GridTag.TERRAIN;
            item.GetComponent<CustomEditItem>().SetData(pair.Value);
            item.GetComponent<CustomEditItem>().OnClick = this.ItemClickCallBack;
            terrain_tab.GetComponent<CustomScrollView>().AddItem(item);
        }
        this.TabView.GetComponent<CustomTabView>().AddTab("地形", terrain_tab);

        GameObject build_tab = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomScrollView"));
        foreach (KeyValuePair<object, ConfigItem> pair in build_config)
        {
            GameObject item = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomEditItem"));
            item.tag = GridTag.BUILD;
            item.GetComponent<CustomEditItem>().SetData(pair.Value);
            item.GetComponent<CustomEditItem>().OnClick = this.ItemClickCallBack;
            build_tab.GetComponent<CustomScrollView>().AddItem(item);
        }
        this.TabView.GetComponent<CustomTabView>().AddTab("建筑", build_tab);

        GameObject tool_tab = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomScrollView"));
        this.TabView.GetComponent<CustomTabView>().AddTab("工具", tool_tab);

        this.TabView.GetComponent<CustomTabView>().SelectTab(1);
    }

    private void ItemClickCallBack(BaseEventData base_event, ConfigItem data)
    {
        string prefab = "Prefabs/" + data["Prefab"];
        GameObject grid = Instantiate((GameObject)Resources.Load(prefab));
        string tag = base_event.selectedObject.tag;
        if (tag == GridTag.TERRAIN)
        {
            grid.GetComponent<TerrainGrid>().RefreshData(0, data);
        }
        else if(tag == GridTag.BUILD)
        {
            grid.GetComponent<BuildGrid>().RefreshData(0, data, 0);
        }
        this.map_editor.SetSelect(grid);
    }
}

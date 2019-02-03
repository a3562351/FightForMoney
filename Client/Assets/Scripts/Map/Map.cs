using Common.Protobuf;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class GridList
{
    private GameObject root;
    private int width;
    private int height;
    private float grid_width;
    private float grid_height;
    private Dictionary<int, GameObject> list = new Dictionary<int, GameObject>();

    public GridList(GameObject root, int width, int height, float grid_width, float grid_height)
    {
        this.root = root;
        this.width = width;
        this.height = height;
        this.grid_width = grid_width;
        this.grid_height = grid_height;
    }

    public void ChangeTerrainGrid(int grid_id, int terrain_id)
    {
        ConfigItem data = ConfigPool.Load("Terrain")[terrain_id];
        if (!this.list.ContainsKey(grid_id))
        {
            float x, z;
            this.GetXZ(grid_id, out x, out z);
            string prefab = "Prefabs/" + data["Prefab"];
            GameObject terrain_grid = GameObject.Instantiate((GameObject)Resources.Load(prefab));
            terrain_grid.name = "Terrain_" + grid_id;
            terrain_grid.transform.position = new Vector3(x, 0, z);
            terrain_grid.transform.SetParent(this.root.transform);
            this.list[grid_id] = terrain_grid;
        }
        this.list[grid_id].GetComponent<TerrainGrid>().RefreshData(grid_id, data);
    }

    public void ChangeBuildGrid(int grid_id, int build_id, float direction)
    {
        ConfigItem data = ConfigPool.Load("Build")[build_id];
        if (!this.list.ContainsKey(grid_id))
        {
            float x, z;
            this.GetXZ(grid_id, out x, out z);
            string prefab = "Prefabs/" + data["Prefab"];
            GameObject build_grid = GameObject.Instantiate((GameObject)Resources.Load(prefab));
            build_grid.name = "Build_" + grid_id;
            build_grid.transform.position = new Vector3(x, 0, z);
            build_grid.transform.SetParent(this.root.transform);
            this.list[grid_id] = build_grid;
        }
        this.list[grid_id].GetComponent<BuildGrid>().RefreshData(grid_id, data, direction);
    }

    public GameObject GetGrid(int grid_id)
    {
        return this.list[grid_id];
    }

    public Dictionary<int, GameObject> GetList()
    {
        return this.list;
    }

    private bool GetXZ(int grid_id, out float x, out float z)
    {
        x = ((grid_id - 1) % this.width) * this.grid_width;
        z = ((grid_id - 1) / this.width) * this.grid_height;
        return true;
    }
}

class Map : MonoBehaviour
{
    private const float GRID_WIDTH = 1;
    private const float GRID_HEIGHT = 1;
    private MapCommonInfo common_info = new MapCommonInfo();
    private MapField<int, MapTerrainInfo> terrain_map = new MapField<int, MapTerrainInfo>();
    private MapField<int, MapBuildInfo> build_map = new MapField<int, MapBuildInfo>();
    private GameObject terrain;
    private GameObject build;
    private GridList terrain_list;
    private GridList build_list;

    public void Create(int width, int height)
    {
        this.common_info.Time = DateTime.Now.ToString();
        this.common_info.Width = width;
        this.common_info.Height = height;
        this.common_info.BaseTerrain = Terrain.BASE;

        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x <= width; x++)
            {
                int grid_id = this.GetGridId(x, y);
                this.terrain_map[grid_id] = new MapTerrainInfo() { DataId = this.common_info.BaseTerrain };
            }
        }

        this.Draw();
    }

    public void Load(MapInfo map_info)
    {
        this.common_info = map_info.CommonInfo;
        this.terrain_map = map_info.TerrainMap;
        this.build_map = map_info.BuildMap;

        this.Draw();
    }

    public MapInfo Save(string name)
    {
        this.common_info.Name = name;
        MapInfo map_info = new MapInfo();
        map_info.CommonInfo = this.common_info;
        foreach (KeyValuePair<int, MapTerrainInfo> pair in this.terrain_map)
        {
            map_info.TerrainMap[pair.Key] = pair.Value;
        }
        foreach (KeyValuePair<int, MapBuildInfo> pair in this.build_map)
        {
            map_info.BuildMap[pair.Key] = pair.Value;
        }
        return map_info;
    }

    public float GetWidth()
    {
        return this.common_info.Width * GRID_WIDTH;
    }

    public float GetHeight()
    {
        return this.common_info.Height * GRID_HEIGHT;
    }

    private int GetGridId(int x, int z)
    {
        return (z - 1) * this.common_info.Width + x;
    }

    private void Draw()
    {
        this.terrain = new GameObject();
        this.terrain.name = "Terrain";
        this.terrain.transform.position = Vector3.zero;
        this.terrain.transform.parent = this.gameObject.transform;
        this.terrain_list = new GridList(this.terrain, this.common_info.Width, this.common_info.Height, GRID_WIDTH, GRID_HEIGHT);

        this.build = new GameObject();
        this.build.name = "Build";
        this.build.transform.position = Vector3.zero;
        this.build.transform.parent = this.gameObject.transform;
        this.build_list = new GridList(this.build, this.common_info.Width, this.common_info.Height, GRID_WIDTH, GRID_HEIGHT);

        foreach (KeyValuePair<int, MapTerrainInfo> pair in this.terrain_map) {
            this.terrain_list.ChangeTerrainGrid(pair.Key, pair.Value.DataId);
        }

        foreach (KeyValuePair<int, MapBuildInfo> pair in this.build_map)
        {
            this.build_list.ChangeBuildGrid(pair.Key, pair.Value.DataId, pair.Value.Direction);
        }
    }

    private void DrawLine()
    {

    }

    public bool IsContainBuild(int grid_id, int x_grid, int y_grid)
    {
        return false;
    }

    public void ChangeTerrainGrid(int grid_id, int data_id)
    {
        this.terrain_map[grid_id] = new MapTerrainInfo() { DataId = data_id };
        this.terrain_list.ChangeTerrainGrid(grid_id, data_id);
    }

    public void ChangeBuildGrid(int grid_id, int data_id, float direction)
    {
        this.build_map[grid_id] = new MapBuildInfo() { DataId = data_id, Direction = direction };
        this.build_list.ChangeBuildGrid(grid_id, data_id, direction);
    }

    public MapTerrainInfo GetTerrainInfo(int grid_id)
    {
        if (this.terrain_map.ContainsKey(grid_id))
        {
            return this.terrain_map[grid_id];
        }
        else
        {
            return null;
        }
    }

    public MapBuildInfo GetBuildInfo(int grid_id)
    {
        if (this.build_map.ContainsKey(grid_id))
        {
            return this.build_map[grid_id];
        }
        else
        {
            return null;
        }
    }
}

using Common.Protobuf;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class ChangeGridCommand
{
    public int GridId;
    public int DataId;
    public float Direction;
}

class MapEditor : MonoBehaviour {
    protected bool map_active = false;
    protected bool is_running = true;
    protected bool meun_visible = false;
    protected Map map;
    protected GameObject Canvas;
    protected GameObject select;
    protected GameObject last_hit_grid;

    private void Awake()
    {
        GameObject map_root = new GameObject();
        map_root.name = "Map";
        this.map = map_root.AddComponent<Map>();
        this.Canvas = GameObject.Find("Canvas");
    }

    private void Update()
    {
        this.CheckKeyEvent();
        if (this.IsActive())
        {
            this.UpdateGridState();
        }
    }

    public void CreateMap(int width, int height)
    {
        this.map.Create(width, height);
        this.map_active = true;
    }

    public void LoadMap(string path)
    {
        FileStream file = File.Open(path, FileMode.Open);
        this.map.Load(MapInfo.Parser.ParseFrom(file));
        file.Close();
        this.map_active = true;
    }

    public void SaveMap(string name)
    {
        MapInfo map_info = map.Save(name);
        string path = Application.streamingAssetsPath + "/Map/" + name + ".Data";
        FileStream file = File.Create(path);
        map_info.WriteTo(file);
        file.Close();

        Debug.Log("SaveMap:" + name);
    }

    public string[] FindAllMapPath()
    {
        string path = Application.streamingAssetsPath + "/Map/";
        string[] files = Directory.GetFiles(path, "*.Data");
        return files;
    }

    public string FindMap(string map_name)
    {
        string file_path = string.Format("{0}/Map/{1}.Data", Application.streamingAssetsPath, map_name);
        return File.Exists(file_path) ? file_path : null;
    }

    public Map GetMap()
    {
        return this.map;
    }

    public void SetSelect(GameObject select)
    {
        this.UnSelect();
        select.layer = LayerNum.EDIT;
        this.select = select;
    }

    public void UnSelect()
    {
        if (this.select)
        {
            DestroyImmediate(this.select);
            this.select = null;
        }
    }

    public bool IsSelect()
    {
        return this.select != null;
    }

    public bool IsActive()
    {
        return this.map_active && this.is_running && !this.meun_visible;
    }

    protected virtual void CheckKeyEvent()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && this.map_active)
        {
            if (this.Canvas.transform.Find("MeunPop") == null)
            {
                GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/MeunPop");
                GameObject MeunPop = Instantiate(perfab);
                MeunPop.name = "MeunPop";
                MeunPop.transform.parent = this.Canvas.transform;
                MeunPop.GetComponent<MeunPop>().EditorInit();
                this.meun_visible = MeunPop.activeInHierarchy;
            }
            else
            {
                GameObject MeunView = this.Canvas.transform.Find("MeunPop").gameObject;
                MeunView.SetActive(!MeunView.activeInHierarchy);
                this.meun_visible = MeunView.activeInHierarchy;
            }
        }
    }

    protected virtual void UpdateGridState()
    {
        if (this.IsSelect())
        {
            this.CheckHitTerrainGrid();
            if (this.last_hit_grid != null)
            {
                this.select.SetActive(true);
                this.select.transform.position = this.last_hit_grid.transform.position;
                this.select.transform.rotation = this.last_hit_grid.transform.rotation;
                this.select.transform.Translate(new Vector3(0f, 0.001f, 0f), Space.Self);

                if (Input.GetMouseButton(0))
                {
                    ChangeGridCommand command = new ChangeGridCommand();
                    command.GridId = this.last_hit_grid.GetComponent<TerrainGrid>().GetGridId();
                    command.Direction = this.select.transform.rotation.eulerAngles.y;
                    if (this.select.GetComponent<TerrainGrid>())
                    {
                        command.DataId = this.select.GetComponent<TerrainGrid>().GetDataId();
                        this.ChangeTerrainGrid(command);
                    }
                    else if (this.select.GetComponent<BuildGrid>())
                    {
                        command.DataId = this.select.GetComponent<BuildGrid>().GetDataId();
                        this.ChangeBuildGrid(command);
                    }
                }
            }
            else
            {
                this.select.SetActive(false);
            }

            if (Input.GetMouseButton(1))
            {
                this.UnSelect();
            }
        }
    }

    protected void CheckHitTerrainGrid()
    {
        GameObject hit_grid = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerNum.TERRAIN))
        {
            hit_grid = hit.collider.gameObject;
        }

        if (this.last_hit_grid != hit_grid)
        {
            this.last_hit_grid = hit_grid;
        }
    }

    protected bool CheckChangeTerrainGrid(int grid_id, int terrain_id)
    {
        return true;
    }

    protected bool CheckChangeBuildGrid(int grid_id, int build_id)
    {
        return true;
    }

    protected virtual void ChangeTerrainGrid(ChangeGridCommand command)
    {
        this.OnTerrainGridChange(command);
    }

    protected virtual void ChangeBuildGrid(ChangeGridCommand command)
    {
        this.OnBuildGridChange(command);
    }

    // 通过保存命令可以做成引导回放等东西，先挖坑
    protected virtual void OnTerrainGridChange(ChangeGridCommand command)
    {
        this.map.ChangeTerrainGrid(command.GridId, command.DataId);
    }

    protected virtual void OnBuildGridChange(ChangeGridCommand command)
    {
        this.map.ChangeBuildGrid(command.GridId, command.DataId, command.Direction);
    }
}

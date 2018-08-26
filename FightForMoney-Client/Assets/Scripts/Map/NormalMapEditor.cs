using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class NormalMapEditor : MapEditor
{
    protected override void CheckKeyEvent()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && this.map_active)
        {
            if (this.Canvas.transform.Find("MeunPop") == null)
            {
                GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/MeunPop");
                GameObject MeunPop = Instantiate(perfab);
                MeunPop.name = "MeunPop";
                MeunPop.transform.parent = this.Canvas.transform;
                MeunPop.GetComponent<MeunPop>().NormalInit();
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

    protected override void UpdateGridState()
    {
        this.CheckHitTerrainGrid();
        if (this.last_hit_grid != null)
        {
            int grid_id = this.last_hit_grid.GetComponent<TerrainGrid>().GetGridId();
            this.map.GetTerrainInfo(grid_id);
            this.map.GetBuildInfo(grid_id);
        }
    }

    protected override void ChangeTerrainGrid(ChangeGridCommand command)
    {

    }

    protected override void ChangeBuildGrid(ChangeGridCommand command)
    {

    }
}

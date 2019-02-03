using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class MeunPop : PopBase
{
    private MapEditor map_editor;

    protected override void Awake()
    {
        base.Awake();
        this.map_editor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
    }

    protected override void FindChildren()
    {
        base.FindChildren();
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    public void EditorInit()
    {
        GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/CustomButton");

        GameObject save_btn = Instantiate(perfab);
        save_btn.transform.Find("Text").GetComponent<Text>().text = "保存";
        save_btn.transform.parent = gameObject.transform;
        save_btn.GetComponent<CustomButton>().OnClick = delegate
        {
            GameObject CustomInputPop = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomInputPop"));
            CustomInputPop.GetComponent<CustomInputPop>().SetTitle("是否地图保存");
            CustomInputPop.GetComponent<CustomInputPop>().SetInputLabel("地图名:");
            CustomInputPop.GetComponent<CustomInputPop>().SetSubmitStr("保存");
            CustomInputPop.GetComponent<CustomInputPop>().SetCancelStr("取消");
            CustomInputPop.GetComponent<CustomInputPop>().SetSubmitCallBack(delegate (object data) {
                this.map_editor.SaveMap(data.ToString());
                this.Canvas.GetComponent<CanvasBase>().RemoveWindow(CustomInputPop);
            });
            CustomInputPop.GetComponent<CustomInputPop>().SetCancelCallBack(delegate (object data) {
                this.Canvas.GetComponent<CanvasBase>().RemoveWindow(CustomInputPop);
            });
            this.Canvas.GetComponent<CanvasBase>().AddWindow(CustomInputPop);
        };

        GameObject back_btn = Instantiate(perfab);
        back_btn.transform.Find("Text").GetComponent<Text>().text = "退出编辑";
        back_btn.transform.parent = gameObject.transform;
        back_btn.GetComponent<CustomButton>().OnClick = delegate
        {
            SceneManager.LoadScene("MapEdit");
        };
    }

    public void NormalInit()
    {

    }
}

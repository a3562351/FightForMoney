using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

class EditView : ViewBase
{
    private MapEditor map_editor;
    private GameObject SelectPop;
    private GameObject CreatePop;
    private GameObject LoadPop;
    private GameObject CreateBtn;
    private GameObject LoadBtn;
    private GameObject WidthInput;
    private GameObject HeightInput;
    private GameObject SubmitBtn;
    private GameObject MapList;

    protected override void Awake()
    {
        base.Awake();
        this.map_editor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
    }

    protected override void FindChildren()
    {
        base.FindChildren();
        this.SelectPop = this.transform.Find("SelectPop").gameObject;
        this.CreatePop = this.transform.Find("CreatePop").gameObject;
        this.LoadPop = this.transform.Find("LoadPop").gameObject;

        this.CreateBtn = this.SelectPop.transform.Find("CreateBtn").gameObject;
        this.LoadBtn = this.SelectPop.transform.Find("LoadBtn").gameObject;

        this.WidthInput = this.CreatePop.transform.Find("WidthInput").gameObject;
        this.HeightInput = this.CreatePop.transform.Find("HeightInput").gameObject;
        this.SubmitBtn = this.CreatePop.transform.Find("SubmitBtn").gameObject;

        this.MapList = this.LoadPop.transform.Find("MapList").gameObject;
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        this.CreateBtn.GetComponent<CustomButton>().OnClick = delegate {
            this.SelectPop.SetActive(false);
            this.CreatePop.SetActive(true);
            this.LoadPop.SetActive(false);

            this.WidthInput.GetComponent<CustomInputField>().SetContentType(InputField.ContentType.IntegerNumber);
            this.HeightInput.GetComponent<CustomInputField>().SetContentType(InputField.ContentType.IntegerNumber);
            this.WidthInput.GetComponent<CustomInputField>().SetLabel("宽:");
            this.HeightInput.GetComponent<CustomInputField>().SetLabel("高:");
            this.SubmitBtn.GetComponent<CustomButton>().SetLabel("创建");
        };

        this.LoadBtn.GetComponent<CustomButton>().OnClick = delegate {
            this.SelectPop.SetActive(false);
            this.CreatePop.SetActive(false);
            this.LoadPop.SetActive(true);

            this.MapList.GetComponent<CustomScrollView>().RemoveAllChildren();

            string[] path_list = this.map_editor.FindAllMapPath();
            GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/CustomButton");
            foreach(string path in path_list)
            {
                GameObject CustomButton = Instantiate(perfab);
                CustomButton.GetComponent<CustomButton>().SetLabel(Path.GetFileNameWithoutExtension(path));
                CustomButton.GetComponent<CustomButton>().OnClick = delegate
                {
                    this.map_editor.LoadMap(path);
                    this.Canvas.transform.Find("SelectView").gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                };
                this.MapList.GetComponent<CustomScrollView>().AddItem(CustomButton);
            }
        };

        this.SubmitBtn.GetComponent<CustomButton>().OnClick = delegate {
            int width = int.Parse(this.WidthInput.GetComponent<CustomInputField>().GetInputStr());
            int height = int.Parse(this.HeightInput.GetComponent<CustomInputField>().GetInputStr());
            GameObject.Find("MapEditor").GetComponent<MapEditor>().CreateMap(width, height);
            this.Canvas.transform.Find("SelectView").gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        };
    }

    protected override void CustomInit()
    {
        base.CustomInit();
        this.CreateBtn.GetComponent<CustomButton>().SetLabel("创建");
        this.LoadBtn.GetComponent<CustomButton>().SetLabel("载入");
    }
}

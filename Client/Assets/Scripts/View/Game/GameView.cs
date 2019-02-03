using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

class GameView : ViewBase
{
    private NormalMapEditor map_editor;
    private GameObject SelectPlayerPop;

    protected override void Awake()
    {
        base.Awake();
        this.map_editor = GameObject.Find("MapEditor").GetComponent<NormalMapEditor>();
    }

    protected override void Start()
    {
        base.Start();
        PlayerCtrl.GetInstance().CSLogin(true);
    }

    protected override void FindChildren()
    {
        base.FindChildren();
        this.SelectPlayerPop = this.Canvas.transform.Find("SelectPlayerPop").gameObject;
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        EventDispatcher.GetInstance().RegisterListen(EventType.Register, this.Register);
        EventDispatcher.GetInstance().RegisterListen(EventType.CreateNewPlayer, this.CreateNewPlayer);
        EventDispatcher.GetInstance().RegisterListen(EventType.SelectPlayer, this.SelectPlayer);
    }

    protected override void CustomInit()
    {
        base.CustomInit();
        this.SelectPlayerPop.GetComponent<CustomScrollPop>().SetTitle("选择角色");
        this.SelectPlayerPop.GetComponent<CustomScrollPop>().SetSubmitStr("新建角色");

        this.SelectPlayerPop.GetComponent<CustomScrollPop>().SetSubmitCallBack(delegate (object data) {
            this.CreateNewPlayer();
        });
    }

    private void Register(EventData event_data = null)
    {
        PlayerCtrl.GetInstance().CSLogin(false);
    }

    private void CreateNewPlayer(EventData event_data = null)
    {
        GameObject CustomInputPop = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomInputPop"));
        CustomInputPop.GetComponent<CustomInputPop>().SetTitle("创建角色");
        CustomInputPop.GetComponent<CustomInputPop>().SetInputLabel("角色名:");
        CustomInputPop.GetComponent<CustomInputPop>().SetSubmitStr("创建");
        CustomInputPop.GetComponent<CustomInputPop>().SetCancelStr("取消");
        CustomInputPop.GetComponent<CustomInputPop>().SetSubmitCallBack(delegate (object player_name) {
            this.SelectMap(delegate(object map_name) {
                PlayerCtrl.GetInstance().CSCreatePlayer(player_name.ToString(), map_name.ToString());
                this.Canvas.GetComponent<CanvasBase>().RemoveWindow(CustomInputPop);
            });
        });
        CustomInputPop.GetComponent<CustomInputPop>().SetCancelCallBack(delegate (object data) {
            this.Canvas.GetComponent<CanvasBase>().RemoveWindow(CustomInputPop);
        });
        this.Canvas.GetComponent<CanvasBase>().AddWindow(CustomInputPop);
    }

    private void SelectMap(WidgetCallBack callback)
    {
        GameObject CustomScrollPop = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomScrollPop"));
        CustomScrollPop.GetComponent<CustomScrollPop>().SetTitle("选择地图");
        CustomScrollPop.GetComponent<CustomScrollPop>().HideSubmitBtn();

        GameObject ScrollView  = CustomScrollPop.GetComponent<CustomScrollPop>().GetScrollView();

        string[] path_list = this.map_editor.FindAllMapPath();
        GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/CustomButton");
        foreach (string path in path_list)
        {
            GameObject CustomButton = Instantiate(perfab);
            CustomButton.GetComponent<CustomButton>().SetLabel(Path.GetFileNameWithoutExtension(path));
            CustomButton.GetComponent<CustomButton>().OnClick = delegate
            {
                callback(Path.GetFileNameWithoutExtension(path));
                this.Canvas.GetComponent<CanvasBase>().RemoveWindow(CustomScrollPop);
            };
            ScrollView.GetComponent<CustomScrollView>().AddItem(CustomButton);
        }
        this.Canvas.GetComponent<CanvasBase>().AddWindow(CustomScrollPop);
    }

    private void SelectPlayer(EventData event_data)
    {
        SCPlayerList protocol = event_data.data as SCPlayerList;

        GameObject ScrollView = this.SelectPlayerPop.GetComponent<CustomScrollPop>().GetScrollView();
        ScrollView.GetComponent<CustomScrollView>().RemoveAllChildren();

        GameObject perfab = (GameObject)Resources.Load("Prefabs/UI/CustomButton");
        foreach (PlayerInfo player_info in protocol.PlayerList)
        {
            GameObject CustomButton = Instantiate(perfab);
            string label = String.Format("角色ID:{0} 角色名:{1} 所属地图名:{2}", player_info.PlayerId, player_info.PlayerName, player_info.MapName);
            CustomButton.GetComponent<CustomButton>().SetLabel(label);
            CustomButton.GetComponent<CustomButton>().OnClick = delegate
            {
                string path = this.map_editor.FindMap(player_info.MapName);
                if(path != null)
                {
                    PlayerCtrl.GetInstance().CSLoadPlayer(player_info.PlayerId);
                }
                else
                {
                    Debug.Log("地图不存在!");
                }
            };
            ScrollView.GetComponent<CustomScrollView>().AddItem(CustomButton);
        }
    }
}

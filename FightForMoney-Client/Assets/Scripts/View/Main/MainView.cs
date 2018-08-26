using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class MainView : ViewBase
{
    private GameObject MeunItemList;
    private GameObject StartBtn;
    private GameObject EditBtn;
    private GameObject OnlineBtn;

    protected override void FindChildren()
    {
        base.FindChildren();
        this.MeunItemList = this.transform.Find("MeunItemList").gameObject;
        this.StartBtn = this.MeunItemList.transform.Find("StartBtn").gameObject;
        this.EditBtn = this.MeunItemList.transform.Find("EditBtn").gameObject;
        this.OnlineBtn = this.MeunItemList.transform.Find("OnlineBtn").gameObject;
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        this.StartBtn.GetComponent<CustomButton>().OnClick = delegate
        {
            GameManager.StartGame();
            SceneManager.LoadScene("Game");
        };

        this.EditBtn.GetComponent<CustomButton>().OnClick = delegate
        {
            SceneManager.LoadScene("MapEdit");
        };

        this.OnlineBtn.GetComponent<CustomButton>().OnClick = delegate
        {
            GameManager.StartOnlineGame();
            SceneManager.LoadScene("Online");
        };
    }

    protected override void CustomInit()
    {
        base.CustomInit();
        this.StartBtn.GetComponent<CustomButton>().SetLabel("开始游戏");
        this.EditBtn.GetComponent<CustomButton>().SetLabel("地图制作");
        this.OnlineBtn.GetComponent<CustomButton>().SetLabel("局域网联机");
    }
}

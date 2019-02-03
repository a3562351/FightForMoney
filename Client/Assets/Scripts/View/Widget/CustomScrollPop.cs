using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CustomScrollPop : PopBase
{
    private GameObject CustomLabel;
    private GameObject CustomScrollView;
    private GameObject SubmitBtn;
    private WidgetCallBack SubmitCallBack;

    protected override void FindChildren()
    {
        base.FindChildren();
        this.CustomLabel = transform.Find("CustomLabel").gameObject;
        this.CustomScrollView = transform.Find("CustomScrollView").gameObject;
        this.SubmitBtn = transform.Find("SubmitBtn").gameObject;
    }

    protected override void RegisterEvent()
    {
        this.SubmitBtn.GetComponent<CustomButton>().OnClick = delegate
        {
            this.SubmitCallBack?.Invoke();
        };
    }

    protected override void CustomInit()
    {
        base.CustomInit();
    }

    public void SetTitle(string str)
    {
        this.CustomLabel.GetComponent<CustomLabel>().SetLabel(str);
    }

    public void SetSubmitStr(string str)
    {
        this.SubmitBtn.GetComponent<CustomButton>().SetLabel(str);
    }

    public void SetSubmitCallBack(WidgetCallBack callback)
    {
        this.SubmitCallBack = callback;
    }

    public GameObject GetScrollView()
    {
        return this.CustomScrollView;
    }

    public void HideSubmitBtn()
    {
        this.SubmitBtn.SetActive(false);
    }
}

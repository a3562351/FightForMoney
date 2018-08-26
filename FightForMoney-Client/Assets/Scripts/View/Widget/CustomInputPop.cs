using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CustomInputPop : PopBase
{
    private GameObject CustomLabel;
    private GameObject NameInputField;
    private GameObject SubmitBtn;
    private GameObject CancelBtn;
    private WidgetCallBack SubmitCallBack;
    private WidgetCallBack CancelCallBack;

    protected override void FindChildren()
    {
        base.FindChildren();
        this.CustomLabel = transform.Find("CustomLabel").gameObject;
        this.NameInputField = transform.Find("NameInputField").gameObject;
        this.SubmitBtn = transform.Find("SubmitBtn").gameObject;
        this.CancelBtn = transform.Find("CancelBtn").gameObject;
    }

    protected override void RegisterEvent()
    {
        this.SubmitBtn.GetComponent<CustomButton>().OnClick = delegate
        {
            if (!this.NameInputField.GetComponent<CustomInputField>().IsContentEmpty())
            {
                this.SubmitCallBack?.Invoke(this.NameInputField.GetComponent<CustomInputField>().GetInputStr());
            }
        };

        this.CancelBtn.GetComponent<CustomButton>().OnClick = delegate {
            this.CancelCallBack?.Invoke(this.NameInputField.GetComponent<CustomInputField>().GetInputStr());
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

    public void SetInputLabel(string str)
    {
        this.NameInputField.GetComponent<CustomInputField>().SetLabel(str);
    }

    public void SetSubmitStr(string str)
    {
        this.SubmitBtn.GetComponent<CustomButton>().SetLabel(str);
    }

    public void SetCancelStr(string str)
    {
        this.CancelBtn.GetComponent<CustomButton>().SetLabel(str);
    }

    public void SetSubmitCallBack(WidgetCallBack callback)
    {
        this.SubmitCallBack = callback;
    }

    public void SetCancelCallBack(WidgetCallBack callback)
    {
        this.CancelCallBack = callback;
    }

    public void HideCancelBtn()
    {
        this.CancelBtn.SetActive(false);
        Vector3 pos = this.SubmitBtn.transform.localPosition;
        pos.x = 0;
        this.SubmitBtn.transform.localPosition = pos;
    }
}

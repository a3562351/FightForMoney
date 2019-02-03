using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CustomInputField : CustomWidget
{
    private GameObject Label;
    private GameObject InputField;

    protected override void FindChildren()
    {
        this.Label = this.transform.Find("Label").gameObject;
        this.InputField = this.transform.Find("InputField").gameObject;
    }

    public void SetLabel(string str)
    {
        this.Label.GetComponent<Text>().text = str;
    }

    public void SetContentType(InputField.ContentType contentType)
    {
        this.InputField.GetComponent<InputField>().contentType = contentType;
    }

    public string GetInputStr()
    {
        return this.InputField.GetComponent<InputField>().text;
    }

    public bool IsContentEmpty()
    {
        string content = this.InputField.GetComponent<InputField>().text;
        return content.Equals("");
    }
}

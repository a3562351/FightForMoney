using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CustomLabel : CustomWidget
{
    private GameObject Image;
    private GameObject Text;

    protected override void FindChildren()
    {
        this.Image = this.transform.Find("Image").gameObject;
        this.Text = this.transform.Find("Text").gameObject;
    }

    public string GetLabel()
    {
        return this.Text.GetComponent<Text>().text;
    }

    public void SetLabel(string str)
    {
        this.Text.GetComponent<Text>().text = str;
    }
}

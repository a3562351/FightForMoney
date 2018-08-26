using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CustomEditItem : CustomWidget
{
    private GameObject Image;
    private GameObject Text;

    protected override void FindChildren()
    {
        this.Image = this.transform.Find("Image").gameObject;
        this.Text = this.transform.Find("Text").gameObject;
    }

    public void SetData(ConfigItem data)
    {
        this.data = data;
        this.UpdateView();
    }

    private void UpdateView()
    {
        string icon = "Textures/" + (this.data["Icon"] != null ? this.data["Icon"] : "Default");
        this.Image.GetComponent<RawImage>().texture = (Texture)Resources.Load(icon);
        this.Text.GetComponent<Text>().text = this.data["Name"].ToString();
    }
}

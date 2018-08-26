using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CustomScrollView : CustomWidget
{
    private GameObject Content;

    protected override void FindChildren()
    {
        this.Content = this.transform.Find("Viewport").Find("Content").gameObject;
    }

    protected override void RegisterEvent()
    {

    }

    public void AddItem(GameObject item)
    {
        //不知什么原因缩放比例在设置了父节点会变化
        Vector3 scale = item.GetComponent<RectTransform>().localScale;
        item.transform.SetParent(this.Content.transform);
        item.GetComponent<RectTransform>().localScale = scale;
    }

    public void RemoveAllChildren()
    {
        foreach(Transform child in this.Content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class CustomButton : CustomWidget
{
    private const float DESIGN_WIDTH = 170f;
    private const float DESIGN_HEIGHT = 70f;
    private GameObject Text;

    protected override void FindChildren()
    {
        this.Text = this.transform.Find("Text").gameObject;
    }

    protected override void CustomInit()
    {
        //RectTransform rectTransform = this.GetComponent<RectTransform>();
        //rectTransform.pivot = new Vector2(0.5f, 0.5f);
        //rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        //rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        //rectTransform.sizeDelta = new Vector2(DESIGN_WIDTH, DESIGN_HEIGHT);

        //LayoutElement layoutElement = this.GetComponent<LayoutElement>();
        //layoutElement.preferredWidth = DESIGN_WIDTH;
        //layoutElement.preferredHeight = DESIGN_HEIGHT;
    }

    public void SetLabel(string str)
    {
        this.Text.GetComponent<Text>().text = str;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CustomTabView : CustomWidget
{
    private GameObject TitleContent;
    private GameObject TabContent;
    private List<GameObject> title_list = new List<GameObject>();
    private List<GameObject> tab_list = new List<GameObject>();

    protected override void FindChildren()
    {
        this.TitleContent = this.transform.Find("TitleContent").gameObject;
        this.TabContent = this.transform.Find("TabContent").gameObject;
    }

    public void AddTab(string str, GameObject tab)
    {
        GameObject title = Instantiate((GameObject)Resources.Load("Prefabs/UI/CustomButton"));
        title.transform.Find("Text").GetComponent<Text>().text = str;
        title.GetComponent<CustomButton>().OnClick = delegate {
            this.SelectTab(title);
        };
        title.transform.SetParent(this.TitleContent.transform);
        tab.transform.SetParent(this.TabContent.transform);
        tab.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        tab.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        tab.SetActive(false);

        title_list.Add(title);
        tab_list.Add(tab);
    }

    public void SelectTab(int index)
    {
        GameObject title = title_list[index - 1];
        if(title != null)
        {
            this.SelectTab(title);
        }
    }

    private void SelectTab(GameObject title)
    {
        this.HideAllTab();
        GameObject tab = tab_list[title_list.IndexOf(title)];
        if (tab != null)
        {
            tab.SetActive(true);
        }
    }

    private void HideAllTab()
    {
        foreach (GameObject tab in tab_list)
        {
            tab.SetActive(false);
        }
    }
}

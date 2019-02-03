using UnityEngine;
using UnityEngine.UI;

class PopBase : ViewBase
{
    protected override void ResolutionAdapter()
    {
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.localScale = new Vector3(UI.WIDTH_SCALE, UI.HEIGHT_SCALE);
    }
}

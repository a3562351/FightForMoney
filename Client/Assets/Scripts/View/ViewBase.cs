using UnityEngine;
using UnityEngine.UI;

class ViewBase : MonoBehaviour{
    protected GameObject Canvas;

    protected virtual void Awake()
    {
        this.FindChildren();
        this.RegisterEvent();
    }

    protected virtual void Start()
    {
        this.CustomInit();
        this.ResolutionAdapter();
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    protected virtual void FindChildren() {
        this.Canvas = GameObject.Find("Canvas");
    }

    /// <summary>
    /// 注册监听
    /// </summary>
    protected virtual void RegisterEvent() { }

    /// <summary>
    /// 分辨率适配
    /// </summary>
    protected virtual void ResolutionAdapter(){
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(UI.DESIGN_WIDTH, UI.DESIGN_HEIGHT);
        rectTransform.localScale = new Vector3(UI.WIDTH_SCALE, UI.HEIGHT_SCALE);
    }

    /// <summary>
    /// 自定义界面
    /// </summary>
    protected virtual void CustomInit() { }

    /// <summary>
    /// 添加到画布调用
    /// </summary>
    protected virtual void AddToCanvas() { }

    /// <summary>
    /// 从画布移除调用
    /// </summary>
    protected virtual void RemoveFromCanvas() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CanvasBase : MonoBehaviour {
    private List<GameObject> window_list = new List<GameObject>();

    private void Awake()
    {
        this.ResolutionAdapter();
    }

    private void ResolutionAdapter()
    {
        UI.WIDTH_SCALE = Screen.width / UI.DESIGN_WIDTH;
        UI.HEIGHT_SCALE = Screen.height / UI.DESIGN_HEIGHT;
        CanvasScaler canvasScaler = this.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
    }

    public void AddWindow(GameObject window)
    {
        window.transform.parent = this.gameObject.transform;
        this.window_list.Add(window);
    }

    public void RemoveWindow(GameObject window)
    {
        this.window_list.Remove(window);
        Destroy(window);
    }

    public void RemoveLastWindow()
    {
        int window_count = window_list.Count;
        if (window_count > 0)
        {
            GameObject window = window_list[window_count - 1];
            window_list.RemoveAt(window_count - 1);
            Destroy(window);
        }
    }
}

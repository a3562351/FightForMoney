using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public delegate void WidgetCallBack(object data = null);
public delegate void WidgetEvent(BaseEventData base_event, ConfigItem data);

class CustomWidget : MonoBehaviour {
    protected ConfigItem data;

    /// <summary>
    /// 点击触发事件
    /// </summary>
    public WidgetEvent OnClick
    {
        set
        {
            this.AddListener(EventTriggerType.PointerClick, value);
        }
    }

    private EventTrigger GetEventTrigger()
    {
        EventTrigger trigger = this.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = this.gameObject.AddComponent<EventTrigger>();
        }
        return trigger;
    }

    private void AddListener(EventTriggerType trigger_type, WidgetEvent widget_event)
    {
        EventTrigger trigger = this.GetEventTrigger();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = trigger_type;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(delegate(BaseEventData base_event) {
            widget_event(base_event, this.data);
        });
        trigger.triggers.Add(entry);
    }

    protected virtual void Awake()
    {
        this.FindChildren();
        this.RegisterEvent();
    }

    protected virtual void Start()
    {
        this.CustomInit();
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    protected virtual void FindChildren() { }

    /// <summary>
    /// 注册监听
    /// </summary>
    protected virtual void RegisterEvent() { }

    /// <summary>
    /// 自定义界面，可用于多语言实现
    /// </summary>
    protected virtual void CustomInit() { }
}

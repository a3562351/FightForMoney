using System.Collections;
using System.Collections.Generic;
using UnityEngine;

delegate void EventCallBack(EventData data);

enum EventType
{
    Register,
    CreateNewPlayer,
    SelectPlayer,
}

class EventData
{
    public object data;
}

class EventDispatcher
{
    private static EventDispatcher Instance = null;
    private Dictionary<EventType, List<EventCallBack>> event_map = new Dictionary<EventType, List<EventCallBack>>();

    public static EventDispatcher GetInstance()
    {
        if (Instance == null)
        {
            Instance = new EventDispatcher();
        }
        return Instance;
    }

    public void RegisterListen(EventType event_type, EventCallBack cb, int priority = 100)
    {
        if (!this.event_map.ContainsKey(event_type))
        {
            this.event_map[event_type] = new List<EventCallBack>();
        }

        priority = priority < 0 ? 0 : priority;
        priority = priority > this.event_map[event_type].Count ? this.event_map[event_type].Count : priority;
        this.event_map[event_type].Insert(priority, cb);
    }

    public void DispatchEvent(EventType event_type, EventData data = null)
    {
        if (this.event_map.ContainsKey(event_type))
        {
            foreach(EventCallBack cb in this.event_map[event_type])
            {
                cb(data);
            }
        }
    }
}

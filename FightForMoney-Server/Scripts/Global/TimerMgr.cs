using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

delegate void TimerAction(object data);

class TimerItem
{
    private int interval;
    private TimerAction action;
    private object data;
    private bool is_loop;
    private int time;

    public TimerItem(int interval, TimerAction action, object data, bool is_loop)
    {
        this.interval = interval;
        this.action = action;
        this.data = data;
        this.is_loop = is_loop;
        this.time = 0;
    }

    public bool Update(int dt)
    {
        this.time += dt;
        if(this.time >= this.interval)
        {
            this.time -= this.interval;
            return true;
        }
        else
        {
            return false;
        }
    }

    public TimerAction GetAction()
    {
        return this.action;
    }

    public object GetData()
    {
        return this.data;
    }

    public bool IsLoop()
    {
        return this.is_loop;
    }
}

class TimerMgr
{
    private static TimerMgr Instance = null;
    private Dictionary<int, TimerItem> timer_map = new Dictionary<int, TimerItem>();
    private int timer_id = 0;

    public static TimerMgr GetInstance()
    {
        if (Instance == null)
        {
            Instance = new TimerMgr();
        }
        return Instance;
    }

    public void Update(int dt)
    {
        foreach (KeyValuePair<int, TimerItem> pair in this.timer_map)
        {
            int timer_idx = pair.Key;
            TimerItem timer_item = pair.Value;
            if (timer_item.Update(dt))
            {
                if (!timer_item.IsLoop())
                {
                    this.timer_map.Remove(timer_idx);
                }
                TimerAction action = timer_item.GetAction();
                object data = timer_item.GetData();

                try
                {
                    action(data);
                }
                catch
                {

                }
            }
        }
    }

    public int AddTimer(int interval, TimerAction action, object data = null, bool is_loop = false)
    {
        TimerItem timer_item = new TimerItem(interval, action, data, is_loop);
        int timer_id = this.GetTimerId();
        this.timer_map[timer_id] = timer_item;
        return timer_id;
    }

    public void DelTimer(int timer_id)
    {
        if (this.timer_map.ContainsKey(timer_id))
        {
            this.timer_map.Remove(timer_id);
        }
    }

    private int GetTimerId()
    {
        return ++this.timer_id;
    }
}

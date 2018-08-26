using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SkillItem
{
    private Role role;
    private int skill_id;
    private List<ActionBase> action_list = new List<ActionBase>();
    private int cur_action_idx = -1;

    public SkillItem(Role role, int skill_id)
    {
        this.role = role;
        this.skill_id = skill_id;
        this.ParseAction();
    }

    public void Update(int dt)
    {
        if(this.cur_action_idx != -1)
        {
            this.action_list[this.cur_action_idx].Update(dt);
        }
    }

    public void Perform(int target_id)
    {
        if (this.action_list.Count - 1 > this.cur_action_idx)
        {
            ActionBase action = this.action_list[++this.cur_action_idx];
            action.Run(target_id);
            if (!this.role.GetSkillMgr().IsDelay())
            {
                this.Perform(target_id);
            }
        }
        else
        {
            this.cur_action_idx = -1;
            this.role.GetSkillMgr().PerformSkillEnd();
        }
    }

    public bool IsPlaying()
    {
        return false;
    }

    public bool CheckDis(Entity entity)
    {
        float dis = ThreeSpace.GetDis(this.role.GetPos(), entity.GetPos());
        return true;
    }

    public bool CheckCoolDown()
    {
        return true;
    }

    public bool CheckMpEnough()
    {
        return true;
    }

    public bool CheckPriority(SkillItem skill_item)
    {
        return true;
    }

    private void ParseAction()
    {
        Config cSkill = ConfigPool.Load("Skill");
        ConfigItem cSkillItem = cSkill[this.skill_id];
        foreach (JToken value in cSkillItem["Action"])
        {
            string str = value.ToString();
            string[] str_list = str.Split(',');
            string action_name = str_list[1];

            ActionBase action = null;
            switch (action_name)
            {
                case "Target":
                    {
                        int effect_id = int.Parse(str_list[2]);
                        action = new TargetAction(this.role, effect_id);
                    } break;
                case "Rect":
                    {
                        float face = Single.Parse(str_list[2]);
                        float dis = Single.Parse(str_list[3]);
                        int effect_id = int.Parse(str_list[4]);
                        action = new RectAction(this.role, face, dis, effect_id);
                    } break;
                case "Delay":
                    {
                        int time = int.Parse(str_list[2]);
                        action = new DelayAction(this.role, time);
                    } break;
            }

            if(action != null)
            {
                this.action_list.Add(action);
            }
        }
    }
}

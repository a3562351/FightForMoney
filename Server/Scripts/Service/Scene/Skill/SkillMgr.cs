using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SkillMgr
{
    private Role role;
    private Dictionary<int, SkillItem> skill_map = new Dictionary<int, SkillItem>();
    private SkillItem cur_skill;
    private bool is_delay = false;

    public SkillMgr(Role role)
    {
        this.role = role;
    }

    public void Init()
    {

    }

    public void Save()
    {

    }

    public void Update(int dt)
    {
        foreach (KeyValuePair<int, SkillItem> pair in this.skill_map)
        {
            pair.Value.Update(dt);
        }
    }

    public void SetDelay(bool flag)
    {
        this.is_delay = flag;
        if (!this.is_delay)
        {
            this.PerformCurSkill();
        }
    }

    public bool IsDelay()
    {
        return this.is_delay;
    }

    public bool CanPerformSkill(int skill_id, Entity entity)
    {
        if (!this.skill_map.ContainsKey(skill_id))
        {
            return false;
        }

        SkillItem skill_item = this.skill_map[skill_id];

        if(this.cur_skill != null && this.cur_skill.IsPlaying() && !skill_item.CheckPriority(this.cur_skill))
        {
            return false;
        }

        if (!skill_item.CheckCoolDown())
        {
            return false;
        }

        if (!skill_item.CheckMpEnough())
        {
            return false;
        }

        return true;
    }

    public void PerformSkill(int skill_id, int target_id = -1)
    {
        this.skill_map[skill_id].Perform(target_id);
    }

    public void PerformSkillEnd()
    {
        this.cur_skill = null;
    }

    private void PerformCurSkill()
    {

    }
}

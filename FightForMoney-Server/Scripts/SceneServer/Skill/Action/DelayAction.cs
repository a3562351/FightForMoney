using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DelayAction : ActionBase
{
    private int last_time;

    public DelayAction(Role role, int time) : base(role)
    {
        this.last_time = time;
    }

    public override void Run(int target_id)
    {
        this.role.GetSkillMgr().SetDelay(true);
    }

    public override void Update(int dt)
    {
        this.last_time -= dt;
        if(this.last_time <= 0)
        {
            this.Stop();
        }
    }

    public override void Stop()
    {
        this.role.GetSkillMgr().SetDelay(false);
    }
}

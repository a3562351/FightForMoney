using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TargetAction : ActionBase
{
    private int effect_id;

    public TargetAction(Role role, int effect_id) : base(role)
    {
        this.effect_id = effect_id;
    }

    public override void Run(int target_id)
    {

    }
}

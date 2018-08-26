using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class RectAction : ActionBase
{
    private float face;
    private float dis;
    private int effect_id;

    public RectAction(Role role, float face, float dis, int effect_id) : base(role)
    {
        this.face = face;
        this.dis = dis;
        this.effect_id = effect_id;
    }

    public override void Run(int target_id)
    {

    }
}

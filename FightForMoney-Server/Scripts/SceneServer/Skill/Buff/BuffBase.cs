using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BuffBase{
    protected Role role;
    protected int buff_id;
    protected List<EffectBase> effect_list = new List<EffectBase>();

    public BuffBase(Role role, int buff_id)
    {
        this.role = role;
        this.buff_id = buff_id;
    }

    public virtual void Update(int dt)
    {

    }
}

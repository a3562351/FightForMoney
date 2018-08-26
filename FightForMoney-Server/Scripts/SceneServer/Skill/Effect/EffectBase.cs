using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EffectBase
{
    public int owner_id;
    public int attacker_id;

    public EffectBase(int owner_id, int attacker_id)
    {
        this.owner_id = owner_id;
        this.attacker_id = attacker_id;
    }

    public virtual void Load()
    {

    }

    public virtual void UnLoad()
    {

    }
}

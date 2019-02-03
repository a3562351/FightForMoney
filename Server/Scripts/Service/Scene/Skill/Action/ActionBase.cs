using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ActionBase
{
    protected Role role;

    public ActionBase(Role role)
    {
        this.role = role;
    }

    public virtual void Run(int target_id)
    {

    }

    public virtual void Update(int dt)
    {

    }

    public virtual void Stop()
    {

    }
}

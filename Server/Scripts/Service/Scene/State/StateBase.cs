using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class StateBase
{
    private int state_type = StateType.INVALID;

    public virtual void Run()
    {

    }

    public virtual bool CanChange(Role role)
    {
        return true;
    }

    public int GetStateType()
    {
        return this.state_type;
    }
}

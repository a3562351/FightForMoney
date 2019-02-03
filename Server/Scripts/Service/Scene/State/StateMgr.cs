using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class StateMgr
{
    private Role role;
    private StateBase CurState = new IdleState();

    public StateMgr(Role role)
    {
        this.role = role;
    }

    public void Update()
    {
        this.CurState.Run();
    }

    public void CanChange(int state_code)
    {
        int cur_state_code = this.CurState.GetStateType();

    }

    public bool ChangeState(int state_code)
    {
        if (this.CurState != null)
        {
            Config cStateChange = ConfigPool.Load("StateChange");
            ConfigItem cStateChangeItem = cStateChange[this.CurState.GetStateType()];
            if(cStateChangeItem == null)
            {
                Log.Error(string.Format("StateChange Config Id:{0} Not Exist", this.CurState.GetStateType()));
                return false;
            }

            if (cStateChangeItem["CanChange"].Contains(state_code))
            {
                return false;
            }
        }

        StateBase state = null;

        switch (state_code)
        {
            case StateType.IDLE:
                {
                    state = new IdleState();
                };break;
            case StateType.RUN:
                {
                    state = new RunState();
                };break;
            case StateType.ATTACK:
                {
                    state = new AttackState();
                };break;
            case StateType.DIE:
                {
                    state = new DieState();
                };break;
            default:
                {
                    Log.Error("");
                }; break;
        }

        if(state == null || !state.CanChange(this.role))
        {
            return false;
        }

        return true;
    }

    public int GetCurStateType()
    {
        return this.CurState.GetStateType();
    }
}

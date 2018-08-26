using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BuffMgr
{
    private Role role;
    private Dictionary<int, BuffBase> magic_buff_map = new Dictionary<int, BuffBase>();
    private Dictionary<int, BuffBase> trigger_buff_map = new Dictionary<int, BuffBase>();
    private Dictionary<int, BuffBase> aura_buff_map = new Dictionary<int, BuffBase>();

    public BuffMgr(Role role)
    {
        this.role = role;
    }

    public void Update(int dt)
    {
        foreach (KeyValuePair<int, BuffBase> pair in this.magic_buff_map)
        {
            pair.Value.Update(dt);
        }
    }

    public void AddBuff(int buff_id)
    {
        Config cMagicBuff = ConfigPool.Load("MagicBuff");
        Config cTriggerBuff = ConfigPool.Load("TriggerBuff");
        Config cAuraBuff = ConfigPool.Load("AuraBuff");

        if(cMagicBuff.ContainsKey(buff_id))
        {
            MagicBuff buff = new MagicBuff(this.role, buff_id);
            this.magic_buff_map.Add(buff_id, buff);
        }
        else if(cTriggerBuff.ContainsKey(buff_id))
        {
            TriggerBuff buff = new TriggerBuff(this.role, buff_id);
            this.trigger_buff_map.Add(buff_id, buff);
        }
        else if (cAuraBuff.ContainsKey(buff_id))
        {
            AuraBuff buff = new AuraBuff(this.role, buff_id);
            this.aura_buff_map.Add(buff_id, buff);
        }
        else
        {
            Log.Debug(string.Format("Buff Id:{0} Not Exist", buff_id));
        }
    }
}

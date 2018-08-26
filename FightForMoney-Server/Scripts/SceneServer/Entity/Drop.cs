using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Drop : Entity
{
    private int belong_player_id;
    private int item_id;
    private int last_time;

    public Drop(int item_id, int belong_player_id = -1, int last_time = -1)
    {
        this.entity_type = EntityType.DROP;
        this.item_id = item_id;
        this.belong_player_id = belong_player_id;
        this.last_time = last_time;
    }

    public override void Update(int dt)
    {
        base.Update(dt);
        this.last_time -= dt;
        if(this.last_time <= 0)
        {
            this.Destory();
        }
    }

    public bool CheckIsPlayer(int player_id)
    {
        //-1为公共掉落
        if (this.belong_player_id == player_id || this.belong_player_id == -1)
        {
            return true;
        }
        return false;
    }
}

using Common.Protobuf;
using System.Collections;
using System.Collections.Generic;

delegate void PlayerIterAction(int player_id, Player player);

class PlayerMgr {
    private Dictionary<int, Player> player_map = new Dictionary<int, Player>();

    private void PlayerIteration(PlayerIterAction action)
    {
        foreach (KeyValuePair<int, Player> pair in this.player_map)
        {
            action(pair.Key, pair.Value);
        }
    }

    public void Init()
    {

    }

    public void Save()
    {

    }

    public void Update(int dt)
    {
        this.PlayerIteration(delegate (int player_id, Player player)
        {
            player.Update(dt);
        });
    }

    public void AddPlayer(Player player)
    {
        this.player_map[player.GetId()] = player;
    }

    public void DelPlayer(int player_id)
    {
        if (this.player_map.ContainsKey(player_id))
        {
            this.player_map.Remove(player_id);
        }
    }

    public Player GetPlayer(int player_id)
    {
        if (!this.player_map.ContainsKey(player_id))
        {
            string str = string.Format("[PlayerMgr:GetPlayer Player_Not_Exist][player_id:{0}]", player_id);
            Log.Info("RunTime", str);
            return null;
        }
        return this.player_map[player_id];
    }

    public void SavePlayer()
    {
        foreach (KeyValuePair<int, Player> pair in this.player_map)
        {
            if (pair.Value.IsDirty())
            {
                pair.Value.Save();
                pair.Value.SetDirty(false);
            }
        }
    }
}

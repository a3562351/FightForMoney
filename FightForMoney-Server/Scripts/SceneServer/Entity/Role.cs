using System;
using System.Collections.Generic;
using System.Linq;
using Common.Protobuf;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

class Role : Entity
{
    private SceneServer server;
    private EntityAttr attr = new EntityAttr();
    private EquipMgr equip_mgr;
    private SkillMgr skill_mgr;
    private BuffMgr buff_mgr;
    private StateMgr state_mgr;
    private Dictionary<int, Entity> aura_map = new Dictionary<int, Entity>();
    private int camp;
    private bool die = false;

    public Role(SceneServer server, int player_id) : base(player_id)
    {
        this.server = server;
        this.equip_mgr = new EquipMgr(this);
        this.skill_mgr = new SkillMgr(this);
        this.buff_mgr = new BuffMgr(this);
        this.state_mgr = new StateMgr(this);
    }

    public override void Init()
    {

    }

    public override void Save()
    {

    }

    public override void Update(int dt)
    {
        base.Update(dt);
    }

    public EntityAttr GetAttr()
    {
        return this.attr;
    }

    public EquipMgr GetEquipMgr()
    {
        return this.equip_mgr;
    }

    public SkillMgr GetSkillMgr()
    {
        return this.skill_mgr;
    }

    public BuffMgr GetBuffMgr()
    {
        return this.buff_mgr;
    }

    public virtual bool IsFriend(Entity entity)
    {
        if (this.GetEntityType() != EntityType.ROLE || entity.GetEntityType() != EntityType.ROLE)
        {
            return false;
        }

        return this.GetPlayerId() == entity.GetPlayerId();
    }

    public virtual bool IsEnemy(Entity entity)
    {
        if (this.GetEntityType() != EntityType.ROLE || entity.GetEntityType() != EntityType.ROLE)
        {
            return false;
        }

        return this.GetPlayerId() != entity.GetPlayerId();
    }

    public void ChangeScene(int scene_id, Pos pos, Face face)
    {

    }

    public void ChangeState(int state_code)
    {

    }

    public void BattleBehavior(Pos pos, Face face, int skill_id, int target_id)
    {

    }

    public override void Move(float to_pos_x, float to_pos_y, float to_pos_z)
    {
        base.Move(to_pos_x, to_pos_y, to_pos_z);
        foreach(KeyValuePair<int, Entity> pair in this.aura_map)
        {
            pair.Value.Move(to_pos_x, to_pos_y, to_pos_z);
        }
    }

    public void PerformSkill(int skill_id, int target_id)
    {
        Entity entity = SceneMgr.GetInstance().GetEntityById(target_id);
        if (this.skill_mgr.CanPerformSkill(skill_id, entity))
        {
            this.skill_mgr.PerformSkill(skill_id, target_id);
        }
    }

    public void SendMsg(IMessage protocol)
    {
        if (this.IsBelongPlayer())
        {
            Player player = PlayerMgr.GetInstance().GetPlayer(this.player_id);
            if (player != null)
            {
                player.SendMsg(protocol);
            }
        }
    }

    public void SendAOEMsg(IMessage protocol, bool include_self = true)
    {
        AOI scene_aoi = this.GetScene().GetSceneAOI();
        
        //一个玩家多个角色的情况下只需发送一次
        Dictionary<int, bool> is_send = new Dictionary<int, bool>();
        List<int> target_id_list = scene_aoi.GetRangeEntityId(entity_id);
        if (include_self)
        {
            target_id_list.Add(entity_id);
        }

        foreach (int target_id in target_id_list)
        {
            Entity entity = SceneMgr.GetInstance().GetEntityById(target_id);
            if (entity != null && entity.IsBelongPlayer())
            {
                int player_id = entity.GetPlayerId();
                Player player = PlayerMgr.GetInstance().GetPlayer(player_id);
                if (player != null && !is_send.ContainsKey(player_id))
                {
                    player.SendMsg(protocol);
                    is_send[player_id] = true;
                }
            }
        }
    }

    private void BattleStateUpdate()
    {

    }
}

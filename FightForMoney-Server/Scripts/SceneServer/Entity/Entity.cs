using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Entity
{
    protected int player_id;
    protected int entity_id;
    protected int scene_idx;
    protected int entity_type = EntityType.INVALID;
    protected Pos pos = new Pos();

    public Entity(int player_id = -1)
    {
        this.player_id = player_id;
    }

    public int GetPlayerId()
    {
        return this.player_id;
    }

    public int GetEntityId()
    {
        return this.entity_id;
    }

    public void SetEntityId(int entity_id)
    {
        this.entity_id = entity_id;
    }

    public int GetSceneIdx()
    {
        return this.scene_idx;
    }

    public void SetSceneIdx(int scene_idx)
    {
        this.scene_idx = scene_idx;
    }

    public int GetEntityType()
    {
        return this.entity_type;
    }

    public Pos GetPos()
    {
        return this.pos;
    }

    public Scene GetScene()
    {
        return SceneMgr.GetInstance().GetScene(this.scene_idx);
    }

    public bool IsBelongPlayer()
    {
        return this.player_id != -1;
    }

    public virtual void Init()
    {

    }

    public virtual void Save()
    {

    }

    public virtual void Update(int dt)
    {

    }

    public virtual void Move(float to_pos_x, float to_pos_y, float to_pos_z)
    {
        this.pos.SetPos(to_pos_x, to_pos_y, to_pos_z);
        this.GetScene().EntityMove(this.entity_id, this.pos);
    }

    public virtual void EnterVision(Entity entity)
    {

    }

    public virtual void LeaveVision(Entity entity)
    {

    }

    public virtual bool Destory()
    {
        Scene scene = SceneMgr.GetInstance().GetScene(this.scene_idx);
        if (scene != null)
        {

            return false;
        }
        scene.DelEntity(this);
        return true;
    }

    public virtual void OnDestory()
    {

    }
}

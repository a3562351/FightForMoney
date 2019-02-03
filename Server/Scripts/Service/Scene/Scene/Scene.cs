using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Scene
{
    private int scene_idx;
    private int scene_id;
    private Dictionary<int, Entity> entity_map = new Dictionary<int, Entity>();
    public AOI aoi = new AOI();
    public Nav nav = new Nav();

    public Scene(int scene_idx, int scene_id)
    {
        this.scene_idx = scene_idx;
        this.scene_id = scene_id;
    }

    public int GetSceneIdx()
    {
        return this.scene_idx;
    }

    public int GetSceneId()
    {
        return this.scene_id;
    }

    public AOI GetSceneAOI()
    {
        return this.aoi;
    }

    public Nav GetSceneNav()
    {
        return this.nav;
    }

    public void Update(int dt)
    {
        foreach (KeyValuePair<int, Entity> pair in this.entity_map)
        {
            pair.Value.Update(dt);
        }
    }

    public bool AddEntity(Entity entity)
    {
        int entity_id = Server.GetInstance().GetService<SceneService>().GetSceneMgr().GetEntityId();
        entity.SetEntityId(entity_id);
        entity.SetSceneIdx(this.GetSceneIdx());
        this.entity_map[entity_id] = entity;
        return true;
    }

    public bool DelEntity(Entity entity)
    {
        return this.DelEntityById(entity.GetEntityId());
    }

    public bool DelEntityById(int entity_id)
    {
        if (!this.entity_map.ContainsKey(entity_id))
        {
            return false;
        }

        Entity entity = this.entity_map[entity_id];
        entity.OnDestory();
        this.entity_map.Remove(entity_id);

        return true;
    }

    public Entity GetEntityById(int entity_id)
    {
        if (!this.entity_map.ContainsKey(entity_id))
        {
            return null;
        }
        return this.entity_map[entity_id];
    }

    public List<Role> GetFriendList(Role role)
    {
        List<Role> friend_list = new List<Role>();
        foreach (KeyValuePair<int, Entity> pair in this.entity_map)
        {
            Entity entity = pair.Value;
            if (entity.GetEntityType() == EntityType.ROLE && role.IsFriend((Role)entity))
            {
                friend_list.Add((Role)entity);
            }
        }
        return friend_list;
    }

    public List<Role> GetEnemyList(Role role)
    {
        List<Role> enemy_list = new List<Role>();
        foreach (KeyValuePair<int, Entity> pair in this.entity_map)
        {
            Entity entity = pair.Value;
            if (entity.GetEntityType() == EntityType.ROLE && role.IsEnemy((Role)entity))
            {
                enemy_list.Add((Role)entity);
            }
        }
        return enemy_list;
    }

    public void Destory()
    {
        Server.GetInstance().GetService<SceneService>().GetSceneMgr().DelScene(this);
    }

    public void OnDestory()
    {

    }

    public void EntityMove(int entity_id, Pos pos)
    {
        this.aoi.Move(entity_id, pos.X, pos.Z);
    }

    private void HandleAOIEvent()
    {
        AOIEventMap enter_map = this.aoi.GetEnterMap();
        AOIEventMap leave_map = this.aoi.GetLeaveMap();

        foreach (KeyValuePair<int, List<int>> pair in enter_map)
        {
            Entity entity = this.GetEntityById(pair.Key);
            if(entity != null)
            {
                //进入Entity的视野
                foreach (int target_entity_id in pair.Value)
                {
                    Entity target_entity = this.GetEntityById(target_entity_id);
                    if(target_entity != null)
                    {
                        entity.EnterVision(target_entity);
                    }
                }
            }
        }

        foreach (KeyValuePair<int, List<int>> pair in leave_map)
        {
            Entity entity = this.GetEntityById(pair.Key);
            if (entity != null)
            {
                //离开Entity的视野
                foreach (int target_entity_id in pair.Value)
                {
                    Entity target_entity = this.GetEntityById(target_entity_id);
                    if (target_entity != null)
                    {
                        entity.LeaveVision(target_entity);
                    }
                }
            }
        }

        this.aoi.ClearEvent();
    }
}

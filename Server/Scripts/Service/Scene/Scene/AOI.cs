using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AOIEventType
{
    public const int Enter = 1;         //进入
    public const int Leave = 2;         //离开
    public const int On = 3;            //保持
    public const int MoveEnter = 4;     //走进
    public const int MoveLeave = 5;     //走出
}

class AOIEventMap : IEnumerable
{
    private Dictionary<int, List<int>> map = new Dictionary<int, List<int>>();

    public void Add(int entity_id, int target_entity_id)
    {
        if (!this.map.ContainsKey(entity_id))
        {
            this.map.Add(entity_id, new List<int>());
        }

        List<int> entity_id_list = this.map[entity_id];
        if (!entity_id_list.Contains(target_entity_id))
        {
            entity_id_list.Add(entity_id);
        }
    }

    public IEnumerator GetEnumerator()
    {
        foreach (var pair in this.map)
        {
            yield return pair;
        }
    }
}

class AOIEntity
{
    public int EntityId;
    public float PosX;
    public float PosZ;
    public float NewPosX;
    public float NewPosZ;
    public float EyeSize;
    public AOIEntity PerEntity;
    public AOIEntity NextEntity;

    public AOIEntity(int entity_id, float pos_x, float pos_z, float eye_size)
    {
        this.EntityId = entity_id;
        this.PosX = pos_x;
        this.PosZ = pos_z;
        this.NewPosX = this.PosX;
        this.NewPosZ = this.PosZ;
        this.EyeSize = eye_size;
    }

    public bool InRect(AOIEntity entity, float rect_x, float rect_z)
    {
        return this.InRectWithXZ(entity.PosX, entity.PosZ, rect_x, rect_z);
    }

    public bool InRectWithXZ(float pos_x, float pos_z, float rect_x, float rect_z)
    {
        if (Math.Abs(this.PosX - pos_x) <= rect_x && Math.Abs(this.PosZ - pos_z) <= rect_z)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    public float GetDis(AOIEntity entity)
    {
        return this.GetDisWithXZ(entity.PosX, entity.PosZ);
    }

    public float GetDisWithXZ(float pos_x, float pos_z)
    {
        double dis = Math.Pow(this.PosX - pos_x, 2) + Math.Pow(this.PosZ - pos_z, 2);
        return (float)dis;
    }

    public bool CanSee(AOIEntity entity)
    {
        float dis = this.GetDis(entity);
        return this.EyeSize >= dis;
    }

    public bool NewCanSee(AOIEntity entity)
    {
        double dis = Math.Pow(this.NewPosX - entity.NewPosX, 2) + Math.Pow(this.NewPosZ - entity.NewPosZ, 2);
        return this.EyeSize >= dis;
    }

    public void UpdatePos()
    {
        this.PosX = this.NewPosX;
        this.PosZ = this.NewPosZ;
    }
}

class AOI
{
    private Dictionary<int, AOIEntity> entity_map = new Dictionary<int, AOIEntity>();
    private AOIEntity head_entity;
    private AOIEntity tail_entity;
    private float max_eye_size = 0;
    private AOIEventMap enter_map = new AOIEventMap();
    private AOIEventMap leave_map = new AOIEventMap();

    public AOI()
    {
        //辅助节点，需保证在所有节点视野外
        this.head_entity = new AOIEntity(0, -1000, 0, 0);
        this.tail_entity = new AOIEntity(0, 1000, 0, 0);

        this.head_entity.NextEntity = this.tail_entity;
        this.tail_entity.PerEntity = this.head_entity;
    }

    public void Enter(int entity_id, float pos_x, float pos_y, float eye_size)
    {
        if (this.entity_map.ContainsKey(entity_id))
        {
            return;
        }

        if(this.max_eye_size < eye_size)
        {
            this.max_eye_size = eye_size;
        }

        AOIEntity entity = new AOIEntity(entity_id, pos_x, pos_y, eye_size);

        AOIEntity next_entity = this.head_entity;
        bool is_insert = false;

        while (next_entity != null)
        {
            //找到合适位置
            if (entity.PosX < next_entity.PosX && !is_insert)
            {
                entity.PerEntity = next_entity.PerEntity;
                next_entity.PerEntity.NextEntity = entity;
                entity.NextEntity = next_entity;
                next_entity.PerEntity = entity;
                is_insert = true;
            }

            //可能触发的视野事件
            if (Math.Abs(entity.PosX - next_entity.PosX) <= this.max_eye_size)
            {
                if (entity.CanSee(next_entity))
                {
                    this.MakeEvent(entity.EntityId, next_entity.EntityId, AOIEventType.Enter);
                }

                if (next_entity.CanSee(entity))
                {
                    this.MakeEvent(next_entity.EntityId, entity.EntityId, AOIEventType.Enter);
                }
            }

            //遍历到最大视野所及的范围
            if (next_entity.PosX > entity.PosX + this.max_eye_size)
            {
                break;
            }

            next_entity = next_entity.NextEntity;
        }
        this.entity_map.Add(entity_id, entity);
    }

    public void Leave(int entity_id)
    {
        if (!this.entity_map.ContainsKey(entity_id))
        {
            return;
        }

        AOIEntity entity = this.entity_map[entity_id];
        AOIEntity per_entity = entity.PerEntity;
        AOIEntity next_entity = entity.NextEntity;

        while (per_entity != null)
        {
            //可能触发的视野事件
            if (Math.Abs(entity.PosX - per_entity.PosX) <= this.max_eye_size)
            {
                if (entity.CanSee(per_entity))
                {
                    this.MakeEvent(entity.EntityId, per_entity.EntityId, AOIEventType.Leave);
                }

                if (per_entity.CanSee(entity))
                {
                    this.MakeEvent(per_entity.EntityId, entity.EntityId, AOIEventType.Leave);
                }
            }
            else
            {
                break;
            }

            per_entity = per_entity.PerEntity;
        }

        while(next_entity != null)
        {
            //可能触发的视野事件
            if (Math.Abs(entity.PosX - next_entity.PosX) <= this.max_eye_size)
            {
                if (entity.CanSee(next_entity))
                {
                    this.MakeEvent(entity.EntityId, next_entity.EntityId, AOIEventType.Leave);
                }

                if (next_entity.CanSee(entity))
                {
                    this.MakeEvent(next_entity.EntityId, entity.EntityId, AOIEventType.Leave);
                }
            }
            else
            {
                break;
            }

            next_entity = next_entity.NextEntity;
        }

        entity.PerEntity.NextEntity = entity.NextEntity;
        entity.NextEntity.PerEntity = entity.PerEntity;
        this.entity_map.Remove(entity.EntityId);
    }

    public void Move(int entity_id, float to_pos_x, float to_pos_y)
    {
        if (!this.entity_map.ContainsKey(entity_id))
        {
            return;
        }

        AOIEntity entity = this.entity_map[entity_id];
        float from_pos_x = entity.PosX;
        float from_pos_y = entity.PosZ;
        entity.NewPosX = to_pos_x;
        entity.NewPosZ = to_pos_y;

        if(from_pos_x == to_pos_x)
        {
            entity.UpdatePos();
            return;
        }

        AOIEntity per_entity = entity.PerEntity;
        AOIEntity next_entity = entity.NextEntity;

        if (to_pos_x < from_pos_x)
        {
            while (per_entity != null)
            {
                //向左一直找到合适的位置
                if (to_pos_x < per_entity.PosX)
                {
                    entity.PerEntity = per_entity.PerEntity;
                    per_entity.PerEntity.NextEntity = entity;
                    per_entity.NextEntity = entity.NextEntity;
                    entity.NextEntity.PerEntity = per_entity;
                    per_entity.PerEntity = entity;
                    entity.NextEntity = per_entity;
                }

                //可能触发的视野事件
                if (to_pos_x - this.max_eye_size <= per_entity.PosX)
                {
                    this.CheckMoveEvent(entity, per_entity);
                }
                else
                {
                    break;
                }

                per_entity = per_entity.PerEntity;
            }

            while (next_entity != null)
            {
                //可能触发的视野事件
                if (from_pos_x + this.max_eye_size >= next_entity.PosX)
                {
                    this.CheckMoveEvent(entity, next_entity);
                }
                else
                {
                    break;
                }

                next_entity = per_entity.NextEntity;
            }
        }
        else
        {
            while (next_entity != null)
            {
                //向右一直找到合适的位置
                if (to_pos_x > next_entity.PosX)
                {
                    entity.NextEntity = next_entity.NextEntity;
                    next_entity.NextEntity.PerEntity = entity;
                    next_entity.PerEntity = entity.PerEntity;
                    entity.PerEntity.NextEntity = next_entity;
                    next_entity.NextEntity = entity;
                    entity.PerEntity = next_entity;
                }

                //可能触发的视野事件
                if (to_pos_x + this.max_eye_size >= next_entity.PosX)
                {
                    this.CheckMoveEvent(entity, next_entity);
                }
                else
                {
                    break;
                }

                next_entity = per_entity.NextEntity;
            }

            while (per_entity != null)
            {
                //可能触发的视野事件
                if (from_pos_x - this.max_eye_size <= per_entity.PosX)
                {
                    this.CheckMoveEvent(entity, per_entity);
                }
                else
                {
                    break;
                }

                per_entity = per_entity.PerEntity;
            }
        }

        entity.UpdatePos();
    }

    private void CheckMoveEvent(AOIEntity entity, AOIEntity target)
    {
        //之前看不见现在看得见
        if (!entity.CanSee(target) && entity.NewCanSee(target))
        {
            this.MakeEvent(entity.EntityId, target.EntityId, AOIEventType.MoveEnter);
        }

        if (!target.CanSee(entity) && target.NewCanSee(entity))
        {
            this.MakeEvent(target.EntityId, entity.EntityId, AOIEventType.MoveEnter);
        }

        //之前看得见现在看不见
        if (entity.CanSee(target) && !entity.NewCanSee(target))
        {
            this.MakeEvent(entity.EntityId, target.EntityId, AOIEventType.MoveLeave);
        }

        if (target.CanSee(entity) && !target.NewCanSee(entity))
        {
            this.MakeEvent(target.EntityId, entity.EntityId, AOIEventType.MoveLeave);
        }
    }

    private void MakeEvent(int entity_id, int target_id, int event_type)
    {
        if(event_type == AOIEventType.Enter || event_type == AOIEventType.MoveEnter)
        {
            this.enter_map.Add(entity_id, target_id);
        }

        if(event_type == AOIEventType.Leave || event_type == AOIEventType.MoveLeave)
        {
            this.leave_map.Add(entity_id, target_id);
        }
    }

    public AOIEventMap GetEnterMap()
    {
        return this.enter_map;
    }

    public AOIEventMap GetLeaveMap()
    {
        return this.leave_map;
    }

    public void ClearEvent()
    {
        this.enter_map = new AOIEventMap();
        this.leave_map = new AOIEventMap();
    }

    public List<int> GetRangeEntityId(int entity_id)
    {
        List<int> entity_id_list = new List<int>();

        if (this.entity_map.ContainsKey(entity_id))
        {
            AOIEntity entity = this.entity_map[entity_id];
            AOIEntity per_entity = entity.PerEntity;
            AOIEntity next_entity = entity.NextEntity;

            while (per_entity != null)
            {
                if (Math.Abs(entity.PosX - per_entity.PosX) <= this.max_eye_size)
                {
                    if (per_entity.CanSee(entity))
                    {
                        entity_id_list.Add(per_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                per_entity = per_entity.PerEntity;
            }

            while (next_entity != null)
            {
                if (Math.Abs(entity.PosX - next_entity.PosX) <= this.max_eye_size)
                {
                    if (next_entity.CanSee(entity))
                    {
                        entity_id_list.Add(next_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                next_entity = next_entity.NextEntity;
            }
        }

        return entity_id_list;
    }

    public List<int> GetRadiusEntityId(int entity_id, float radius)
    {
        List<int> entity_id_list = new List<int>();

        if (this.entity_map.ContainsKey(entity_id))
        {
            AOIEntity entity = this.entity_map[entity_id];
            AOIEntity per_entity = entity.PerEntity;
            AOIEntity next_entity = entity.NextEntity;

            while (per_entity != null)
            {
                if (Math.Abs(entity.PosX - per_entity.PosX) <= radius)
                {
                    if (entity.GetDis(per_entity) <= radius)
                    {
                        entity_id_list.Add(per_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                per_entity = per_entity.PerEntity;
            }

            while (next_entity != null)
            {
                if (Math.Abs(entity.PosX - next_entity.PosX) <= radius)
                {
                    if (entity.GetDis(next_entity) <= radius)
                    {
                        entity_id_list.Add(next_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                next_entity = next_entity.NextEntity;
            }
        }

        return entity_id_list;
    }

    public List<int> GetPosRadiusEntityId(float pos_x, float pos_z, float radius)
    {
        List<int> entity_id_list = new List<int>();

        AOIEntity next_entity = this.head_entity.NextEntity;
        while (next_entity != null)
        {
            if (next_entity.PosX - pos_x <= radius)
            {
                if (next_entity.GetDisWithXZ(pos_x, pos_z) <= radius)
                {
                    entity_id_list.Add(next_entity.EntityId);
                }
            }
            else
            {
                break;
            }

            next_entity = next_entity.NextEntity;
        }

        return entity_id_list;
    }

    public List<int> GetRectEntityId(int entity_id, float rect_x, float rect_z)
    {
        List<int> entity_id_list = new List<int>();

        if (this.entity_map.ContainsKey(entity_id))
        {
            AOIEntity entity = this.entity_map[entity_id];
            AOIEntity per_entity = entity.PerEntity;
            AOIEntity next_entity = entity.NextEntity;

            while (per_entity != null)
            {
                if (Math.Abs(entity.PosX - per_entity.PosX) <= rect_x)
                {
                    if (entity.InRect(per_entity, rect_x, rect_z))
                    {
                        entity_id_list.Add(per_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                per_entity = per_entity.PerEntity;
            }

            while (next_entity != null)
            {
                if (Math.Abs(entity.PosX - next_entity.PosX) <= rect_x)
                {
                    if (entity.InRect(next_entity, rect_x, rect_z))
                    {
                        entity_id_list.Add(next_entity.EntityId);
                    }
                }
                else
                {
                    break;
                }

                next_entity = next_entity.NextEntity;
            }
        }

        return entity_id_list;
    }

    public List<int> GetPosRectEntityId(float pos_x, float pos_z, float rect_x, float rect_z)
    {
        List<int> entity_id_list = new List<int>();

        AOIEntity next_entity = this.head_entity.NextEntity;
        while (next_entity != null)
        {
            if (pos_x - next_entity.PosX <= rect_x)
            {
                if (next_entity.InRectWithXZ(pos_x, pos_z, rect_x, rect_z))
                {
                    entity_id_list.Add(next_entity.EntityId);
                }
            }
            else
            {
                break;
            }

            next_entity = next_entity.NextEntity;
        }

        return entity_id_list;
    }
}

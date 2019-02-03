using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

delegate void SceneIterAction(int scene_idx, Scene scene);

class SceneMgr
{
    private int max_entity_id = 0;
    private int scene_idx = 10000;
    private Dictionary<int, Scene> scene_map = new Dictionary<int, Scene>();
    private Dictionary<int, int> entity_id_to_scene_idx = new Dictionary<int, int>();

    private void SceneIteration(SceneIterAction action)
    {
        foreach (KeyValuePair<int, Scene> pair in this.scene_map)
        {
            action(pair.Key, pair.Value);
        }
    }

    public void Update(int dt)
    {
        this.SceneIteration(delegate (int scene_idx, Scene scene)
        {
            scene.Update(dt);
        });
    }

    public bool AddScene(int scene_id)
    {
        Config cScene = ConfigPool.Load("Scene");
        ConfigItem cSceneItem = cScene[scene_id];
        if (cSceneItem == null)
        {
            Log.Error(string.Format("Scene Id:{0} Not Exist", scene_id));
            return false;
        }

        int scene_idx = this.GetSceneIdx();
        Scene scene = new Scene(scene_idx, scene_id);
        this.scene_map[scene_idx] = scene;

        Log.Debug(string.Format("Scene Id:{0} Idx:{1} Create", scene_id, scene_idx));

        return true;
    }

    public bool DelScene(Scene scene)
    {
        return this.DelSceneByIdx(scene.GetSceneIdx());
    }

    public bool DelSceneByIdx(int scene_idx)
    {
        if (!this.scene_map.ContainsKey(scene_idx))
        {
            return false;
        }

        Scene scene = this.scene_map[scene_idx];
        scene.OnDestory();
        this.scene_map.Remove(scene_idx);

        return true;
    }

    public int GetEntityId()
    {
        return ++this.max_entity_id;
    }

    public Scene GetScene(int idx)
    {
        if (!this.scene_map.ContainsKey(idx))
        {
            return null;
        }
        return this.scene_map[idx];
    }

    public Entity GetEntityById(int entity_id)
    {
        if (!this.entity_id_to_scene_idx.ContainsKey(entity_id))
        {
            return null;
        }
        int scene_idx = this.entity_id_to_scene_idx[entity_id];

        Scene scene = this.GetScene(scene_idx);
        if(scene == null)
        {
            return null;
        }

        return scene.GetEntityById(entity_id);
    }

    private int GetSceneIdx()
    {
        return ++this.scene_idx;
    }
}

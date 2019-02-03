using Common.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class LandMgr
{
    private Dictionary<string, Land> land_map = new Dictionary<string, Land>();

    public void Load()
    {
        string[] files = Directory.GetFiles(PathTool.GetOriginMapPath(), "*.Data");
        foreach (string path in files)
        {
            string map_name = Path.GetFileNameWithoutExtension(path);
            this.land_map[map_name] = new Land(DataTool.LoadMapInfo(map_name));
        }
    }

    public Land GetLand(string name)
    {
        if (!this.land_map.ContainsKey(name))
        {
            return null;
        }
        return this.land_map[name];
    }
}

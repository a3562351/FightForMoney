using Common.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Land
{
    private MapInfo map_info;

    public Land(MapInfo map_info)
    {
        this.map_info = map_info;
    }

    public MapInfo GetData()
    {
        return this.map_info;
    }
}

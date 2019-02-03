using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ViewTool
{
    public static Dictionary<string, GameObject> CreateViewMap(GameObject root)
    {
        Dictionary<string, GameObject> view_map = new Dictionary<string, GameObject>();
        Recursive(root, view_map);
        return view_map;
    }

    private static void Recursive(GameObject root, Dictionary<string, GameObject> view_map)
    {
        foreach (Transform child in root.transform)
        {
            view_map[child.name] = child.gameObject;
            Recursive(child.gameObject, view_map);
        }
    }
}

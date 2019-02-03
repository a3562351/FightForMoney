using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PoolManager {
    private static List<GameObject> obj_list = new List<GameObject>();

    public static GameObject GetObj(string str)
    {
        if(obj_list.Count <= 0)
        {

        }
        else
        {

        }
        return null;
    }

    public static void ReleaseObj(GameObject obj)
    {
        obj.SetActive(false);
        obj_list.Add(obj);
    }
}

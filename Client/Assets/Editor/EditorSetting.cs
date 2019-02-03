using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class EditorSetting : MonoBehaviour {
    private static string[] TagList = new string[]
    {
        GridTag.TERRAIN, GridTag.BUILD
    };

    private static Dictionary<int, string> LayerMap = new Dictionary<int, string>()
    {
        {LayerNum.EDIT, "Edit"},
        {LayerNum.TERRAIN, "Terrain"},
        {LayerNum.BUILD, "Build"},
    };

    [MenuItem("Custom/CreateTagAndLayer")]
    private static void CreateTagAndLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        tagsProp.ClearArray();
        tagManager.ApplyModifiedProperties();
        for (int i = 0; i < TagList.Length; i++)
        {
            tagsProp.InsertArrayElementAtIndex(i);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
            sp.stringValue = TagList[i];
            tagManager.ApplyModifiedProperties();
        }

        SerializedProperty layersProp = tagManager.FindProperty("layers");
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (LayerMap.ContainsKey(i))
            {
                sp.stringValue = LayerMap[i];
            }
            else
            {
                sp.stringValue = null;
            }
            tagManager.ApplyModifiedProperties();
        }
    }
}

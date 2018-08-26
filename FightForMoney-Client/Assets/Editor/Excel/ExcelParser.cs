using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

class ExcelParser : EditorWindow
{
    [MenuItem("Custom/ExcelParser")]
    private static void CreateExcel()
    {
        string path = Application.dataPath + "/Editor/Excel/";
        string[] files = Directory.GetFiles(path, "*.xlsx");
        foreach (string file in files)
        {
            Parse(file);
        }
    }

    private static void Parse(string path)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelReader.AsDataSet();
        DataTable describe = result.Tables[0];
        if (describe == null)
        {
            return;
        }

        Dictionary<string, string> file_map = new Dictionary<string, string>();
        Dictionary<string, string> key_map = new Dictionary<string, string>();
        for (int i = 1; i < describe.Rows.Count; i++)
        {
            string table = describe.Rows[i][0] as string;
            string file = describe.Rows[i][1] as string;
            string key = describe.Rows[i][2] as string;

            file_map[table] = file;
            key_map[table] = key;
        }

        for (int i = 1; i < result.Tables.Count; i++)
        {
            Config config = new Config();
            DataTable table = result.Tables[i];

            if (file_map[table.TableName] != null && !GridIsEmpty(table.Rows[0]) && !GridIsEmpty(table.Rows[1]))
            {
                int key_idx = -1;
                Dictionary<int, string> field_map = new Dictionary<int, string>();
                Dictionary<int, string> type_map = new Dictionary<int, string>();

                for (int col = 0; col < table.Columns.Count; col++)
                {
                    //连续的列才能导出
                    if (GridIsEmpty(table.Rows[0][col]) || GridIsEmpty(table.Rows[1][col]))
                    {
                        break;
                    }

                    if (table.Rows[0][col].ToString().Equals(key_map[table.TableName]))
                    {
                        key_idx = col;
                    }

                    field_map[col] = table.Rows[0][col].ToString();
                    type_map[col] = table.Rows[1][col].ToString();
                }

                for (int row = 3; row < table.Rows.Count; row++)
                {
                    //连续的行才能导出
                    if (GridIsEmpty(table.Rows[row][0]))
                    {
                        break;
                    }

                    ConfigItem item = new ConfigItem();
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        if (!field_map.ContainsKey(col))
                        {
                            break;
                        }

                        string key = field_map[col];
                        JToken value = ParseType(type_map[col], table.Rows[row][col]);
                        item[key] = value;
                    }
                    object config_key = key_idx == -1 ? config.Count : table.Rows[row][key_idx];
                    config[config_key] = item;
                }
                SaveAsJson(file_map[table.TableName], config);
            }
        }
    }

    private static JToken ParseType(string type, object content)
    {
        JToken json_token;
        switch (type)
        {
            case "int":
                {
                    json_token = new JValue(Convert.ToInt32(content));
                }
                break;
            case "float":
                {
                    json_token = new JValue(Convert.ToSingle(content));
                }
                break;
            case "double":
                {
                    json_token = new JValue(Convert.ToDouble(content));
                }
                break;
            case "string":
                {
                    json_token = new JValue(content.ToString());
                }
                break;
            case "array1-1":
                {
                    JArray json_array = new JArray();
                    char[] separator_list = { '|' };
                    ParseArray(json_array, content.ToString(), separator_list, 0);
                    json_token = json_array;
                }
                break;
            case "array2-1":
                {
                    JArray json_array = new JArray();
                    char[] separator_list = { '#', '|' };
                    ParseArray(json_array, content.ToString(), separator_list, 0);
                    json_token = json_array;
                }
                break;
            default:
                {
                    json_token = new JValue(content.ToString());
                }
                break;
        }
        return json_token;
    }

    private static void ParseArray(JArray json_array, string str, char[] separator_list, int index)
    {
        if (separator_list.Length < index + 1)
        {
            return;
        }

        char separator = separator_list[index];
        string[] str_list = str.Split(separator);

        if (separator_list.Length - 1 > index)
        {
            for (int i = 0; i < str_list.Length; i++)
            {
                JArray sub_json_array = new JArray();
                json_array.Add(sub_json_array);
                string sub_str = str_list[i];
                ParseArray(sub_json_array, sub_str, separator_list, index + 1);
            }
        }
        else
        {
            for (int i = 0; i < str_list.Length; i++)
            {
                string sub_str = str_list[i];
                int value;
                JValue json_value;
                if (!int.TryParse(sub_str, out value))
                {
                    json_value = new JValue(sub_str);
                }
                else
                {
                    json_value = new JValue(value);
                }
                json_array.Add(json_value);
            }
        }
    }

    private static bool GridIsEmpty(object grid)
    {
        return grid == null || grid.ToString().Equals("");
    }

    private static void SaveAsJson(string file_name, object config)
    {
        string json = JsonConvert.SerializeObject(config);
        string path = Application.dataPath + "/Scripts/Common/Config/" + file_name + ".json";

        FileStream fs = File.Create(path);
        byte[] bts = System.Text.Encoding.UTF8.GetBytes(json);
        fs.Write(bts, 0, bts.Length);
        fs.Close();

        Debug.Log("Parser Excel:" + file_name);
    }
}

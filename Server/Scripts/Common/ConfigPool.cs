using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

public class ConfigItem : JObject
{

}

public class Config : Dictionary<object, ConfigItem>
{

}

public class ParseConfig : Dictionary<object, JObject>
{

}

public class ConfigPool {
    private static Dictionary<string, Config> config_map = new Dictionary<string, Config>();

    public static Config Load(string file_name)
    {
        if (!config_map.ContainsKey(file_name))
        {
            string path = Environment.CurrentDirectory + "/Scripts/Common/Config/" + file_name + ".json";
            if (File.Exists(path)) {
                FileStream fs = File.Open(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string json = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                
                config_map[file_name] = Convert(JsonConvert.DeserializeObject<ParseConfig>(json));
            }
            else
            {
                return null;
            }
        }
        return config_map[file_name];
    }

    public static Config Convert(ParseConfig config)
    {
        Config convert_config = new Config();
        foreach (KeyValuePair<object, JObject> pair in config)
        {
            object key;
            int ret;
            if (int.TryParse(pair.Key.ToString(), out ret))
            {
                key = ret;
            }
            else
            {
                key = pair.Key.ToString();
            }

            ConfigItem config_item = new ConfigItem();
            foreach (KeyValuePair<string, JToken> json_pair in pair.Value)
            {
                config_item[json_pair.Key] = json_pair.Value;
            }
            convert_config.Add(key, config_item);
        }
        return convert_config;
    }

    public static void Release(string file_name)
    {
        if (config_map.ContainsKey(file_name))
        {
            config_map.Remove(file_name);
        }
    }

    public static bool HaveValue(JToken array, JToken target_value)
    {
        foreach(JToken value in array)
        {
            if (value.Equals(target_value))
            {
                return true;
            }
        }
        return false;
    }
}

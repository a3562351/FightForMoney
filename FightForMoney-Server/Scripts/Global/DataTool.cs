using Common.Protobuf;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class DataTool {
    //LitJson
    //若包含Dictionary结构，则key的类型必须是string，而不能是int类型（如需表示id等），否则无法正确解析！
    //若需要小数，要使用double类型，而不能使用float，可后期在代码里再显式转换为float类型。

    public static string SaveAsJSON<T>(T obj)
    {
        return LitJson.JsonMapper.ToJson(obj);
    }

    public static T LoadFromJSON<T>(string json)
    {
        return LitJson.JsonMapper.ToObject<T>(json);
    }

    public static MapInfo LoadMapInfo(string map_name)
    {
        string path = PathTool.GetMapSavePath(map_name);
        if (PathTool.IsExistFile(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            MapInfo data = MapInfo.Parser.ParseFrom(file);
            file.Close();
            Log.Debug("Load From:" + path);

            return data;
        }
        else
        {
            //加载原始地图数据
            path = PathTool.GetOriginMapPath(map_name);
            if (PathTool.IsExistFile(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                MapInfo data = MapInfo.Parser.ParseFrom(file);
                file.Close();
                Log.Debug("Load From:" + path);

                return data;
            }
            else
            {
                return null;
            }
        }
    }

    public static void SaveMapInfo(MapInfo map_info)
    {
        string path = PathTool.GetMapSavePath(map_info.CommonInfo.Name);
        FileStream file = File.Create(path);
        map_info.WriteTo(file);
        file.Close();
        Log.Debug("Save To:" + path);
    }

    public static void SaveLServerData(LoginServerData data)
    {
        string path = PathTool.GetLServerSavePath();
        FileStream file = File.Create(path);
        data.WriteTo(file);
        file.Close();
        Log.Debug("Save To:" + path);
    }

    public static LoginServerData LoadLServerData()
    {
        string path = PathTool.GetLServerSavePath();
        if (PathTool.IsExistFile(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            LoginServerData data = LoginServerData.Parser.ParseFrom(file);
            file.Close();
            Log.Debug("Load From:" + path);

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveAccount(AccountInfo account_info)
    {
        string path = PathTool.GetAccountSavePath(account_info.Account);
        FileStream file = File.Create(path);
        account_info.WriteTo(file);
        file.Close();
        Log.Debug("Save To:" + path);
    }

    public static AccountInfo LoadAccountData(string account)
    {
        string path = PathTool.GetAccountSavePath(account);
        if (PathTool.IsExistFile(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            AccountInfo account_info = AccountInfo.Parser.ParseFrom(file);
            file.Close();
            Log.Debug("Load From:" + path);

            return account_info;
        }
        else
        {
            return null;
        }
    }

    public static void SaveUser(UserInfo user_info)
    {
        string path = PathTool.GeUserSavePath(user_info.UserId);
        FileStream file = File.Create(path);
        user_info.WriteTo(file);
        file.Close();
        Log.Debug("Save To:" + path);
    }

    public static UserInfo LoadUser(int user_id)
    {
        string path = PathTool.GeUserSavePath(user_id);
        if (PathTool.IsExistFile(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            UserInfo user_info = UserInfo.Parser.ParseFrom(file);
            file.Close();
            Log.Debug("Load From:" + path);

            return user_info;
        }
        else
        {
            return null;
        }
    }

    public static void SavePlayer(int player_id, PlayerStruct player_struct)
    {
        //FileStream fs = File.Create(PathTool.GetPlayerSavePath(player_id));
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(fs, player_struct);
        //file.Close();


        //string json = SaveAsJSON(player_struct);
        //ModuleLog.Debug("player_id:" + player_id + " SavePlayer JSON: " + json);

        //FileStream fs = File.Create(PathTool.GetPlayerSavePath(player_id));
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        //fs.Write(bytes, 0, bytes.Length);
        //fs.Close();

        string path = PathTool.GetPlayerSavePath(player_id);
        FileStream file = File.Create(path);
        player_struct.WriteTo(file);
        file.Close();
        Log.Debug("Save To:" + path);
    }

    public static PlayerStruct LoadPlayer(int player_id)
    {
        string path = PathTool.GetPlayerSavePath(player_id);
        if (PathTool.IsExistFile(path))
        {
            //FileStream fs = File.Open(path, FileMode.Open);
            //BinaryFormatter bf = new BinaryFormatter();
            //PlayerStruct player_struct = (PlayerStruct)bf.Deserialize(fs);
            //file.Close();


            //FileStream fs = File.Open(path, FileMode.Open);
            //StreamReader sr = new StreamReader(fs);
            //string json = sr.ReadToEnd();
            //sr.Close();
            //fs.Close();
            //ModuleLog.Debug("player_id:" + player_id + " LoadPlayer JSON: " + json);

            //PlayerStruct player_struct = LoadFromJSON<PlayerStruct>(json);


            FileStream file = File.Open(path, FileMode.Open);
            PlayerStruct player_struct = PlayerStruct.Parser.ParseFrom(file);
            file.Close();
            Log.Debug("Load From:" + path);

            return player_struct;
        }
        else
        {
            return null;
        }
    }
}

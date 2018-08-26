using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class PathTool {
    private static string RootPath;
    private static string StreamPath;
    private static string DataPath;

    //全局数据
    private static string GlobalSavePath = "Data/Global";

    //地图数据
    private static string MapSavePath = "Data/Map";

    //账号数据
    private static string AccountSavePath = "Data/Account";

    //用户数据
    private static string UserSavePath = "Data/User";

    //玩家数据
    private static string PlayerSavePath = "Data/Player";

    public static void SetRootPath(string path)
    {
        RootPath = path;
        Log.Debug("SetRootPath:" + path);
    }

    public static void SetStreamPath(string path)
    {
        StreamPath = path;
        Log.Debug("SetStreamPath:" + path);
    }

    public static void SetDataPath(string path)
    {
        DataPath = path;
        Log.Debug("SetDataPath:" + path);
    }

    public static string GetServerConfigPath(string config_name)
    {
        return string.Format("{0}/Global/ServerConfig/{1}.json", RootPath, config_name);
    }

    public static string GetOriginMapPath(string map_name = null)
    {
        if(map_name != null)
        {
            return string.Format("{0}/Map/{1}.Data", StreamPath, map_name);
        }
        else
        {
            return StreamPath + "/Map/";
        }
    }

    public static string GetMapSavePath(string map_name)
    {
        string path = string.Format("{0}/{1}/{2}.Data", DataPath, MapSavePath, map_name);
        return InitPath(path);
    }

    public static string GetLServerSavePath()
    {
        string path = string.Format("{0}/{1}/LServer.Data", DataPath, GlobalSavePath);
        return InitPath(path);
    }

    public static string GetCommonSavePath()
    {
        string path = string.Format("{0}/{1}/Common.Data", DataPath, GlobalSavePath);
        return InitPath(path);
    }

    public static string GetAccountSavePath(string account)
    {
        string path = string.Format("{0}/{1}/{2}.Data", DataPath, AccountSavePath, account);
        return InitPath(path);
    }

    public static string GeUserSavePath(int user_id)
    {
        string path = string.Format("{0}/{1}/{2}.Data", DataPath, UserSavePath, user_id);
        return InitPath(path);
    }

    public static string GetPlayerSavePath(int player_id)
    {
        string path = string.Format("{0}/{1}/{2}.Data", DataPath, PlayerSavePath, player_id);
        return InitPath(path);
    }

    public static string InitPath(string file_path)
    {
        string path = file_path.Substring(0, file_path.LastIndexOf("/"));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Log.Debug("Create Path:" + path);
        }
        return file_path;
    }

    public static bool IsExistFile(string file_path)
    {
        string path = file_path.Substring(0, file_path.LastIndexOf("/"));
        return Directory.Exists(path) && File.Exists(file_path);
    }
}

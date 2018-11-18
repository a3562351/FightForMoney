using log4net;
using System;
using System.Collections.Generic;
using System.IO;

public class Encrypt
{
    public static byte[] DefaultKey = new byte[] { 6, 3, 2, 4 };

    public static byte[] Encode(byte[] bytes, byte[] key)
    {
        byte[] encode_bytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            encode_bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
        }
        return encode_bytes;
    }

    public static byte[] Decode(byte[] bytes, byte[] key)
    {
        byte[] decode_bytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            decode_bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
        }
        return decode_bytes;
    }
}

public class Global
{
    public static int GetCurTime()
    {
        return (int)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public static string ParseFileToJson(string path)
    {
        FileStream fs = File.Open(path, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string json = sr.ReadToEnd();
        sr.Close();
        fs.Close();

        return json;
    }

    public static bool ParseItemInfo(List<int> item_info, ref int item_id, ref int count, ref int bind, ref int addition)
    {
        if(item_info.Count != 4)
        {
            return false;
        }

        item_id = item_info[1];
        count = item_info[2];
        bind = item_info[3];
        addition = item_info[4];
        return true;
    }
}

public class Log
{
    private static ILog Logger = LogManager.GetLogger("");

    /// <summary>
    /// 日志打印
    /// </summary>
    public static void Info(string file_name, string log)
    {
        Logger.Info(file_name + "|" + log);
    }

    public static void InfoFormat(string str, params object[] str_list)
    {
        Logger.InfoFormat(str, str_list);
    }

    /// <summary>
    /// 警告打印
    /// </summary>
    public static void Warn(params object[] str_list)
    {
        string str = MergeStr(str_list);
        Logger.Warn(str);
    }

    public static void WarnFormat(string str, params object[] str_list)
    {
        Logger.WarnFormat(str, str_list);
    }

    /// <summary>
    /// 错误打印
    /// </summary>
    public static void Error(params object[] str_list)
    {
        string str = MergeStr(str_list);
        Logger.Error(str);
    }

    public static void ErrorFormat(string str, params object[] str_list)
    {
        Logger.ErrorFormat(str, str_list);
    }

    /// <summary>
    /// 调试打印
    /// </summary>
    public static void Debug(params object[] str_list)
    {
        string str = MergeStr(str_list);
        Logger.Debug(str);
    }

    public static void DebugFormat(string str, params object[] str_list)
    {
        Logger.DebugFormat(str, str_list);
    }

    private static string MergeStr(params object[] str_list)
    {
        string str = "";
        for (int i = 0; i < str_list.Length; i++)
        {
            str = str + str_list[i].ToString();
            if (i < str_list.Length - 1)
            {
                str += "|";
            }
        }
        return str;
    }
}

public class ThreeSpace
{
    public static float GetDis(Pos start, Pos end)
    {
        return (float)Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2) + Math.Pow(start.Z - end.Z, 2));
    }

    public static Pos MoveDis(Pos start, Face face, float dis)
    {
        Pos end = new Pos();
        end.X = start.X + face.X * dis;
        end.Y = start.Y + face.Y * dis;
        end.Z = start.Z + face.Z * dis;
        return end;
    }

    public static Face GetFace(Pos start, Pos end)
    {
        float dis = ThreeSpace.GetDis(start, end);
        Face face = new Face();
        face.X = (end.X - start.X) / dis;
        face.Y = (end.Y - start.Y) / dis;
        face.Z = (end.Z - start.Z) / dis;
        return face;
    }
}



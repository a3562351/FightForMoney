//游戏内文字编号从1开始，配置内文字编号从100001开始
class LangManager
{
    public static string GetStr(int str_code)
    {
        Config config = ConfigPool.Load("Lang_" + SettingManager.GetLangCodeStr());
        if (config == null)
        {
            config = ConfigPool.Load("Lang_" + SettingManager.GetDefaultLangCodeStr());
        }
        ConfigItem config_item = config[str_code];
        if(config_item == null)
        {
            return "";
        }
        return config_item["Value"].ToString();
    }
}

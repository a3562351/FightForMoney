using Common.Protobuf;

enum LangCode{
    CN = 1,
}

class SettingManager
{
    private static SettingData mSettingData;
    private static LangCode CurLangCode = LangCode.CN;

    public static void Init(SettingData data)
    {
        mSettingData = data;
    }

    public static SettingData Save()
    {
        return mSettingData;
    }

    public static void SetLangCode(LangCode lang_code)
    {
        CurLangCode = lang_code;
    }

    public static LangCode GetLangCode()
    {
        return CurLangCode;
    }

    public static string GetLangCodeStr()
    {
        return CurLangCode.ToString();
    }

    public static LangCode GetDefaultLangCode()
    {
        return LangCode.CN;
    }

    public static string GetDefaultLangCodeStr()
    {
        return LangCode.CN.ToString();
    }
}

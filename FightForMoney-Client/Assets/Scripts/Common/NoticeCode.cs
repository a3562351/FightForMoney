using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NoticeCode
{
    public const int NotExistAccount = -1;
    public const int ExistAccount = -2;
    public const int NotExistPlayer = -3;
    public const int RepeatLogin = -4;
    public const int BeRepeatLogin = -5;

    public const int LoginSucc = 1;

    private static Dictionary<int, string> NoticeMap = new Dictionary<int, string>() {
        { NotExistAccount, "账号不存在"},
        { ExistAccount, "账号已存在"},
        { NotExistPlayer, "角色不存在"},
        { RepeatLogin, "重复登陆"},
        { BeRepeatLogin, "被顶号{0}"},
        { LoginSucc, "登陆成功"},
    };

    public static string GetStr(int code)
    {
        if (!NoticeMap.ContainsKey(code)) {
            return "未定义提示码:" + code;
        }
        return NoticeMap[code];
    }
}

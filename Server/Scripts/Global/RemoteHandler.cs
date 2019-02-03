using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

class RemoteId
{
    public const int LS_LoadPlayer = 1;         //登陆服通知场景服加载玩家
    public const int SS_CheckChangeScene = 2;   //检测是否可切场景
}

class RemoteHandler
{
    public virtual JObject Handle(int remote_id, JObject data)
    {
        return null;
    }
}

using Common.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ItemCtrl : CtrlBase
{
    public static ItemCtrl Instance = null;
    private ItemData item_data = new ItemData();

    public static ItemCtrl GetInstance()
    {
        if (Instance == null)
        {
            Instance = new ItemCtrl();
        }
        return Instance;
    }

    public override void Init()
    {
        base.Init();
        ClientSocket.GetInstance().AddHandler(typeof(SCItemData), this.SCItemData);
    }

    private void SCItemData(object data)
    {
        SCItemData protocol = data as SCItemData;
        this.item_data = protocol.ItemData;
    }
}

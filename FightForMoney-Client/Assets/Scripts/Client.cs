using UnityEngine;

class Client {
    public static Client instance = null;

    public static Client GetInstance()
    {
        if (instance == null)
        {
            instance = new Client();
        }
        return instance;
    }

    public void Init()
    {
        PlayerCtrl.GetInstance().Init();
        ClientSocket.GetInstance().Init();
        Debug.Log("Client Init");
    }

    public void Update(int dt)
    {
        ClientSocket.GetInstance().DispatchProtocol();
    }

    public void Release()
    {
        ClientSocket.GetInstance().Release();
        Debug.Log("Client Release");
    }
}

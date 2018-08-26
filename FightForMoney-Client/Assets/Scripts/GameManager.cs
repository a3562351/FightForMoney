using System;
using UnityEngine;

class GameManager : MonoBehaviour {
    private static GameManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        Client.GetInstance().Update(20);
    }

    private void OnApplicationQuit()
    {
        Client.GetInstance().Release();
    }

    public static void StartGame()
    {
        Client.GetInstance().Init();
    }

    public static void StartOnlineGame()
    {
        Client.GetInstance().Init();
    }
}

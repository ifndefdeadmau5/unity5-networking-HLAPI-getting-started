using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


[RequireComponent(typeof(MyNetManager))]
public class MyNetworkDiscovery : NetworkDiscovery
{

    public MyNetManager netManager;

    /* Added by projaguar */

    void Awake()
    {
        // Modified and dded by projaguar
       

        
    }
    
    void Start( )
    {
         netManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
         Initialize();
    }
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {

        //서버로부터 브로드캐스트 메시지를 받았을 때 실행됩니다.
        base.OnReceivedBroadcast(fromAddress, data);

        Debug.Log(fromAddress + "/" + data);
        if (!netManager.isConnected())
        {
            netManager.ConnectToServer(fromAddress);
            Debug.Log("연결중이 아니므로 접속합니다");
        }
        else
        {
            // ncInterface.SendNCEvent("RCV_BROADCAST", data);
        }

    }
}

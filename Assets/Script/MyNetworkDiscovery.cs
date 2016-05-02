using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


[RequireComponent(typeof(MyNetManager))]
public class MyNetworkDiscovery : NetworkDiscovery
{

    public MyNetManager netManager;
    void Awake()
    {
               
    }
    
    void Start( )
    {
         netManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
         Initialize();
    }
    
    /// <summary>
    /// Called when the broadcast message has received.
    /// </summary>
    /// <param name="fromAddress">An IP address of the broadcast message sender</param>
    /// <param name="data">Custom data with broadcast message</param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        Debug.Log(fromAddress + "/" + data);
        if (!netManager.isConnected())
        {
            netManager.ConnectToServer(fromAddress);
        }
    }
}

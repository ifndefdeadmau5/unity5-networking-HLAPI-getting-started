using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetManager : NetworkManager
{
    NetworkClient myClient;
    public string address;
    public string uid;
    public NetworkDiscovery discovery;
    public ListManager listManager;
    PlayerManager playerManager;

    void Start()
    {
        address = null;
		uid = SystemInfo.deviceUniqueIdentifier;
        playerManager = PlayerManager.Instance;
        myClient = new NetworkClient();
        if( "client" == SceneManager.GetActiveScene().name )
        {
            SetupClient( );
        } 
    }
    
    /** Declare NetworkMessage class **/
    public class UidMessage : MessageBase
    {
        public string uid;
    };
    
    public class CustomMessage : MessageBase
    {
        public string text;
    };

    public class MyMsgType
    {
        // Unique device id
        public static short UID = MsgType.Highest + 1;
        // Your custom message type
        public static short Custom = MsgType.Highest + 2;
    };

    public bool isConnected()
    {
        return myClient.isConnected;
    }

    public void sendUid()
    {
        UidMessage msg = new UidMessage();
        msg.uid = uid;

        myClient.Send(MyMsgType.UID, msg);
    }

    public void OnConnected(NetworkMessage netMsg)
    {
        if (NetworkServer.active)
        {
            
            Debug.Log("Connected address:" + netMsg.conn.address);
        }
        else
        {
            Debug.Log("Connected :" + netMsg.conn.address);
            sendUid();
            Debug.Log(myClient.GetRTT());
            discovery.StopBroadcast( );
        }
    }

    public void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Disconnected :" + netMsg.conn.address);

        if (!discovery.running)
        {
            myClient.ReconnectToNewHost(address, 4444);
        }

        if (NetworkServer.active)
        {
            Debug.Log("Disconnected :" + netMsg.conn.connectionId);
            string disconnectedUid = playerManager.getPlayerUid(netMsg.conn.connectionId);
            if (null != disconnectedUid)
            {
                listManager.displayConnectionState( disconnectedUid, false );
            }

            playerManager.setPlayerOffline(netMsg.conn.connectionId);
        }
    }

    //// 접속 후 기기 고유값을 받았을 때, 유저 추가
    public void OnUID(NetworkMessage netMsg)
    {
        UidMessage msg = netMsg.ReadMessage<UidMessage>();
        Debug.Log("OnUID " + msg.uid);
        
        // If UID already exsist, do not add new player
        if (playerManager.isExsistUID(msg.uid))
        {
            playerManager.setPlayerConnId(netMsg.conn.connectionId, msg.uid);// connID 새로 부여
            string connectedUid = playerManager.getPlayerUid(netMsg.conn.connectionId);
            listManager.displayConnectionState( connectedUid, true );
        }
        else
        {
            // New connection
            playerManager.addPlayer(netMsg.conn.connectionId, msg.uid); //
            listManager.addItem(msg.uid);
        }
    }

    public void OnCustomMessage(NetworkMessage netMsg)
    {
        CustomMessage msg = netMsg.ReadMessage<CustomMessage>();
        Debug.Log("OnCustomMessage" + msg.text );
    }
    // 모든 클라이언트 팅구기
    public void kickAllClient()
    {
        NetworkServer.DisconnectAll();
    }

    /* 사용자 정의 함수 */
    public void sendMessageToClient( string uid )
    {
         CustomMessage msg = new CustomMessage();
        int connID = -1;
        msg.text = "Hello";
      
        PlayerManager.playerData Data = playerManager.getPlayerByUid(uid);
        connID = Data.ConnID;
        
        Debug.Log( connID + ", " + msg.text );
        NetworkServer.SendToClient(connID, MyMsgType.Custom, msg);
    }
    
    public void SetupServer()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("SetupServer( )");
            ConnectionConfig config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(config, 1000);

            NetworkServer.Listen(4444);
            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetworkServer.RegisterHandler(MyMsgType.UID, OnUID);
        }
        
        if (!discovery.running)
        {
            discovery.Initialize();
            discovery.StartAsServer();
        }
    }


    public void SetupClient()
    {
        Debug.Log("SetupClient()");
        
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.Unreliable);
        myClient.Configure(config, 1000);

        discovery.Initialize();
        discovery.StartAsClient();

        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        myClient.RegisterHandler(MyMsgType.Custom, OnCustomMessage);   
    }

    public void ConnectToServer(string givenAddress)
    {
        if (null == address) { address = givenAddress; }
        myClient.Connect(givenAddress, 4444);
    }

    public void DisableServer()
    {
        Debug.Log("StopServer");
        if (NetworkServer.active)
        {
            Debug.Log("서버 가동중이므로 닫습니다");
            NetworkServer.UnregisterHandler(MsgType.Connect);
            NetworkServer.UnregisterHandler(MsgType.Disconnect);
            NetworkServer.UnregisterHandler(MyMsgType.UID);

            NetworkServer.Shutdown();
        }

        if (discovery.running)
        {
            Debug.Log("브로드캐스팅 중이므로 중지합니다.");

            discovery.StopBroadcast();
        }
    }

}

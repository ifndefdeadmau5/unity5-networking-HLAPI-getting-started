using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MyNetManager : NetworkManager
{
    NetworkClient myClient;
    public string address;
    // public UILabel ConnectedLabel;
    public string uid;
    public NetworkDiscovery discovery;
    public ListManager listManager;
    /** 
    네트워크 메시지 클래스 정의
    클라이언트 쪽과 클래스 명, 변수명을 정확히 일치시켜야 합니다.
    **/

    void Start()
    {
        address = null;
		uid = SystemInfo.deviceUniqueIdentifier;
    }
    public class UidMessage : MessageBase
    {
        public string uid;
    };

    public class MyMsgType
    {
        // unique device id
        public static short UID = MsgType.Highest + 3;
    };

    public bool isConnected()
    {
        return myClient.isConnected;
    }

    public void SendUID()
    {
        UidMessage msg = new UidMessage();
        msg.uid = uid;


        myClient.Send(MyMsgType.UID, msg);
    }

    public void OnConnected(NetworkMessage netMsg)
    {
        if (NetworkServer.active)
        {
            PlayerManager playerManager = PlayerManager.Instance;
            Debug.Log("Connected address:" + netMsg.conn.address);
        }
        else
        {
            Debug.Log("Connected :" + netMsg.conn.address);
            SendUID();
            // ConnectedLabel.text = "Connected ^.^";
            Debug.Log(myClient.GetRTT());
        }


    }

    public void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Disconnected :" + netMsg.conn.address);
        // ConnectedLabel.text = "Disconnected T.T";


        Debug.Log("호스트 제거" + netMsg.conn.hostId);

        //NetworkTransport.RemoveHost (netMsg.conn.hostId);
        // 브로드캐스팅 실행 중이 아니면 ( 검사 진행 중이라면 )
        if (!discovery.running)
        {
            myClient.ReconnectToNewHost(address, 4444);
        }

        if (NetworkServer.active)
        {
            Debug.Log("Disconnected :" + netMsg.conn.connectionId);
            PlayerManager playerManager = PlayerManager.Instance;

            string disconnectedUid = playerManager.getPlayerUid(netMsg.conn.connectionId);
            if (null != disconnectedUid)
            {
                listManager.displayConnectionState( disconnectedUid, false );
            }
            else
            {
                Debug.Log("disconnectedUid is NULL");
            }

            playerManager.setPlayerOffline(netMsg.conn.connectionId);
        }
    }

    //// 접속 후 기기 고유값을 받았을 때, 유저 추가
    public void OnUID(NetworkMessage netMsg)
    {
        UidMessage msg = netMsg.ReadMessage<UidMessage>();
        Debug.Log("OnUID " + msg.uid);


        PlayerManager playerManager = PlayerManager.Instance;

        // If UID is already exsist, do not add new player
        // 재접속 했을 경우
        if (playerManager.isExsistUID(msg.uid))
        {
            playerManager.setPlayerConnId(netMsg.conn.connectionId, msg.uid);// connID 새로 부여
            string connectedUid = playerManager.getPlayerUid(netMsg.conn.connectionId);
            listManager.displayConnectionState( connectedUid, true );
        }
        else
        {
            // 신규 접속
            playerManager.addPlayer(netMsg.conn.connectionId, msg.uid); //
            listManager.addItem(msg.uid);

        }
    }


    // 모든 클라이언트 팅구기
    public void kickAllClient()
    {
        NetworkServer.DisconnectAll();
    }

    /* 아래는 NeteworkManager 에서 상속된 메서드 */
    public override void OnStartHost()
    {
        Debug.Log("OnStartHost( )");
    }

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("OnStartClient( )");

        discovery.showGUI = false;
    }

    public override void OnStopClient()
    {
        Debug.Log("OnStopClient( )");
        discovery.StopBroadcast();
        discovery.showGUI = true;
    }

    //   /* 사용자 정의 함수 */
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
        else
        {
            //    NGUIDebug.Log("already started");
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
        myClient = new NetworkClient();
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.Unreliable);
        myClient.Configure(config, 1000);

        discovery.Initialize();
        discovery.StartAsClient();

        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
    }

    public void ConnectToServer(string givenAddress)
    {
        if (null == address) { address = givenAddress; }
        myClient.Connect(givenAddress, 4444);
    }

    public void DisableServer()
    {
        PlayerManager playerManager = PlayerManager.Instance;

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

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The custom networkmanager derived from 'NetworkManager'.
/// you can override virtual function to implement customize behaviour.
/// </summary>
public class MyNetManager : NetworkManager
{
    NetworkClient myClient;
    public string address;
    public string uid;
    public NetworkDiscovery discovery;
    public ListManager listManager;
    PlayerManager playerManager;
    public Text receivedText;
    public InputField inputfield;

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

   /** Callback functions **/
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected :" + netMsg.conn.address);
        
        // if started as client
        if (!NetworkServer.active)
        {
            sendUid();
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
            string disconnectedUid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            if (null != disconnectedUid)
            {
                listManager.displayConnectionState( disconnectedUid, false );
            }

            playerManager.setPlayerOffline(netMsg.conn.connectionId);
        }
    }

    /// <summary>
    /// Called when UidMessage has received.
    /// </summary>
    /// <param name="netMsg">A network message object</param>
    public void OnUID(NetworkMessage netMsg)
    {
        UidMessage msg = netMsg.ReadMessage<UidMessage>();
        Debug.Log("OnUID " + msg.uid);
        
        // If UID already exsist, do not add new player
        if (playerManager.isExsistUID(msg.uid))
        {
            // Assign new connection id for re-connected client.
            playerManager.setPlayerConnId(netMsg.conn.connectionId, msg.uid);
            string connectedUid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            listManager.displayConnectionState( connectedUid, true );
        }
        else
        {
            // New connection
            playerManager.addPlayer(netMsg.conn.connectionId, msg.uid); //
            listManager.addItem(msg.uid);
        }
    }

    /// <summary>
    /// Called when custom network message has received.
    /// </summary>
    /// <param name="netMsg">A network message object</param>
    public void OnCustomMessage(NetworkMessage netMsg)
    {
        CustomMessage msg = netMsg.ReadMessage<CustomMessage>();
     
        
        if( NetworkServer.active )
        {
            string uid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            uid = uid.Substring(0, 10);
            receivedText.text += msg.text = "[" + uid + "...]:" + msg.text + "\n";

            NetworkServer.SendToAll( MyMsgType.Custom, msg );
        }
        else
        {
            receivedText.text += msg.text;
        }
        Debug.Log("OnCustomMessage : " + msg.text );
    }
    
    /// <summary>
    /// Check if client is currently connected.
    /// </summary>
    /// <returns>returns true if client is connected.</returns>
    public bool isConnected()
    {
        return myClient.isConnected;
    }

    /// <summary>
    /// Send unique device identifier to server to identify this client.
    /// </summary>
    public void sendUid()
    {
        UidMessage msg = new UidMessage();
        msg.uid = uid;

        myClient.Send(MyMsgType.UID, msg);
    }


    /// <summary>
    /// Send raw network message to client
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    public void sendMessageToClient( string uid )
    {
         CustomMessage msg = new CustomMessage();
        int connID = -1;
        msg.text = "Hello";
      
        PlayerManager.playerData Data = playerManager.getPlayerByUid(uid);
        connID = Data.ConnID;
        
        NetworkServer.SendToClient(connID, MyMsgType.Custom, msg);
    }
    
    /// <summary>
    /// Send raw network message to server
    /// </summary>
    /// <param name="text">A message text will sended.</param>
    public void sendMessageToServer( )
    {
        CustomMessage msg = new CustomMessage();
        msg.text = inputfield.text;
        inputfield.text = "";


        myClient.Send(MyMsgType.Custom, msg);
    }
    
    /// <summary>
    /// Start as server and if discovery not running, start broadcast.
    /// </summary>
    public void SetupServer( )
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
            NetworkServer.RegisterHandler(MyMsgType.Custom, OnCustomMessage);   
        }
        
        if (!discovery.running)
        {
            discovery.Initialize();
            discovery.StartAsServer();
        }
    }

    /// <summary>
    /// Start as client and if discovery not running, start broadcast receive mode.
    /// </summary>
    public void SetupClient( )
    {
        Debug.Log("SetupClient()");
        
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.Unreliable);
        myClient.Configure(config, 1000);

        discovery.Initialize();
        discovery.StartAsClient();

        // Register message event handler
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        myClient.RegisterHandler(MyMsgType.Custom, OnCustomMessage);   
    }

    /// <summary>
    /// Connect to server with IP address.
    /// </summary>
    /// <param name="givenAddress">An IP address trying to connect</param>
    public void ConnectToServer(string givenAddress)
    {
        if (null == address) { address = givenAddress; }
        myClient.Connect(givenAddress, 4444);
    }

    /// <summary>
    /// Initialize NetworkServer object and Stop listening on port.
    /// </summary>
    public void DisableServer()
    {
        Debug.Log("StopServer");
        if (NetworkServer.active)
        {

            NetworkServer.ClearHandlers();
            NetworkServer.Reset();
            NetworkServer.Shutdown();
        }

        if (discovery.running)
        {
            discovery.StopBroadcast();
        }
    }

}

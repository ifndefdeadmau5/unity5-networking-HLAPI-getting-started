using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;


/// <summary>
/// The class that manage connected client's data.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    /** Singleton of this object **/
    static PlayerManager current = null;

    /** GameObject for create new object **/
    static GameObject container = null;

    public class playerData
    {
        public int ConnID;
        public string uid;


        public playerData(int connId, string uid)
        {
            ConnID = connId;
            this.uid = uid;
        }
    }

    public List<playerData> playerList = new List<playerData>();

    /// <summary>
    /// Create singleton object
    /// </summary>
    /// <returns>A singleton object.</returns>
    public static PlayerManager Instance
    {
        get
        {
            if (current == null)
            {
                container = new GameObject();
                container.name = "PlayerManager";
                current = container.AddComponent(typeof(PlayerManager)) as PlayerManager;
            }
            return current;
        }
    }

    /// <summary>
    /// Add new player to playerlist
    /// </summary>
    /// <param name="connId">Unique identifier for this connection.</param>
    /// <param name="uid">A unique device identifier.</param>
    public void addPlayer(int connId, string uid)
    {
        playerList.Add(new playerData(connId, uid));
        Debug.Log("PlayerAdded");
    }
    
    /// <summary>
    ///  Change connId to '9999(offline)' when client has disconnected
    /// </summary>
    /// <param name="connId">Unique identifier for this connection.</param>
    public void setPlayerOffline(int connId)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (connId == playerList[i].ConnID)
            {
                playerList[i].ConnID = 9999;
            }
        }
    }

    // 연결시 playerList 에서 uid를 조회하여 새로운 connId를 부여합니다.
    /// <summary>
    /// Assign new connection id for reconnected client.
    /// </summary>
    /// <param name="connId">Unique identifier for this connection.</param>
    /// <param name="uid">A unique device identifier.</param>
    public void setPlayerConnId(int connId, string uid)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (uid == playerList[i].uid)
                playerList[i].ConnID = connId;
        }
    }

    /// <summary>
    /// returns true if uid already exsist.
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    /// <returns></returns>
    public bool isExsistUID(string uid)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (uid == playerList[i].uid)
            {
                return true;
            }
        }

        return false;
    
    }
    
    /// <summary>
    /// Get player's device Unique Identifier(uid) with connection id
    /// </summary>
    /// <param name="connID">The Unique identifier for this connection</param>
    /// <returns>A unique device identifier.</returns>
    public string getPlayerUidByConnID(int connID)
    {

        for (int i = 0; i < playerList.Count; i++)
        {
            if (connID == playerList[i].ConnID)
            {
                return playerList[i].uid;
            }
        }

        return null;
    }

    /// <summary>
    /// Get player's Data with unique device identifier.
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    /// <returns>Player's data</returns>
    public playerData getPlayerByUid(string uid)
    {

        for (int i = 0; i < playerList.Count; i++)
        {
            if (uid == playerList[i].uid)
            {
                return playerList[i];
            }
        }

        return null;
    }
}

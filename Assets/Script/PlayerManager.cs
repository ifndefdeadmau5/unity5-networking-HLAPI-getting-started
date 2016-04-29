﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

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

    /** Create singleton object **/
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

    /* Add new player to playerlist */
    public void addPlayer(int connId, string uid)
    {
        playerList.Add(new playerData(connId, uid));
        Debug.Log("PlayerAdded");
    }


    
    /// <summary>
    ///  change connId to '9999(offline)' when client has disconnected
    /// </summary>
    /// <param name="connId"></param>
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
    public void setPlayerConnId(int connId, string uid)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (uid == playerList[i].uid)
                playerList[i].ConnID = connId;
        }
    }

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

    public string getPlayerUid(int connID)
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

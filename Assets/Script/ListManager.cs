using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is used by server scene.
/// </summary>
public class ListManager : MonoBehaviour
{
    MyNetManager NetManager;
    public GameObject ItemPrefab;
    IDictionary<string, GameObject> dic = new Dictionary<string, GameObject> { };
    GameObject GetPrefab;
    
    void Awake()
    {
        NetManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
    }
    
    void Start()
    {
        // Start the server manually...
        // if you want to change the server starting point,
        // move this method to another function.
        NetManager.SetupServer( );
    }
    
    /// <summary>
    /// Add Instantiated object to layout group.
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    public void addItem(string uid)
    {
        if (!dic.ContainsKey(uid))
        {
            ItemPrefab.transform.FindChild("uid").GetComponent<Text>().text = uid;
            GameObject a = (GameObject)Instantiate(ItemPrefab);
            a.transform.SetParent(gameObject.transform, false);

            dic.Add(uid, a);
        }
    }

    /// <summary>
    /// Display current connection state with text color.
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    /// <param name="isConnected">Connection status</param>
    public void displayConnectionState(string uid, bool isConnected)
    {
        if (null == uid)
        {
            Debug.Log("displayConnectionState : uid is null");
            return;
        }
        dic.TryGetValue(uid, out GetPrefab);
        Debug.Log(uid + " " + isConnected);
        if (isConnected)
        {
            GetPrefab.transform.FindChild("uid").GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f);
        }
        else
        {
            GetPrefab.transform.FindChild("uid").GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f);
        }
    }


}
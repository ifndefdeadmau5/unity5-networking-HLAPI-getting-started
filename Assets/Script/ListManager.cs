using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ListManager : MonoBehaviour
{
    MyNetManager NetManager;
    public GameObject ItemPrefab;
    public Canvas canvas;
    IDictionary<string, GameObject> dic = new Dictionary<string, GameObject> { };
    GameObject GetPrefab;
    void Awake()
    {
        NetManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
        RectTransform panel = canvas.GetComponentInChildren<RectTransform>();
    }


    public void Init()
    {

    }

    public void addItem(string uid)
    {


        if (!dic.ContainsKey(uid))
        {
            ItemPrefab.GetComponent<Text>().text = uid;
            GameObject a = (GameObject)Instantiate(ItemPrefab);
            a.transform.SetParent(gameObject.transform, false);

            dic.Add(uid, a);
        }
    }

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
            GetPrefab.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f);
        }
        else
        {
            GetPrefab.GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f);
        }
    }


}
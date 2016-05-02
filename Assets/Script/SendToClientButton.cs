using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script will attached to button, and assign button event dynamically.
/// (Because it's prefab button)
/// </summary>
public class SendToClientButton : MonoBehaviour
{
    public Button SendMessageButton;
    MyNetManager networkManager;

    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetManager>( );
        buttonSetup(SendMessageButton);
    }
    void buttonSetup(Button button)
    {
        button.onClick.RemoveAllListeners();
        //Add your new event
        button.onClick.AddListener(() => handleButton(button));
    }

    void handleButton(Button b)
    {
        networkManager.sendMessageToClient( b.transform.parent.FindChild("uid").GetComponent<Text>().text );
    }
}
/**
        버튼에 부착되어
        이벤트를 동적으로 할당합니다.
**/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        Debug.Log("Button '" + b.name + "' pressed!");
        networkManager.sendMessageToClient( b.transform.parent.FindChild("uid").GetComponent<Text>().text );
    }
}
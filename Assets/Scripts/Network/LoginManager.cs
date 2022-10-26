using Mirror;
using PlayFab.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    [Header("Network")]
    [SerializeField] NetworkManager networkManager;
    public string IP = "127.0.0.1";
    public string PORT = "7777";
    [Header("Login Fields")]
    public TMP_InputField m_InputField;



    public void Login()
    {
        if (m_InputField.text == null || m_InputField.text == string.Empty) return;
        PlayerPrefs.SetString("Username", m_InputField.text);



        JoinServer();
        //networkManager.ServerChangeScene("GameScene");
    }

    public void JoinServer()
    {
        networkManager.networkAddress = IP;
        networkManager.StartClient();
        networkManager.autoCreatePlayer = true;
    }


}

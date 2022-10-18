using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public Transform spawn;
    private NetworkManager networkManager;
    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        NetworkServer.SpawnObjects();


    }
}

using Mirror;
using PlayFab.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public Transform spawn;
    private UnityNetworkServer mServer; 
    private void Start()
    {
        mServer = FindObjectOfType<UnityNetworkServer>();
        //NetworkServer.SpawnObjects();
    }
}

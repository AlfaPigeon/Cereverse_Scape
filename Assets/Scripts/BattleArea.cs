using Mirror;
using PlayFab.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleArea : NetworkBehaviour
{

  //  [Header("Battle Queue")]


    [Header("Teleport Locations")]
    public Transform player_1_location;
    public Transform player_2_location;
    public Transform return_location;

    [Header("Network")]
    public NetworkManager networkManager;
   

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }



    private void OnTriggerEnter(Collider other)
    {
        PlayerController _player = other.gameObject.GetComponent<PlayerController>();

        if (_player == null || !_player.isLocalPlayer) return;

        BattleController battleController = _player.GetComponent<BattleController>();

        battleController.battleArea = this;
        battleController.EnterBattleQueue();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController _player = other.gameObject.GetComponent<PlayerController>();

        if (_player == null || !_player.isLocalPlayer) return;

        BattleController battleController = _player.GetComponent<BattleController>();

        battleController.ExitBattleQueue();
    }

    
    [Server]
    private void Update()
    {

        if(BattleQueueCount() > 1)
        {
            BattleController player_1 = null;
            BattleController player_2 = null;

            foreach (BattleController battleController in FindObjectsOfType<BattleController>())
            {
                if (player_1 == null && battleController.inBattleQueue)
                {
                    player_1 = battleController;
                    player_1.inBattleQueue = false;
                } else if (player_2 == null && battleController.inBattleQueue)
                {
                    player_2 = battleController;
                    player_2.inBattleQueue = false;
                }
                else if (player_1 != null && player_2 != null) break;
            }



            StartBattle(player_1, player_2);
        }
    }

    [Server]
    private int BattleQueueCount()
    {
        int result = 0;
        foreach(BattleController battleController in FindObjectsOfType<BattleController>()){
            if (battleController.inBattleQueue) result++;
        }
        return result;
    }

    [Server]
    private void StartBattle(BattleController player_1, BattleController player_2)
    {
        Debug.Log(player_1.player.username +" is battling "+ player_2.player.username);


        player_1.transform.position = new Vector3(player_1_location.position.x, player_1.transform.position.y, player_1_location.position.z);
        player_2.transform.position = new Vector3(player_2_location.position.x, player_2.transform.position.y, player_2_location.position.z);

        player_1.transform.rotation = Quaternion.LookRotation((player_2.transform.position - player_1.transform.position).normalized);
        player_2.transform.rotation = Quaternion.LookRotation((player_1.transform.position - player_2.transform.position).normalized);

        player_1.navMeshAgent.destination = player_1.transform.position;
        player_2.navMeshAgent.destination = player_2.transform.position;


        int player1_health = 100;
        int player2_health = 100;

        List<int> player_1_damage = new List<int>();
        List<int> player_2_damage = new List<int>();


        while (player1_health > 0 && player2_health > 0)
        {
            int damage_1 = Random.Range(0, 30);
            if (damage_1 > player1_health) damage_1 = player1_health;

            int damage_2 = Random.Range(0, 30);
            if (damage_2 > player2_health) damage_2 = player2_health;

            player_1_damage.Add(damage_1);
            player_2_damage.Add(damage_2);

            player1_health -= damage_1;
            player2_health -= damage_2;
        }


        //Debug ===========================================================

        

        if(player1_health > player2_health)
        {
            Debug.Log(player_1.player.username + " Should win with "+ player1_health+":"+ player2_health+" health");
        }
        else if(player1_health < player2_health)
        {
            Debug.Log(player_2.player.username + " Should win with" + player1_health + ":" + player2_health + " health");
        }
        else
        {
            Debug.Log("This battle should be a tie");
        }
        //===================================================================

        rpcStartBattle(player_1, player_2, player_1_damage, player_2_damage);
    }

   

    [ClientRpc]
    private void rpcStartBattle(BattleController player_1, BattleController player_2, List<int> player_1_damage, List<int> player_2_damage)
    {
        player_1.StartBattle(player_2, new Queue<int>(player_1_damage));
        player_2.StartBattle(player_1, new Queue<int>(player_2_damage));
    }


}








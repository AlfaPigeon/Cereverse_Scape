using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class BattleController : NetworkBehaviour
{
    [Header("Stats")]


    [SyncVar]
    public int Health = 100;

    
    public BattleController Enemy;

    [SyncVar]
    public bool inBattle = false;


    public float AttackWaitTime=1f;

    public Queue<int> damage_queue;

    private FeedManager feedManager;
    public PlayerController player;
    private Animator animator;

    [SyncVar]
    public bool inBattleQueue = false;


    public NavMeshAgent navMeshAgent;


    public BattleArea battleArea = null;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        feedManager = GetComponent<FeedManager>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        damage_queue = new Queue<int>();
    }


    public void StartBattle(BattleController _enemy,Queue<int> _damage_queue)
    {
        if (_enemy == this || inBattle) return;

        Enemy = _enemy;

        damage_queue = new Queue<int>(_damage_queue);

        if (isLocalPlayer) cmdStartBattle();
    }




    private IEnumerator Hit(float _waittime)
    {
        while(Enemy != null && damage_queue.Count > 0) {

            animator.SetBool("Hit", true);
        
            yield return new WaitForSeconds(_waittime);

       

            GetHit();

            animator.SetBool("Hit", false);

        }
        yield return new WaitForSeconds(_waittime);
        if (isLocalPlayer)cmdEndBattle();
    }




    public void GetHit()
    {
        

        if (isLocalPlayer)
        {
            int damage = damage_queue.Dequeue();
            feedManager.PostLocalMessage("Took " + damage + " damage", Color.red);
            cmdSetHealth(Health - damage);
            feedManager.PostLocalMessage(Health-damage + "/100 health remaining", Color.green);
        }
       
    }







    public void EnterBattleQueue()
    {
        if (isLocalPlayer) cmdEnterBattleQueue();
    }



    public void ExitBattleQueue()
    {
        if (isLocalPlayer) cmdExitBattleQueue();
    }

    //Server commands

    [Command]
    private void cmdEnterBattleQueue()
    {
        inBattleQueue = true;
       
    }


    [Command]
    private void cmdExitBattleQueue()
    {
        inBattleQueue = false;
      
    }

    [Command]
    private void cmdStartBattle()
    {
        Health = 100;
        inBattle = true;

        rpcStartBattle();
    }
    [Command]
    private void cmdEndBattle()
    {
        inBattle = false;

        //transform.position = new Vector3(battleArea.return_location.position.x, transform.position.y, battleArea.return_location.position.z);

        navMeshAgent.destination = transform.position;

        rpcEndBattle();
       
    }

    [Command]
    private void cmdSetHealth(int _health)
    {
        Health = _health;
        //rpcSetHealthCallBack();
    }

    //Client rpc's

    [ClientRpc]

    private void rpcSetHealthCallBack()
    {
        if(isLocalPlayer) feedManager.PostLocalMessage(Health + "/100 health remaining", Color.green);
    }


    [ClientRpc]

    private void rpcStartBattle()
    {
        animator.SetBool("Battle", true);
        StartCoroutine(Hit(AttackWaitTime));
    }



    [ClientRpc]
    public void rpcEndBattle()
    {


        if (isLocalPlayer) {

            feedManager.PostLocalMessage("===========", Color.blue);

            if (Health > Enemy.Health)
            {
                feedManager.PostLocalMessage("You win your battle with "+Enemy.GetComponent<PlayerController>().username, Color.green);
            }
            else if (Health < Enemy.Health)
            {
                feedManager.PostLocalMessage("You lose your battle with " + Enemy.GetComponent<PlayerController>().username, Color.red);
            }
            else if(Enemy.Health == Health)
            {
                feedManager.PostLocalMessage("You had a tie in battle with" + Enemy.GetComponent<PlayerController>().username, Color.yellow);
            }

            feedManager.PostLocalMessage("===========", Color.blue);

            animator.SetBool("Battle", false);



            
            
        }
       
    
    }



}

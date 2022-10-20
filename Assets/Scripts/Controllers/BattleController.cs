using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BattleController : NetworkBehaviour
{
    [Header("Stats")]
    [SyncVar]
    public float Health = 100f;
    private BattleController Enemy;
    public bool inBattle = false;


    public float AttackWaitTime=0.5f;

    [SyncVar]
    public bool attacking = false;


    private FeedManager feedManager;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        feedManager = GetComponent<FeedManager>();
    }

    public void StartBattle(BattleController _enemy)
    {
        if(_enemy == this || inBattle)return;
        inBattle = true;
        Enemy = _enemy;

        StartCoroutine(Hit(AttackWaitTime));
    }



    private IEnumerator Hit(float _waittime)
    {
        if (Enemy == null || !isLocalPlayer || !inBattle) yield return new WaitForSeconds(0);
        cmdTakeDamage();

        yield return new WaitForSeconds(_waittime);

        while(attacking || Enemy.attacking) yield return new WaitForSeconds(0.1f);

        StartCoroutine(Hit(AttackWaitTime));    
    }

    public void LeaveBattle()
    { 
        inBattle = false;
        Enemy = null;
    }






    [Command]
    private void cmdTakeDamage()
    {
        //Server Checks

        attacking = true;


        int damage = Random.Range(0, 24);
        if (damage > Health) Health = 0;
        else Health -= damage;

        //Rpc Hit animation here -->

        if(Health <= 0)
        {
            rpcEndBattle();
        }


        attacking = false;
    }
    [Command]
    private void cmdResetHealth()
    {
        //Server Checks

        Health = 100f;
    }

    [Command]
    private void cmdStartBattle(BattleController _enemy)
    {
        //Server Checks
        rpcStartBattle(_enemy);
    }


    [ClientRpc]
    private void rpcEndBattle()
    {

        if(Health > Enemy.Health)
        {
            feedManager.PostLocalMessage("You win your battle with "+Enemy.GetComponent<PlayerController>().username, Color.green);
        }
        else if (Health < Enemy.Health)
        {
            feedManager.PostLocalMessage("You lose your battle with " + Enemy.GetComponent<PlayerController>().username, Color.red);
        }
        else
        {
            feedManager.PostLocalMessage("You had a tie in battle with" + Enemy.GetComponent<PlayerController>().username, Color.yellow);
        }


        LeaveBattle();
    }


    [ClientRpc]
    private void rpcStartBattle(BattleController _enemy)
    {
        if (_enemy == this || inBattle) return;
        inBattle = true;
        Enemy = _enemy;

        StartCoroutine(Hit(AttackWaitTime));
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : NetworkBehaviour
{
    [Header("Stats")]
    [Range(0f,100f)]
    public float Health = 100f;
    [Range(0f, 100f)]
    public float Damage = 0f;


    private BattleController Enemy;
    public bool inBattle = false;

    private int GetRandomInt()
    {
        return Random.RandomRange(0, 24);
    }

    private void Start()
    {
        
    }


    public void StartBattle(BattleController _enemy)
    {
        if(_enemy == this || inBattle)return;
        inBattle = true;
        Enemy = _enemy;
    }

    public void LeaveBattle()
    { 
        inBattle = false;
        Enemy = null;
    }

}

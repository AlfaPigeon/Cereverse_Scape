using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerController : NetworkBehaviour
{
    [SyncVar(hook=nameof(SetUsername))]
    public string username = "";

    public Transform SpeechPoint;
    public TMP_Text NamePlate;
    private NavMeshAgent _agent;
    private PlayerInputs _input;
    private Animator _animator;
    private ChatManager _ChatManager;
    public ChatBubbleSpawner bubbleSpawner;
    public FeedManager feedManager;

    private BattleController BattleController;



    void Start()
    {

        Debug.Log("Player spawned");
        
        PlayerInput playerInput = GetComponent<PlayerInput>();

        bubbleSpawner = GetComponent<ChatBubbleSpawner>();
        feedManager = GetComponent<FeedManager>();
        _input = GetComponent<PlayerInputs>();

        BattleController = GetComponent<BattleController>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        if (isLocalPlayer)
        {

            cmdSetUsername(PlayerPrefs.GetString("Username"));

            CinemachineVirtualCamera _camera = FindObjectOfType<CinemachineVirtualCamera>();
            _camera.Follow = transform;
            _camera.LookAt = transform;

            _ChatManager = FindObjectOfType<ChatManager>();
            _ChatManager.SetPlayer(this);

        }


    }

    void Update()
    {   
        if (!isLocalPlayer) return;
        
        MoveToCursor();
    }
   
    public void MoveToCursor()
    {
        if (_input.move && !BattleController.inBattle)
        {
            Ray movePosition = Camera.main.ScreenPointToRay(_input.cursor_location);
            if (Physics.Raycast(movePosition, out var hit))
            {
                cmdMoveToCursor(hit.point);
            }
        }
        _animator.SetFloat("Velocity", _agent.velocity.magnitude / _agent.speed);
    }

    private void SetUsername(string old_username, string new_username)
    {
        username = new_username;
        NamePlate.text = new_username;
    }

    public void ChangeUsername(string _username)
    {
        if (!isLocalPlayer) return;
        cmdSetUsername(_username);
    }

    //Server commands
    [Command]
    private void cmdMoveToCursor(Vector3 _destination)
    {
        //Put server validation checks here

        BattleController battleController = GetComponent<BattleController>();
        if (battleController != null && battleController.inBattle)return;

        //====


        //This is for movement
        _agent.SetDestination(_destination);

        rpcMoveToCursor(_destination);


    }
    
    
    [ClientRpc]
    private void rpcMoveToCursor(Vector3 _destination)
    {
        //This is for velocity update
        _agent.SetDestination(_destination);
    }

    [Command]
    private void cmdSetUsername(string  _username)
    {
        //Put server validation checks here
        username = _username;
        foreach (PlayerController p in FindObjectsOfType<PlayerController>())p.rpcSetUsername(p.username);
    }

    [ClientRpc]
    public void rpcSetUsername(string _username)
    {
        username = _username;
        NamePlate.text=username;
    }



}

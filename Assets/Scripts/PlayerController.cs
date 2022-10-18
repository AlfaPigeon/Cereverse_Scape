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
 
    public string username = "Player";
    public Transform SpeechPoint;
    public TMP_Text NamePlate;
    private NavMeshAgent _agent;
    private PlayerInputs _input;
    private Animator _animator;
    private ChatManager _ChatManager;
    public ChatBubbleSpawner bubbleSpawner;
    public FeedManager feedManager;
    void Start()
    {

        Debug.Log("Player Started");
        
        PlayerInput playerInput = GetComponent<PlayerInput>();

        bubbleSpawner = GetComponent<ChatBubbleSpawner>();
        feedManager = GetComponent<FeedManager>();
        _input = GetComponent<PlayerInputs>();


        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        
        NamePlate.text = username;
        
        if (!isLocalPlayer || SceneManager.GetActiveScene().name == "Login") return;

        username = PlayerPrefs.GetString("Username");
        cmdSetUsername();

        CinemachineVirtualCamera _camera = FindObjectOfType<CinemachineVirtualCamera>();
            _camera.Follow = transform;
            _camera.LookAt = transform;

        _ChatManager = FindObjectOfType<ChatManager>();
        _ChatManager.SetPlayer(this);
        

    }


   
    void Update()
    {
       
        if (!isLocalPlayer) return;
        
        MoveToCursor();
    }
   
    public void MoveToCursor()
    {

        if (_input.move)
        {
            
            Ray movePosition = Camera.main.ScreenPointToRay(_input.cursor_location);
            if (Physics.Raycast(movePosition, out var hit))
            {
                //_agent.SetDestination(hit.point);
                cmdMoveToCursor(hit.point);
            }
        }
        
        _animator.SetFloat("Velocity", _agent.velocity.magnitude / _agent.speed);

    }

    public void SetUsername(string _username)
    {
        if (!isLocalPlayer) return;
        username = _username;
        cmdSetUsername();
    }



    public override void OnStartClient()
    {
        base.OnStartClient();


    }


    //Server commands
    [Command]
    private void cmdMoveToCursor(Vector3 _destination)
    {
        //Put server validation checks here

        rpcMoveToCursor(_destination);
    }
    [Command]
    private void cmdSetUsername()
    {
        //Put server validation checks here

        rpcSetUsername(username);
    }


    //Client rpcs
    [ClientRpc]
    private void rpcMoveToCursor(Vector3 _destination)
    {
        _agent.SetDestination(_destination);
    }

    [ClientRpc]
    private void rpcSetUsername(string _username)
    {
        username = _username;
        NamePlate.text = _username;
    }



}

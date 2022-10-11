using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : NetworkBehaviour
{

    public string username = "Player";
    public Transform SpeechPoint;
    public TMP_Text NamePlate;
    private NavMeshAgent _agent;
    private PlayerInputs _input;
    private Animator _animator;



    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = true;

        _input = GetComponent<PlayerInputs>();
    }


    void Start()
    {
        
        if (!isLocalPlayer) return;
        
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        NamePlate.text = username;
        
       
        
        CinemachineVirtualCamera _camera = FindObjectOfType<CinemachineVirtualCamera>();
            _camera.Follow = transform;
            _camera.LookAt = transform;

        ChatManager _ChatManager = FindObjectOfType<ChatManager>();
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
                _agent.SetDestination(hit.point);
                Debug.Log(hit.point);
            }

           
        }
        
        _animator.SetFloat("Velocity", _agent.velocity.magnitude / _agent.speed);

    }
}

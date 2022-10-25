using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class ChatManager : NetworkBehaviour
{


    //player
    private PlayerInput playerInput;
    private PlayerInputs _input;



    [Header("Bubble Management")]
    public GameObject BubblePreset;
    public GameObject ChatText;
    public int limit = 1;
    public float timeout = 3f;


    private TMP_InputField _InputField;
    public GameObject FeedContent;
    private PlayerController player;
    public int feed_limit = 100;
    public Queue<TMP_Text> FeedContentQueue;

    void Start()
    {
        _InputField = GetComponentInChildren<TMP_InputField>();
        FeedContentQueue = new Queue<TMP_Text>();
    }
    public void SetPlayer(PlayerController _player)
    {
        player = _player;
        _input = _player.gameObject.GetComponent<PlayerInputs>();
        playerInput = _player.gameObject.GetComponent<PlayerInput>();
    }



    public void SendChat()
    {

        
        if (playerInput.currentActionMap.name != "Chat")return;
        if (!_input.send)return;
        if (_InputField.text == "") return;

        string _text = _InputField.text;
        _InputField.text = "";


        //Chat Commands
        if(_text != "" &&  _text.IndexOf("/") == 0)
        {
            ChatCommand(_text);
            return;
        }


        if (player.isLocalPlayer)
        {
            player.bubbleSpawner.SpawnBubble(_text);
            player.feedManager.PostFeed(_text); 
        }

        
    }
    
    public void ChatCommand(string _command)
    {
        string[] args = _command.Split(" ");

        if (args[0] == "/username" && args.Length >= 2)
        {
            player.SetUsername(args[1]);

            LocalMessage("Changing username to " + _command.Split(" ")[1],Color.yellow);

        }else if (args[0] == "/battle" && args.Length >= 2)
        {
            string _username = args[1];
            if(_username == player.username)
            {
                LocalMessage("Cannot send battle request to your self", Color.yellow);
                return;
            }
            PlayerController otherPlayer = FindPlayerByUsername(args[1]);
            if(otherPlayer == null)
            {
                LocalMessage("No such player", Color.red);
                return;
            }
            BattleController enemy = otherPlayer.gameObject.GetComponent<BattleController>();
            BattleController battleController = player.gameObject.GetComponent<BattleController>();
           
            LocalMessage("Battle Request sent to "+ args[1], Color.green);
        }

    }

    private PlayerController FindPlayerByUsername(string _username)
    {
        foreach(PlayerController p in FindObjectsOfType<PlayerController>())
        {
            if(p.username == _username)return p;
        }
        return null;
    }


    public void LocalMessage(string _text,Color color)
    {
        GameObject _chattext = Instantiate(ChatText, FeedContent.transform);
        TMP_Text tMP_Text = _chattext.GetComponent<TMP_Text>();


        tMP_Text.text =  _text;
        tMP_Text.color = color;

        FeedContentQueue.Enqueue(tMP_Text);

        if (FeedContentQueue.Count > feed_limit)
        {
            Destroy(FeedContentQueue.Dequeue().gameObject);
        }
    }







    public void SelectChat()
    {
        playerInput.SwitchCurrentActionMap("Chat");
    }

    public void DeselectChat()
    {
        playerInput.SwitchCurrentActionMap("Movement");
    }



}

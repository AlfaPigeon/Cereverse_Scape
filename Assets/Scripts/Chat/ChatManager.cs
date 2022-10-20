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
    private PlayerController Player;
    public int feed_limit = 100;
    public Queue<TMP_Text> FeedContentQueue;
    void Start()
    {
        _InputField = GetComponentInChildren<TMP_InputField>();
        FeedContentQueue = new Queue<TMP_Text>();
    }
    public void SetPlayer(PlayerController _player)
    {
        Player = _player;
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


        if (Player.isLocalPlayer)
        {
            Player.bubbleSpawner.SpawnBubble(_text);
            Player.feedManager.PostFeed(_text); 
        }

        
    }
    
    public void ChatCommand(string _command)
    {
        if (_command.Split(" ")[0] == "/username" && _command.Split(" ").Length >= 2)
        {
            Player.SetUsername(_command.Split(" ")[1]);

            LocalMessage("Changing username to " + _command.Split(" ")[1],Color.yellow);

        }

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

using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedManager : NetworkBehaviour
{


    private ChatManager chatManager;

    private GameObject FeedContent;
    private int feed_limit = 100;
    private GameObject ChatText;

    private PlayerController Player;
    private void Start()
    {
        chatManager = FindObjectOfType<ChatManager>();
        Player = GetComponent<PlayerController>();
        feed_limit = chatManager.feed_limit;
        FeedContent = chatManager.FeedContent;
        ChatText = chatManager.ChatText;
    }

    public void PostFeed(string _text)
    {
        if(!isLocalPlayer)return;
        cmdPostFeed(_text);
    }


    public void PostLocalMessage(string _text, Color color)
    {
        chatManager.LocalMessage(_text, color);
    }

    [Command]
    public void cmdPostFeed(string _text)
    {
        rpcPostFeed(_text);
    }
    [ClientRpc]
    public void rpcPostFeed(string _text)
    {
        GameObject _chattext = Instantiate(ChatText, FeedContent.transform);
        TMP_Text tMP_Text = _chattext.GetComponent<TMP_Text>();

        
        tMP_Text.text = Player.username + ": " + _text;

       
        chatManager.FeedContentQueue.Enqueue(tMP_Text);

        if (chatManager.FeedContentQueue.Count > feed_limit)
        {
            Destroy(chatManager.FeedContentQueue.Dequeue().gameObject);
        }
    }
}

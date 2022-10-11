using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Transform ChatPoint;
    public Queue<ChatBubble> bubbles;
    public int limit = 1;
    public float timeout = 3f;
    private TMP_InputField _InputField;

    private PlayerController Player;
    void Start()
    {
        bubbles = new Queue<ChatBubble>();
        _InputField = GetComponentInChildren<TMP_InputField>();  
    }
    public void SetPlayer(PlayerController _player)
    {
        Player = _player;
        _input = _player.gameObject.GetComponent<PlayerInputs>();
        ChatPoint = _player.transform.Find("Speech_point");
    }


    
    public void SendChat()
    {
        if (playerInput.currentActionMap.name != "Chat")return;
        if (!_input.send)return;
        if (_InputField.text == "") return;
        
        
        GameObject go_bubble = Instantiate(BubblePreset, ChatPoint);
        
        
        string _text = _InputField.text;
        _InputField.text = "";
        ChatBubble _bubble = go_bubble.GetComponent<ChatBubble>();

        MoveBubblesUp(1.5f);

        _bubble.Chat(_text);

        bubbles.Enqueue(_bubble);

        StartCoroutine(BubbleTimeout(_bubble,timeout));
    }

    private void MoveBubblesUp(float _distance)
    {
       foreach(var bubble in bubbles)
        {
            bubble.transform.localPosition = new Vector3(bubble.transform.localPosition.x, bubble.transform.localPosition.y + _distance, bubble.transform.localPosition.z);
        }

        //Bubble Limit

        if (bubbles.Count != 0 && bubbles.Count > limit -1)
            if (!bubbles.Peek().disappearing) bubbles.Dequeue().Die();
    }

    private IEnumerator BubbleTimeout(ChatBubble _bubble,float timeout)
    {
        yield return new WaitForSeconds(timeout);
        if(bubbles.Peek().Equals(_bubble)) bubbles.Dequeue();
        _bubble.Die();
    }
}

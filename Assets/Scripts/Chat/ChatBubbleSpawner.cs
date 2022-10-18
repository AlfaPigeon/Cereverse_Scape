using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubbleSpawner : NetworkBehaviour
{
    private ChatManager chatManager;

    //Bubble Management
    public GameObject BubblePreset;
    public Transform ChatPoint;
    private Queue<ChatBubble> bubbles;
    private int limit = 1;
    private float timeout = 3f;





    private void Start()
    {
        chatManager = FindObjectOfType<ChatManager>();
        BubblePreset = chatManager.BubblePreset;
        bubbles = new Queue<ChatBubble>();
        limit = chatManager.limit;
        timeout = chatManager.timeout;
    }


    public void SpawnBubble(string _text)
    {
        if(!isLocalPlayer)return;
        cmdSpawnChatBaloon(_text);
    }


    public void SpawnLocalBubble(string _text)
    {
        SpawnLocalChatBaloon(_text);
    }

    public IEnumerator BubbleTimeout(ChatBubble _bubble, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        if (bubbles.Peek().Equals(_bubble)) bubbles.Dequeue();
        _bubble.Die();
    }


    public void MoveBubblesUp(float _distance)
    {
        foreach (var bubble in bubbles)
        {
            bubble.transform.localPosition = new Vector3(bubble.transform.localPosition.x, bubble.transform.localPosition.y + _distance, bubble.transform.localPosition.z);
        }

        //Bubble Limit

        if (bubbles.Count != 0 && bubbles.Count > limit - 1)
            if (!bubbles.Peek().disappearing) bubbles.Dequeue().Die();
    }

    private void SpawnLocalChatBaloon(string _text)
    {
        GameObject go_bubble = Instantiate(BubblePreset, ChatPoint);
        ChatBubble _bubble = go_bubble.GetComponent<ChatBubble>();

        if (bubbles.Count != 0)
            MoveBubblesUp(1.5f);

        _bubble.Chat(_text);

        bubbles.Enqueue(_bubble);

        StartCoroutine(BubbleTimeout(_bubble, timeout));
    }



    //Server commands

    [Command]
    private void cmdSpawnChatBaloon(string _text)
    {
        //Server Checks here
        rpcSpawnChatBaloon(_text);
    }

    //Client rpcs


    [ClientRpc]
    private void rpcSpawnChatBaloon(string _text)
    {
        SpawnLocalChatBaloon(_text);
    }
}

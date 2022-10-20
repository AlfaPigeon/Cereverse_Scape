using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NpcScript : NetworkBehaviour
{
    public string[] Lines;
    public float SpeechWaitTime = 3f;
    private ChatBubbleSpawner chatBubbleSpawner;
    public bool talking = false;
    public int line = 0;
    void Start()
    {
        chatBubbleSpawner = GetComponent<ChatBubbleSpawner>();  
    }

    
    public void StartTalking()
    {
        if (!talking)
            StartCoroutine(Talk(SpeechWaitTime));
    }

    private IEnumerator Talk(float timeout)
    {
        talking = true;

        foreach(string s in Lines)
        {
            chatBubbleSpawner.SpawnLocalBubble(s);
            yield return new WaitForSeconds(timeout);
        }
        


        talking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        GameObject go_collision = collision.gameObject;
        PlayerController player = go_collision.GetComponent<PlayerController>();
        if (player == null || !player.isLocalPlayer)return;
        StartTalking();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        GameObject go_collision = other.gameObject;
        PlayerController player = go_collision.GetComponent<PlayerController>();
        if (player == null || !player.isLocalPlayer) return;
        StartTalking();
    }
}

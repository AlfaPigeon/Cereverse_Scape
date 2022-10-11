using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class ChatBubble : MonoBehaviour
{

    public float DisappearSpeed = 0.05f;
    public bool disappearing = false;
    public float BubbleHeight = 0f;
    private SpriteRenderer baloon;
    private TextMeshPro tm_text;

    private void Awake()
    {
        baloon = transform.Find("Chat_Baloon").GetComponent<SpriteRenderer>();
        tm_text = transform.Find("Text").GetComponent<TextMeshPro>();
    }


    public void Chat(string text)
    {
        tm_text.text = text;
        tm_text.ForceMeshUpdate();

        Vector2 textSize = tm_text.GetRenderedValues(false);
        Vector2 new_size = textSize + new Vector2(10f, 20f);
        baloon.size = new_size;

        BubbleHeight = baloon.size.y;


    }
    
    public void Die()
    {
        disappearing=true;
        StartCoroutine(Disappear(DisappearSpeed));
    }

    private IEnumerator Disappear(float timeout)
    {
        for (float i = 1f; i > 0f; i -= timeout)
        {

            //Color vanish
            Color color = baloon.color;
            color.a = i;
            baloon.color = color;
            
            color = tm_text.color;
            color.a = i;
            tm_text.color = color;

            //Translation
            
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+i, transform.localPosition.z);
            
            yield return new WaitForSeconds(timeout);
        }

        
        Destroy(gameObject);
       
    }
}

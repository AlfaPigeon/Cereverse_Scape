using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPointFallow : MonoBehaviour
{
    public Transform _speech_point;
    private void FixedUpdate()
    {
        transform.position = _speech_point.position;
    }
}

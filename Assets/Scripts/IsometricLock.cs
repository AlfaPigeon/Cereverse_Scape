using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricLock : MonoBehaviour
{

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(60f, 45f, 0);
    }
}

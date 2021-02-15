using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnyKeyTo : MonoBehaviour
{
    public UnityEvent onAnyKey;
    
    void Update()
    {
        if(Input.anyKey)
        {
            Debug.Log("Any Key Pressed");
            onAnyKey.Invoke();
        }
    }
}

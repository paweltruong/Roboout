using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedEvent : MonoBehaviour
{
    [SerializeField] float afterXSeconds;
    public UnityEvent action;

    private void Start()
    {
        StartCoroutine(DoActionAfterTime());
    }

    IEnumerator DoActionAfterTime()
    {
        yield return new WaitForSeconds(afterXSeconds);
        action.Invoke();
    }
}

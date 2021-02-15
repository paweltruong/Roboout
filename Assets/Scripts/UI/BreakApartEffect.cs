using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakApartEffect : MonoBehaviour
{
    //TODO:PTRU this will take some time, for now without break apart effect
    //https://www.youtube.com/watch?v=bCkvh0lZFVk&ab_channel=RomiFauzi

    [SerializeField]
    GameObject[] rigidBodiesAndCollidersToActivate;


    private void Awake()
    {
        if (rigidBodiesAndCollidersToActivate != null && rigidBodiesAndCollidersToActivate.Length == 0)
            Debug.LogWarning("RigidBodies not set");
    }

    public void BreakApart()
    {
        foreach(var go in rigidBodiesAndCollidersToActivate)
        {
            if (go != null)
            {
                var rb = go.GetComponent<Rigidbody2D>();
                if(rb != null)
                    rb.bodyType = RigidbodyType2D.Dynamic;
                var col = go.GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = true;
            }
        }
    }
}

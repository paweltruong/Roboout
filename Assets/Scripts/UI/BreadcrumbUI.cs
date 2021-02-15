using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class BreadcrumbUI : MonoBehaviour
{
    /// <summary>
    /// When any of these encounters is complete you can pass through this
    /// </summary>
    [SerializeField] EncounterUI[] unlockers;
    [SerializeField] bool isLocked;
    SpriteRenderer sr;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (unlockers != null && unlockers.Any(u => u != null && u.isAvailable && u.isCompleted))
            isLocked = false;
        //TODO:refactor this propable can be done without update, but on event
        UpdateColor();
    }

    void UpdateColor()
    {
        //TODO:move colors to const class
        if (sr != null)
        {
            if (isLocked)
                sr.color = Color.black;
            else
            {
                sr.color = new Color32(116, 153, 92, 255);
            }
        }
    }
}

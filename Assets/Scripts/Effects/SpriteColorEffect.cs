using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorEffect : MonoBehaviour
{
    [SerializeField] Color Color1;
    [SerializeField] Color Color2;

    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor1()
    {
        sr.color = Color1;
    }
    public void SetColor2()
    {
        sr.color = Color2;
    }

}

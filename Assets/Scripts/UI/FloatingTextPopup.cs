using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextPopup : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float fadeStart = 3f;
    [SerializeField] float alphaFadeSpeed = 3f;
    [SerializeField] float minFontSize = 2f;
    TextMeshPro textField;
    [SerializeField] public string Value;
    float timer;

    private void Awake()
    {
        textField = GetComponent<TextMeshPro>();
        if (textField == null)
            Debug.LogError("FloatingTextPopup text field null");
    }

    private void Start()
    {
        timer = fadeStart;
    }

    public void Set(string value)
    {
        this.Value = value;
        textField.text = Value;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += new Vector3(0, speed) * Time.deltaTime;
        if (timer <= 0)
        {
            var newColor = textField.color;
            newColor.a -= alphaFadeSpeed*Time.deltaTime;
            textField.color = newColor;
            if (textField.color.a < 0)
                Destroy(this);
        }

        if (textField.fontSize > minFontSize)
            textField.fontSize -= Time.deltaTime;
    }
}

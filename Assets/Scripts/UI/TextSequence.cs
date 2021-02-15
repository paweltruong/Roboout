using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextSequence : MonoBehaviour
{
    TextMeshProUGUI textControl;
    [System.Serializable]
    public class TextEntry
    {
        /// <summary>
        /// Delay after start of the scene or previous entry finished
        /// </summary>
        public float Delay;
        /// <summary>
        /// Duration how long will text be displayed
        /// </summary>
        public float Duration;
        [Multiline]
        public string Text;
        public UnityEvent textOverride;
    }

    [SerializeField()]
    TextEntry[] Items;
    int currentIndex = -1;

    public UnityEvent onSequenceFinished;


    private void Awake()
    {
        onSequenceFinished = new UnityEvent();
        textControl = GetComponent<TextMeshProUGUI>();
        if (textControl == null)
            Debug.LogError("No TMP control found");
    }

    private void Start()
    {
        PlayNext();
    }

    void PlayNext()
    {
        if (currentIndex + 1 < Items.Length && Items.Length > 0)
        {
            ++currentIndex;
            var entry = Items[currentIndex];
            StartCoroutine(DisplayText(entry));
        }
        else if(currentIndex >= Items.Length)
        {
            onSequenceFinished.Invoke();
        }
    }

    IEnumerator DisplayText(TextEntry entry)
    {
        textControl.text = string.Empty;
        yield return new WaitForSeconds(entry.Delay);
        if (entry.textOverride.GetPersistentEventCount() > 0)
            entry.textOverride.Invoke();
        else
            textControl.text = entry.Text;
        yield return new WaitForSeconds(entry.Duration);
        textControl.text = string.Empty;
        PlayNext();
    }
}

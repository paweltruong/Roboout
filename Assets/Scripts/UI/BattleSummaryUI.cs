using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BattleSummaryUI : MonoBehaviour
{
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject DefeatPanel;
    [SerializeField] TMPro.TextMeshProUGUI BoltsDropTextField;
    /// <summary>
    /// in seconds
    /// </summary>
    [SerializeField] float boltDropIncrementInterval = 0.1f;
    [SerializeField] AudioClip BoltStartDropSFX;
    [SerializeField] AudioClip BoltContinueDropSFX;
    [SerializeField] AudioClip BoltEndDropSFX;
    AudioSource audioSource;

    bool boltDropInProgress;

    private void Awake()
    {
        if (VictoryPanel == null || DefeatPanel == null)
            Debug.LogError("Battle summary panels not set up");

        audioSource = GetComponent<AudioSource>();

        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);
    }
    private void Start()
    {
    }

    public void ShowVictory()
    {
        Debug.Log("UISummary Victory");
        this.gameObject.SetActive(true);
        VictoryPanel.SetActive(true);
        DefeatPanel.SetActive(false);
    }
    public void ShowDefeat()
    {
        Debug.Log("UISummary Defeat");
        this.gameObject.SetActive(true);
        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(true);
    }

    public void AddBoltsDrop(float value)
    {
        if (value > 0)
        {

            StartCoroutine(ShowBoltDropAnimation(value));
            StartCoroutine(PlayBoltDropSound());
        }
    }

    IEnumerator ShowBoltDropAnimation(float amountToAdd)
    {
        boltDropInProgress = true;

        float currentAmount = 0;
        while (amountToAdd > 0)
        {
            Debug.Log($"CurrentAmount: {currentAmount}");
            BoltsDropTextField.text = Mathf.FloorToInt(currentAmount).ToString();
            yield return new WaitForSeconds(boltDropIncrementInterval);
            ++currentAmount;
            amountToAdd--;
        }
        boltDropInProgress = false;
    }

    IEnumerator PlayBoltDropSound()
    {
        audioSource.clip = BoltStartDropSFX;
        audioSource.loop = false;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        if (boltDropInProgress)

            audioSource.clip = BoltContinueDropSFX;
        audioSource.loop = true;
        audioSource.Play();

        while (boltDropInProgress)
        {
            yield return new WaitForFixedUpdate();
        }

        audioSource.clip = BoltEndDropSFX;
        audioSource.loop = false;
        audioSource.Play();

    }
}

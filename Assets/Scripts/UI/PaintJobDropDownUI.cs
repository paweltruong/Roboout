using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// used in robo galery scene, will be usen in workshop encounter?
/// </summary>
public class PaintJobDropDownUI : MonoBehaviour
{
    [SerializeField] PaintJobManager paintJobManager;
    TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    void Start()
    {
        if (paintJobManager == null)
            Debug.LogError("Paintjob Manager is null");

        dropdown.ClearOptions();
        var values = System.Enum.GetNames(typeof(PaintJob)).ToList();
        dropdown.AddOptions(values);
        Bind();
        //attache to update paint job when drop down changed
        dropdown.onValueChanged.AddListener(paintJobManager.SelectPaintjobIndex);
        //when paintjob changed without drop down update dropdown
        paintJobManager.onSelectedPaintJobChanged.AddListener(OnSelectedPaintJobChanged);
    }

    void Bind()
    {
        var enumNameIndex = paintJobManager.selectedPaintJob.GetEnumElementIndexByName();
        if (enumNameIndex >= 0)
            dropdown.value = enumNameIndex;
    }

    void OnSelectedPaintJobChanged()
    {
        Bind();
    }
}

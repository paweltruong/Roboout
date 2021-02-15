using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Can change robot paint jobs in RoboGalleryScene and later in workshop?
/// </summary>
[ExecuteInEditMode]
public class PaintJobManager : MonoBehaviour
{
    [Header("PaintJob")]
    [SerializeField] internal PaintJob selectedPaintJob;
    [SerializeField] PaintJobLibrary library;

    public UnityEvent onSelectedPaintJobChanged;


    public void ApplyPaint(IEnumerable<IPaintJobPaintable> paintables)
    {
        if (library != null && paintables != null)
        {
            foreach (var paintable in paintables)
            {
                if (paintable != null && paintable.PaintJobMapping != null)
                {
                    foreach (var paintJobElement in System.Enum.GetValues(typeof(PaintJobElement)))
                    {
                        var color = library.GetColor(selectedPaintJob, (PaintJobElement)paintJobElement);
                        switch ((PaintJobElement)paintJobElement)
                        {
                            case PaintJobElement.BodyMain:
                                PaintAll(ref paintable.PaintJobMapping.BodyMain, color);
                                break;
                            case PaintJobElement.BodySecondary:
                                PaintAll(ref paintable.PaintJobMapping.BodySecondary, color);
                                break;
                            case PaintJobElement.Detail1:
                                PaintAll(ref paintable.PaintJobMapping.Detail1, color);
                                break;
                            case PaintJobElement.Detail2:
                                PaintAll(ref paintable.PaintJobMapping.Detail2, color);
                                break;
                            case PaintJobElement.Detail3:
                                PaintAll(ref paintable.PaintJobMapping.Detail3, color);
                                break;
                            case PaintJobElement.Plugs:
                                PaintAll(ref paintable.PaintJobMapping.Plugs, color);
                                break;
                            default:
                                break;
                        }
                    }

                    var additionals = paintable.PaintJobAdditionals.FirstOrDefault(a => a.paintJobKey == selectedPaintJob);
                    if (additionals != null && additionals.AdditionalGameObjects != null)
                    {
                        foreach (var additional in additionals.AdditionalGameObjects)
                            if (additional != null)
                            {
                                additional.SetActive(true);
                            }
                    }
                    else
                    {
                        //hide other additionals
                        foreach(var otherAdditionals in paintable.PaintJobAdditionals)
                        {
                            if(otherAdditionals != null && otherAdditionals.AdditionalGameObjects != null)
                            {
                                foreach (var additionalGO in otherAdditionals.AdditionalGameObjects)
                                    additionalGO.SetActive(false);
                            }
                        }

                    }
                }
            }
        }
    }

    void PaintAll(ref SpriteRenderer[] parts, Color paintColor)
    {
        if (parts != null && parts.Length > 0)
        {
            foreach (var sr in parts)
            {
                if (sr != null)
                    sr.color = paintColor;
            }
        }
    }

    public void SelectPaintjobIndex(int index)
    {
        Debug.Log($"SelectPaintjobIndex: {index} = {(PaintJob)index}");
        selectedPaintJob = index.GetEnumValueByIndex<PaintJob>();
        onSelectedPaintJobChanged.Invoke();
    }

    public void NextPaintjob()
    {
        var paintJobValues = System.Enum.GetValues(typeof(PaintJob));
        bool found = false;
        for(int i=0;i<paintJobValues.Length;++i)
        {
            if((int)selectedPaintJob == (int)paintJobValues.GetValue(i) 
                && i + 1 < paintJobValues.Length)
            {
                selectedPaintJob = (PaintJob)paintJobValues.GetValue(i+1);
                onSelectedPaintJobChanged.Invoke();
                found = true;
                break;
            }
            else if((int)selectedPaintJob == (int)paintJobValues.GetValue(i))
            {
                //reached last PaintJob so start from begining
                selectedPaintJob = (PaintJob)paintJobValues.GetValue(0);
                onSelectedPaintJobChanged.Invoke();
                found = true;
                break;
            }
        }

        if (!found)
            Debug.LogWarning("Nie ma paintJobów?");
    }
    public void PreviousPaintjob()
    {
        var paintJobValues = System.Enum.GetValues(typeof(PaintJob));
        bool found = false;
        for (int i = paintJobValues.Length -1; i >=0; --i)
        {
            if ((int)selectedPaintJob == (int)paintJobValues.GetValue(i)
                && i - 1 >= 0)
            {
                selectedPaintJob = (PaintJob)paintJobValues.GetValue(i-1);
                onSelectedPaintJobChanged.Invoke();
                found = true;
                break;
            }
            else if ((int)selectedPaintJob == (int)paintJobValues.GetValue(i))
            {
                //this is the first so jump to the last one
                selectedPaintJob = (PaintJob)paintJobValues.GetValue(paintJobValues.Length - 1);
                onSelectedPaintJobChanged.Invoke();
                found = true;
                break;
            }
        }

        if (!found)
            Debug.LogWarning("Nie ma paintJobów?");
    }
}

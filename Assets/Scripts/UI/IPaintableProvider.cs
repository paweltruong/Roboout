using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

/// <summary>
/// responsible for providing key properties and events for PaintjobManager(which operates on IPaintableProvider objects)
/// </summary>
public interface IPaintableProvider
{
    PaintJobManager PaintJobManager { get; }

    IEnumerable<IPaintJobPaintable> GetPaintables();
}

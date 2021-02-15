using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IPaintJobPaintable
{
    PaintJobGroups PaintJobMapping { get; set; }

    //TODO: change skins to creating custom paint sets with color picker, or unlocking colors as reward
    PaintJobSpecificParts[] PaintJobAdditionals { get; set; }
}

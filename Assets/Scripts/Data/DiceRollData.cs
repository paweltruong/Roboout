using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DiceRollData
{
    public int value;
    public bool isOverclock;
    public bool isAdditional;
    public bool isUsed;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="overclock"></param>
    /// <returns>was overclocked</returns>
    public bool RollAndTryToOverclock(IDiceRollerService diceRollerService, int overclock = 0, bool isAdditional = false)
    {
        if (overclock > 0)
        {
            value = overclock;
            isOverclock = true;
        }
        else
            value = diceRollerService.Roll();
        if (isAdditional)
            this.isAdditional = true;
        return overclock > 0;
    }
}

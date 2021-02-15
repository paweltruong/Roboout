
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages visual presentation of combat on client
/// </summary>
public class DiceRollerService : IDiceRollerService
{
    public int Roll() => Random.Range(1, 7);
}

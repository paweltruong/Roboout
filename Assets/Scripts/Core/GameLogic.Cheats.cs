
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameLogic
{
    //cheats
    //--------------------------
    public void DamagePlayerKinetic(float amount)
    {
        PlayerData.ApplyDamageKinetic(amount);
    }

    public void DamageEnemyKinetic(float amount)
    {
        BattleData.AllFighters.FirstOrDefault(r => r.Identity == RobotIdentity.Enemy).ApplyDamageKinetic(amount);
    }
    public void HealPlayer(float amount)
    {
        PlayerData.Heal(amount);
    }
    public void HealEnemy(float amount)
    {
        BattleData.AllFighters.FirstOrDefault(r => r.Identity == RobotIdentity.Enemy).Heal(amount);
    }
}
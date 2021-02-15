
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//battle logic
public partial class GameLogic
{
    public void LoadBattle(ICombatSceneLogic battleScene)
    {
        this.battleScene = battleScene;

        if (!gameState.gameInProgress)
        {
            //in debug scene, have to init new game first
            StartNewGame();
        }

        if (gameState != null && gameState.gameInProgress)
        {
            if (gameState.battleData != null)
            {
                this.battleScene.Initialize();

                gameState.battleData.onDiceRolled += BattleData_onDiceRolled;
                gameState.battleData.onDiceUsed += BattleData_onDiceUsed;
                gameState.battleData.onMoveChanged += BattleData_onMoveChanged; ;
                gameState.battleData.onModuleUsedOn += BattleData_onModuleUsedOn;
                gameState.battleData.onRobotKilled += BattleData_onRobotKilled;

                if (gameState.battleData.inProgress)
                {
                    ContinueBattle();
                }
                else
                {
                    SetUpNewBattle();
                }
            }
        }
    }

    private void BattleData_onDiceUsed(int diceIndex)
    {
        battleScene.HideDice(diceIndex);
    }

    private void BattleData_onDiceRolled(object sender, EventArgs e)
    {
        battleScene.RefreshDicesUI();
    }

    private void BattleData_onModuleUsedOn(RoboInstanceData source, ModuleInstance module, RoboInstanceData target)
    {
        battleScene.SetBattleHint(System.String.Format(StringResources.BattleHintText_EnemyUsingModuleFormat,
            gameState.battleData.CurrentMoveRobotData.RoboName,
            module.ModuleName, target.RoboName));
    }

    private void BattleData_onMoveChanged(Guid value)
    {
        battleScene.RefreshCurrentMoveUI();
    }

    private void SetUpNewBattle()
    {
        var selectedEncounter = gameState.mapData.SelectedEncounter;
        if (selectedEncounter != null && selectedEncounter.IsBattleType)
        {
            gameState.battleData.SetUpNewBattle();

            battleScene.ShowPreBattleUI();

            //Spawn robots

            gameState.battleData.AddSpawnedRobot(battleScene.SpawnPlayer());
            //Spwan enemy robots
            if (battleScene.IsDebug)
            {
                //when debug battle scene spawn enemies from BattleScene debug settings
                foreach (var enemyBlueprint in battleScene.DebugEnemyBlueprints)
                    gameState.battleData.AddSpawnedRobot(battleScene.SpawnEnemy(enemyBlueprint));
            }
            else
            {
                //when true battle spawn enemies from encounter configuration
                if (selectedEncounter.enemyBlueprints == null || selectedEncounter.enemyBlueprints.Count == 0)
                {
                    Debug.LogError($"No enemies in encounter!(See if {nameof(ICombatSceneLogic.IsDebug)} on {nameof(ICombatSceneLogic)} is checked)");
                }
                foreach (var enemyBlueprint in selectedEncounter.enemyBlueprints)
                    gameState.battleData.AddSpawnedRobot(battleScene.SpawnEnemy(enemyBlueprint));
            }
        }
        else
            Debug.LogError("Selected encounter is not a valid battle");
    }

    private void ContinueBattle()
    {
        battleScene.ShowContinueBattleUI();

        throw new NotImplementedException("TODO:spawn player and enemies based on BattleData (not blueprints)");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">module at hand index (index in hand)</param>
    public void UseModuleAtHandIndex(int index)
    {
        if (index >= 0
            && BattleData.ModulesAtHand.Count() > index
            && BattleData.CurrentMoveRobotIsPlayer)
        {
            var module = BattleData.ModulesAtHand.ElementAt(index);
            //if can cast on self pick self as target else assume its hostile and pick first enemy
            //TODO:refactor when added Threat for Robots to pick target by priority
            var defaultTarget = module.AllowedTargets.Any(m => m == AllowedTargets.Self) ? BattleData.CurrentMoveRobotData : BattleData.AllFighters.FirstOrDefault(r => !r.IsKilled && r.Identity == RobotIdentity.Enemy);
            if (module != null && defaultTarget != null)
                TryUseModule(BattleData.CurrentMoveRobotData, module.Id, defaultTarget.Id);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceRobot">robot that is using a module</param>
    /// <param name="moduleId">module from source robot</param>
    /// <param name="targetRobotId">robot to use module on</param>
    /// <returns>true if succesfully used module</returns>
    public bool TryUseModule(RoboInstanceData sourceRobot, System.Guid moduleId, System.Guid targetRobotId)
    {
        if (sourceRobot.CanUseModule(moduleId, targetRobotId))
        {
            return UseModuleOn(sourceRobot.Id, sourceRobot.Identity, targetRobotId, moduleId, ModuleUsages.Active);
        }
        return false;
    }


    public bool UseModuleOn(Guid sourceRobotId, RobotIdentity identity, Guid targetRobotId, Guid moduleId, ModuleUsages usage)
    {
        bool used = true;
        RoboInstanceData source;
        RoboInstanceData target;
        ModuleInstance module = null;

        source = BattleData.AllFighters.FirstOrDefault(r => !r.IsKilled && r.Id == sourceRobotId);
        target = BattleData.AllFighters.FirstOrDefault(e => e.Id == targetRobotId);
        if (target != null)
        {
            module = source.AllEquippedModules.FirstOrDefault(m => m.Id == moduleId);
            if (module == null)
                Debug.LogError($"Module {moduleId} not found in {sourceRobotId}");
            if (module.IsActiveEffectModule && usage.HasFlag(ModuleUsages.Active))
            {
                used = ApplyModuleEffect(source, target, module);
                if (used)
                {
                    UseDice(module.DiceToActivate);
                    battleScene.ClearHandUI();
                    battleScene.RefreshAvailableModulesUI();
                }
            }
        }
        else
        {
            Debug.LogError($"No enemy {targetRobotId} in {nameof(BattleData)}");
        }
        if (used)
            BattleData.OnModuleUsed(source, module, target);
        return used;
    }

    /// <summary>
    /// apply module effect: trigger animation on source, apply effect on target, pay cost etc.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="module"></param>
    /// <returns></returns>
    bool ApplyModuleEffect(RoboInstanceData source, RoboInstanceData target, ModuleInstance module)
    {
        bool applied = false;
        var sourceLoadout = battleScene.Robots.FirstOrDefault(r => r.Id == source.Id);
        if (sourceLoadout == null)
            Debug.LogError($"sourceLoadout {source.Id} not found in scene");
        var moduleVisualization = sourceLoadout.GetModuleVisualization(module.Key, module.SlotIndex);
        if (moduleVisualization == null)
            Debug.LogError($"moduleVisualization {module.Key}, slot[{module.SlotIndex}] not found in Robot");

        var targetLoadout = battleScene.Robots.FirstOrDefault(r => r.Id == target.Id);
        if (targetLoadout == null)
            Debug.LogError($"targetLoadout {source.Id} not found in scene");

        UnityAction applyEffectAction = null;

        //damage after all calculations (armor, block, shield reductions)
        int appliedDamaged = 0;
        bool wasReduced = false;
        bool wasBlocked = false;
        bool wasAbsorbed = false;

        int totalKineticDamage = source.GetTotalKineticDamage(module.Id);
        int totalTHermalDamage = source.GetTotalThermalDamage(module.Id);

        switch (module.Animation)
        {
            case AnimationType.Kick1:
                applyEffectAction = () =>
                {
                    appliedDamaged = ApplyDamage(target, totalKineticDamage, totalTHermalDamage, out wasReduced, out wasBlocked, out wasAbsorbed);
                    moduleVisualization.onAnimationKick1ContactFrame.RemoveListener(applyEffectAction);
                    SetAnimationAndState(targetLoadout, target, appliedDamaged, wasBlocked);
                    source.ApplyPowerCost(module.PowerCost);
                };
                moduleVisualization.onAnimationKick1ContactFrame.AddListener(applyEffectAction);
                sourceLoadout.Kick1(module.Key, module.SlotIndex);
                applied = true;
                break;
            case AnimationType.Attack1:
                applyEffectAction = () =>
                {
                    appliedDamaged = ApplyDamage(target, totalKineticDamage, totalTHermalDamage, out wasReduced, out wasBlocked, out wasAbsorbed);
                    moduleVisualization.onAnimationAttack1ContactFrame.RemoveListener(applyEffectAction);
                    SetAnimationAndState(targetLoadout, target, appliedDamaged, wasBlocked);
                    source.ApplyPowerCost(module.PowerCost);
                };
                moduleVisualization.onAnimationAttack1ContactFrame.AddListener(applyEffectAction);
                sourceLoadout.Attack1(module.Key, module.SlotIndex);
                applied = true;
                break;
            case AnimationType.Block1:
                sourceLoadout.ToggleBlock(true);
                applied = true;
                break;

            case AnimationType.Shot1:
                applyEffectAction = () =>
                {
                    appliedDamaged = ApplyDamage(target, totalKineticDamage, totalTHermalDamage, out wasReduced, out wasBlocked, out wasAbsorbed);
                    moduleVisualization.onAnimationShotFrame.RemoveListener(applyEffectAction);
                    SetAnimationAndState(targetLoadout, target, appliedDamaged, wasBlocked);
                    source.ApplyPowerCost(module.PowerCost);
                };
                moduleVisualization.onAnimationShotFrame.AddListener(applyEffectAction);
                sourceLoadout.ShootRecoil1(module.Key, module.SlotIndex);
                applied = true;
                break;
            case AnimationType.Heal1:
                target.Heal(module.HpRegen);
                applied = true;
                break;
            default:
                break;
        }

        return applied;
    }

    /// <summary>
    /// apply damage to target (both types of damage)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageKinetic"></param>
    /// <param name="damageThermal"></param>
    /// <param name="wasReduced">true when some dmg was reduced by armor</param>
    /// <param name="wasBlocked">true when some damage was blocked with block skill</param>
    /// <param name="wasAbsorbed">true when some damage was absorbed by thermal shield </param>
    /// <returns></returns>
    int ApplyDamage(RoboInstanceData target, int damageKinetic, int damageThermal, out bool wasReduced, out bool wasBlocked, out bool wasAbsorbed)
    {
        int appliedDamage = 0;
        wasReduced = false;
        wasBlocked = false;
        wasAbsorbed = false;

        appliedDamage += target.ApplyDamageThermal(damageThermal, out wasAbsorbed);
        appliedDamage += target.ApplyDamageKinetic(damageKinetic, out wasReduced, out wasBlocked);

        return appliedDamage;
    }

    void SetAnimationAndState(RobotLoadout roboLoadout, RoboInstanceData robo, int appliedDamaged, bool blocked)
    {
        if (appliedDamaged > 0)
        {
            //even if some was blocked, the robot still got hit, so play hit animation
            roboLoadout.TakeHit1(appliedDamaged);
        }
        //everything was blocked so play block hit animation
        else if (blocked)
            roboLoadout.BlockHit();

        //block was pierced and there are no more block points, so lower the shield/turn of blocking animation
        if (robo.BlockPoints <= 0)
            if (blocked)
                roboLoadout.ToggleBlock(false);
    }

    /// <summary>
    /// mark dice as used
    /// </summary>
    /// <param name="diceToActivate"></param>
    private void UseDice(DiceRoll diceToActivate)
    {
        int matchingIndex = -1;
        for (int i = 0; i < BattleData.diceRolls.Count; ++i)
        {
            var dice = BattleData.diceRolls[i];

            if (dice.value.RollValueMatchedDiceRoll(diceToActivate))
            {
                dice.isUsed = true;
                matchingIndex = i;
                break;
            }
        }
        if (matchingIndex >= 0)
        {
            BattleData.OnDiceUsed(matchingIndex);
        }
    }
    
    private void BattleData_onRobotKilled(RoboInstanceData sender)
    {
        Debug.LogWarning($"Robo killed {sender.RoboName}");
        CheckVictory();
        CheckDefeat();
    }

    private void CheckDefeat()
    {
        if (BattleData.AllFighters.Any(r => r.Identity == RobotIdentity.Player && r.IsKilled))
        {
            Debug.LogWarning("DEFEAT!");
            BattleData.SetDefeat();

            ++gameState.battleCount;
            battleScene.ShowDefeat();
        }
    }

    private void CheckVictory()
    {
        if (BattleData.Enemies.Any(r => !r.IsKilled))
        {
            //at least one enemy is alive
        }
        else
        {
            Debug.LogWarning("Victory");
            BattleData.SetVictory();
            BattleData.SetBoltsThatDropped();

            ++gameState.battleCount;
            battleScene.ShowVictory();
        }
    }

    public void NextTurn()
    {
        if (gameState.battleData.hasStarted && gameState.battleData.CurrentMoveRobotIsPlayer)
            gameState.battleData.autoEndCanceled = true;

        battleScene.ClearHandUI();

        if (gameState.battleData.hasFinished)
        {
            battleScene.HideAllUI();
        }
        else
        {

            if (!gameState.battleData.AreAllMoved && gameState.battleData.hasStarted)
            {
                gameState.battleData.FinishMove();

                //next robot
                gameState.battleData.DrawDices();
                TryUseModule();
            }
            else
            {
                gameState.battleData.NextTurn();
                gameState.battleData.DrawDices();
                battleScene.RefreshAvailableModulesUI();
                battleScene.ToggleNextTurnUI(true);
            }
        }
    }

    void TryUseModule()
    {
        battleScene.RefreshAvailableModulesUI();
        if (!gameState.battleData.CurrentMoveRobotIsPlayer)
        {
            if (gameState.battleData.ModulesAtHand.Any() && !gameState.battleData.CurrentMove.Skipped)
            {
                battleScene.SetBattleHint(System.String.Format(StringResources.BattleHintText_EnemyChoosingModuleFormat,
                    gameState.battleData.CurrentMoveRobotData.RoboName));
                battleScene.StartCoroutine(gameState.aiDecicionWaitTime, gameState.aiAfterMoveWaitTime, AIModuleUseAlgorithm, TryUseModule);
            }
            else
                battleScene.StartCoroutine(gameState.aiAfterMoveWaitTime,0, NextTurn, null);
        }
    }

    void AIModuleUseAlgorithm()
    {
        if (gameState.battleData.ModulesAtHand.Count() > 0)
        {
            var getDamageModules = gameState.battleData.ModulesAtHand.Where(m => m.DamageKinetic > 0 || m.DamageThermal > 0 || m.HpRegen > 0);
            //TODO:for now every enemy has the same default AI behaviour, later ai personalities will be added in resources
            var ai = new AIAlgorithm();
            RoboInstanceData selectedTarget = null;
            var module = ai.GetModuleToUse(gameState.battleData.CurrentMoveRobotData, getDamageModules, gameState.battleData.GetTargetsForCurrent(), out selectedTarget);
            if (module != null && selectedTarget != null)
            {
                TryUseModule(gameState.battleData.CurrentMoveRobotData, module.Id, selectedTarget.Id);
            }
            else
            {
                gameState.battleData.SkipMove();
                Debug.LogWarning("AI will skip using module");
                battleScene.SetBattleHint(System.String.Format(StringResources.BattleHintText_SkipFormat,
                    gameState.battleData.CurrentMoveRobotData.RoboName));
            }
        }
    }








}
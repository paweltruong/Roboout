using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class AIAlgorithm
{
    //brain characteristic (for more offense put more weight in damage)
    float damageWeight = .5f;
    float regenWeight = .5f;
    float powerCostWeight = .5f;

    float decisionPointsForKill = 100f;
    float decionPointsForSuicide = -50f;
    /// <summary>
    /// is always go for a kill even if will die, if false then robot will regen if possible or skip
    /// </summary>
    public bool isKamikaze = false;
    /// <summary>
    /// if not kamikaze will prioritize healing when belowe % hp  0.2 => belowe 20%
    /// </summary>
    float healPriorityThreshold = 0.3f;
    /// <summary>
    /// when HP% will be less or equal x then healing will we impact in decisionValue, this is because robot would heal himself all the time because healing has no power cost therefore would lose only with free attack
    /// </summary>
    public float healDecisionPointsThreshold = 0.7f;


    public ModuleInstance GetModuleToUse(RoboInstanceData source, IEnumerable<ModuleInstance> modulesToConsider, IEnumerable<RoboInstanceData> availableHostileTargets, out RoboInstanceData selectedTarget)
    {
        //TODO: for now we take first enemy, later there should be threatbased algorithm
        //select target
        selectedTarget = availableHostileTargets.FirstOrDefault();
        if (selectedTarget != null)
        {
            //select module
            var module = PickModule(source, modulesToConsider, selectedTarget);

            if (module == null)
            {
                //ai will skip turn because it wants to survive
            }
            else
            {
                //if module can be cast on self then change target to self = prevent casting heal on enemy faction
                if (module.AllowedTargets.Any(t => t == AllowedTargets.Self))
                    selectedTarget = source;
            }

            return module;
        }
        else
            return null;
    }

    ModuleInstance PickModule(RoboInstanceData source, IEnumerable<ModuleInstance> modulesToConsider, RoboInstanceData target)
    {
        ModuleInstance selectedModule = null;

        var calculations = CalculateModules(source, modulesToConsider, target);
        var calculationWithMaxDecisionValue = calculations.Aggregate((c1, c2) => c1.DecisionValue > c2.DecisionValue ? c1 : c2);


        //if not kamikaze and will die save yourself
        if (!isKamikaze)
        {
            //check healPriorityTreshold, 
            if ((float)source.CurrentPower / (float)source.MaxPower <= healPriorityThreshold
                && calculations.Any(c => !c.sourceWillDie && c.RegenPower > 0))//is there a chance to heal and stay alive(no dot etc)
            {
                calculationWithMaxDecisionValue = calculations.Where(c => c.RegenPower > 0 && !c.sourceWillDie)
                    .Aggregate((c1, c2) => c1.DecisionValue > c2.DecisionValue ? c1 : c2);
            }
            else
            {
                // if max decision value ends with death, then try to survive or skip
                if (calculationWithMaxDecisionValue.sourceWillDie)
                {
                    var decisionsThatNotEndWithSourceDeath = calculations.Where(c => !c.sourceWillDie);
                    if (decisionsThatNotEndWithSourceDeath.Any())
                    {
                        //there is move that will not kill me
                        calculationWithMaxDecisionValue = decisionsThatNotEndWithSourceDeath.Aggregate((c1, c2) => c1.DecisionValue > c2.DecisionValue ? c1 : c2);
                    }
                    else
                    {
                        //do nothing try to survive
                        calculationWithMaxDecisionValue = null;
                    }
                }
            }
        }
        if (calculationWithMaxDecisionValue != null)
            selectedModule = modulesToConsider.FirstOrDefault(m => m.Id == calculationWithMaxDecisionValue.Id);
        return selectedModule;
    }

    private IEnumerable<ModuleCalculations> CalculateModules(RoboInstanceData source, IEnumerable<ModuleInstance> modulesToConsider, RoboInstanceData target)
    {
        var calculations = new List<ModuleCalculations>();
        for (int i = 0; i < modulesToConsider.Count(); ++i)
        {
            var module = modulesToConsider.ElementAt(i);
            var totalKineticDMG = source.GetTotalKineticDamage(module.Id);
            var totalThermalDMG = source.GetTotalThermalDamage(module.Id);
            var calculation = new ModuleCalculations();
            calculation.Id = module.Id;

            try
            {
                int kineticDMG = target.ApplyDamageKinetic(totalKineticDMG, out bool wasReduced, out bool wasBlocked, true);
                int thermalDMG = target.ApplyDamageThermal(totalThermalDMG, out bool wasAbsorbed, true);

                int appliedDamageSum = kineticDMG + thermalDMG;
                calculation.DecisionValue += appliedDamageSum * damageWeight;
                calculation.DecisionValue -= module.PowerCost * powerCostWeight;
                if (source.HealthPercent <= healDecisionPointsThreshold)
                    calculation.DecisionValue += module.HpRegen * regenWeight;
                calculation.AppliedDamage = appliedDamageSum;
                calculation.PowerCost = module.PowerCost;

                //if target will be killed
                if (target.CurrentPower - appliedDamageSum <= 0)
                {
                    calculation.DecisionValue += decisionPointsForKill;
                    calculation.targetWillDie = true;
                }
                int hpAfterKill = calculation.targetWillDie ? module.HpForEnemy : 0;
                //if this will kill me
                if (source.CurrentPower - module.PowerCost + hpAfterKill <= 0)
                {
                    calculation.DecisionValue += decionPointsForSuicide;
                    calculation.sourceWillDie = true;
                }
                calculations.Add(calculation);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        return calculations;
    }
}

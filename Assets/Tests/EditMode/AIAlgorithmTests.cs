using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIAlgorithmTests
    {
        /// <summary>
        /// when have 1 hp and can use module to attack but it will kill him, he still gonna use it
        /// </summary>
        [Test]
        public void AI_AlwaysAttack_WhenKamikze()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = true;
            int sourceHP = 1;
            int dmg = 2;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 1, ArmSlots = 1 }, null, null,
                //arm
                new Module[] { new Module { Slot = SlotType.Arm, DamageKinetic = dmg, Key = ModuleKey.ARM_010, PowerCost = sourceHP * 2, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg * 2 /*cannot suicide kill*/});
            ModuleInstance[] modulesToConsider = new[] { source.AllEquippedModules.FirstOrDefault() };

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNotNull(moduleToUse);
        }

        /// <summary>
        /// if 1 hp and using module will kill you, dont do anything wait for next turn and maybe regen
        /// </summary>
        [Test]
        public void AI_SkipMove_WhenIsNoKamikazeAndNoMovesToStayAlive()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = false;
            int sourceHP = 1;
            int dmg = 2;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 1, ArmSlots = 1 }, null, null,
                //arm
                new Module[] { new Module { Slot = SlotType.Arm, DamageKinetic = dmg, Key = ModuleKey.ARM_010, PowerCost = sourceHP * 2, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg * 2 /*cannot suicide kill*/});
            ModuleInstance[] modulesToConsider = new[] { source.AllEquippedModules.FirstOrDefault() };

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNull(moduleToUse);
        }

        /// <summary>
        /// there was error when ai(not kamikaze) and player robots had 2/10 hp, ai rolled D6 and could use ARM010 that would kill player and ai, they ai stucked making no decision
        /// </summary>
        [Test]
        public void AI_SkillMove_WhenNoKamikazeAndCanOnlyKillHimself()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = false;
            int sourceHP = 2;
            int dmg = 3;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 10, ArmSlots = 1, ReactorSlots = 1 },
                null,
                null,
                //arm
                new Module[] { new Module { Slot = SlotType.Arm, DamageKinetic = dmg, Key = ModuleKey.ARM_010, PowerCost = sourceHP * 2, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            source.ApplyDamageKinetic(8 /*basehp-dmg  should be <= AIAlgorithm.HPThreashold*/);
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg /*can suicide kill*/});
            ModuleInstance[] modulesToConsider = source.AllEquippedModules.ToArray();

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNull(moduleToUse);
        }

        /// <summary>
        /// when ai robot had 7/10 hp , threshold was 0.7 and could heal or attack, should prioritize killing player instead of healing
        /// </summary>
        [Test]
        public void AI_Attack_WhenNotKamikazeAndBelowHealThresholdButCanKillPlayer()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = false;
            sut.healDecisionPointsThreshold = 0.7f;
            float sourceHP = sut.healDecisionPointsThreshold * 10;
            int dmg = 2;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 10, LegSlots = 1, ReactorSlots = 1 },
                //reactor
                new Module[] { new Module { Slot = SlotType.Reactor, BonusHpRegen = 2, Key = ModuleKey.REAC_001, PowerCost = 0, AllowedTargets = new[] { AllowedTargets.Self } } },
                null,
                null,
                //legs
                new Module[] { new Module { Slot = SlotType.Leg, DamageKinetic = dmg, Key = ModuleKey.LEG_001, PowerCost = 1, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            source.ApplyDamageKinetic(3 /*basehp-dmg should be <= AIAlgorithm.healDecisionPointsThreshold*/);
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg /*can kill*/});
            ModuleInstance[] modulesToConsider = source.AllEquippedModules.ToArray();

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNotNull(moduleToUse);
            Assert.AreEqual(ModuleKey.LEG_001, moduleToUse.Key);
        }

        /// <summary>
        /// when is not kamikazee, and heal available among other attacks, should consider heal as valuable decision
        /// (unless can kill then look other test:AI_Attack_WhenNotKamikazeAndBelowHealHPThresholdButCanKillPlayer)
        /// ai 7/10  hp threshold 0.7 and available is heal and attack
        /// </summary>
        [Test]
        public void AI_Heal_WhenNotKamikazeAndBelowHealThreshold()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = false;
            sut.healDecisionPointsThreshold = 0.7f;
            float sourceHP = sut.healDecisionPointsThreshold * 10;
            int dmg = 2;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 10, LegSlots = 1, ReactorSlots = 1 },
                //reactor
                new Module[] { new Module { Slot = SlotType.Reactor, BonusHpRegen = 2, Key = ModuleKey.REAC_001, PowerCost = 0, AllowedTargets = new[] { AllowedTargets.Self } } },
                null,
                null,
                //legs
                new Module[] { new Module { Slot = SlotType.Leg, DamageKinetic = dmg, Key = ModuleKey.LEG_001, PowerCost = 1, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            source.ApplyDamageKinetic(3 /*basehp-dmg should be <= AIAlgorithm.healDecisionPointsThreshold*/);
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg * 2 /*cannot kill*/});
            ModuleInstance[] modulesToConsider = source.AllEquippedModules.ToArray();

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNotNull(moduleToUse);
            Assert.AreEqual(ModuleKey.REAC_001, moduleToUse.Key);
        }

        /// <summary>
        /// when ai have 8/10 hp  threshold 0.7  and can heal or attack, should attack because heal threashold was not reached
        /// </summary>
        [Test]
        public void AI_Attack_WhenNotKamikazeAndAboveHealThreshold()
        {
            //Arrange
            var sut = new AIAlgorithm();
            sut.isKamikaze = false;
            sut.healDecisionPointsThreshold = 0.7f;
            float sourceHP = sut.healDecisionPointsThreshold * 10;
            int dmg = 2;
            var source = new RoboInstanceData() { Identity = RobotIdentity.Enemy };
            var target = new RoboInstanceData() { Identity = RobotIdentity.Player };
            RoboInstanceData[] availableTargets = new[] { target };
            RoboInstanceData selectedTarget;
            source.SetCompleteLoadout(new Mainframe { BaseHp = 10, LegSlots = 1, ReactorSlots = 1 },
                //reactor
                new Module[] { new Module { Slot = SlotType.Reactor, BonusHpRegen = 2, Key = ModuleKey.REAC_001, PowerCost = 0, AllowedTargets = new[] { AllowedTargets.Self } } },
                null,
                null,
                //legs
                new Module[] { new Module { Slot = SlotType.Leg, DamageKinetic = dmg, Key = ModuleKey.LEG_001, PowerCost = 1, AllowedTargets = new[] { AllowedTargets.Enemy } } });
            source.ApplyDamageKinetic(2);
            target.SetCompleteLoadout(new Mainframe { BaseHp = dmg * 2 /*cannot kill*/});
            ModuleInstance[] modulesToConsider = source.AllEquippedModules.ToArray();

            //Act
            var moduleToUse = sut.GetModuleToUse(source, modulesToConsider, availableTargets, out selectedTarget);
            //Assert
            Assert.IsNotNull(moduleToUse);
            Assert.AreEqual(ModuleKey.LEG_001, moduleToUse.Key);
        }
    }
}

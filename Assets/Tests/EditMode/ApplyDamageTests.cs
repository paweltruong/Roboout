using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ApplyDamageTests
    {
        /// <summary>
        /// Apply damage to plain hp (without armor, shield, block)
        /// </summary>
        [Test]
        public void ApplyDamageToPlainHp()
        {
            //Arrange
            float damegeToApply = 3;
            RoboInstanceData robo = new RoboInstanceData();
            var mainframe = ScriptableObject.CreateInstance<Mainframe>();
            mainframe.BaseHp = 10;
            robo.SetCompleteLoadout(mainframe);
            //Arrange for event
            RoboInstanceData sender = null;
            bool eventWasCalled = false;
            float absorbedDamage = 0;

            robo.onPowerChanged += (s, e) =>
              {
                  eventWasCalled = true;
                  sender = s;
                  absorbedDamage = e.Value * -1;
              };

            //Act
            robo.ApplyDamageKinetic(damegeToApply);

            //Assert
            Assert.IsTrue(eventWasCalled, $"Event {nameof(RoboInstanceData.onPowerChanged)} was not invoked");
            Assert.AreEqual(sender, robo, "Sender does not match with invoked");
            Assert.AreEqual(absorbedDamage, damegeToApply, "Damage absorbed differs with applied");
            Assert.AreEqual(mainframe.BaseHp - damegeToApply, robo.CurrentPower, "CurrentPower was not updatd");
        }

        /// <summary>
        /// Apply damage to armored and hp (without shield, block)
        /// </summary>
        [Test]
        public void ApplyDamageToArmoredAndHp()
        {
            //Arrange
            float damegeToApply = 3;
            RoboInstanceData robo = new RoboInstanceData();
            var mainframe = ScriptableObject.CreateInstance<Mainframe>();
            mainframe.BaseHp = 10;
            mainframe.BaseArmor = 2;
            robo.SetCompleteLoadout(mainframe);
            //Arrange for event
            RoboInstanceData sender_onDamageAbsorbed = null;
            bool onPowerChanged = false;
            float absorbedDamage = 0;
            RoboInstanceData sender_onDamageReducedByArmor = null;
            bool onDamageReducedByArmor = false;
            float reducedDamage = 0;

            robo.onDamageReducedByArmor += (s, e) =>
            {
                onDamageReducedByArmor = true;
                sender_onDamageReducedByArmor = s;
                reducedDamage = e.Value;
            };
            robo.onPowerChanged += (s, e) =>
            {
                onPowerChanged = true;
                sender_onDamageAbsorbed = s;
                absorbedDamage = e.Value * -1;
            };

            //Act
            robo.ApplyDamageKinetic(damegeToApply);

            //Assert
            Assert.IsTrue(onDamageReducedByArmor, $"Event {nameof(RoboInstanceData.onDamageReducedByArmor)} was not invoked");
            Assert.IsTrue(onPowerChanged, $"Event {nameof(RoboInstanceData.onPowerChanged)} was not invoked");
            Assert.AreEqual(sender_onDamageAbsorbed, robo, "Sender does not match with invoked");
            Assert.AreEqual(sender_onDamageReducedByArmor, robo, "Sender does not match with invoked");
            Assert.AreEqual(reducedDamage, mainframe.BaseArmor, "Damage reduced differs with applied");
            Assert.AreEqual(absorbedDamage, damegeToApply - mainframe.BaseArmor, "Damage absorbed differs with applied");
            Assert.AreEqual(mainframe.BaseHp - damegeToApply + mainframe.BaseArmor, robo.CurrentPower, "CurrentPower was not updatd");
        }


        /// <summary>
        /// Apply damage to block and hp (without armor, shield)
        /// </summary>
        [Test]
        public void ApplyDamageToBlockedAndHp()
        {
            //Arrange
            float damegeToApply = 3;
            RoboInstanceData robo = new RoboInstanceData();
            var mainframe = ScriptableObject.CreateInstance<Mainframe>();
            mainframe.BaseHp = 10;
            float initialBlock = 2;
            robo.AddBlock(initialBlock);
            robo.SetCompleteLoadout(mainframe);
            //Arrange for event
            RoboInstanceData sender_onDamageAbsorbed = null;
            bool onPowerChanged = false;
            float absorbedDamage = 0;
            RoboInstanceData sender_onDamageBlocked = null;
            bool onDamageBlocked = false;
            float blockedDamage = 0;

            robo.onDamageBlocked += (s, e) =>
            {
                onDamageBlocked = true;
                sender_onDamageBlocked = s;
                blockedDamage = e.Value;
            };
            robo.onPowerChanged += (s, e) =>
            {
                onPowerChanged = true;
                sender_onDamageAbsorbed = s;
                absorbedDamage = e.Value * -1;
            };

            //Act
            robo.ApplyDamageKinetic(damegeToApply);

            //Assert
            Assert.IsTrue(onDamageBlocked, $"Event {nameof(RoboInstanceData.onDamageReducedByArmor)} was not invoked");
            Assert.IsTrue(onPowerChanged, $"Event {nameof(RoboInstanceData.onDamageAbsorbed)} was not invoked");
            Assert.AreEqual(sender_onDamageAbsorbed, robo, "Sender does not match with invoked");
            Assert.AreEqual(sender_onDamageBlocked, robo, "Sender does not match with invoked");
            Assert.AreEqual(blockedDamage,initialBlock, "Damage blocked differs with initial block points");
            Assert.AreEqual(absorbedDamage, damegeToApply - initialBlock, "Damage absorbed differs with applied");
            Assert.AreEqual(mainframe.BaseHp - damegeToApply + initialBlock, robo.CurrentPower, "CurrentPower was not updated");
            Assert.AreEqual(Mathf.Clamp(initialBlock - damegeToApply,0, initialBlock),robo.BlockPoints, "BlockPoints was not updated");
        }


    }
}

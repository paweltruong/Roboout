using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RoboMoveStatusTests
    {
        /// <summary>
        /// testing NotWaitForMove property for default RoboMoveStatus object
        /// </summary>
        [Test]
        public void MoveStatus_WaitForMove_WhenDefault()
        {
            //Arrange
            var sut = new RoboMoveStatus { Id = System.Guid.NewGuid(), Skipped = false, Dead = false, Moved = false };
            //Act
            //Assert
            Assert.IsTrue(sut.WaitingForMove);
        }
        /// <summary>
        /// testing NotWaitForMove property
        /// </summary>
        [Test]
        public void MoveStatus_NotWaitForMove_WhenDead()
        {
            //Arrange
            var sut = new RoboMoveStatus { Id = System.Guid.NewGuid(), Skipped = false, Dead = true, Moved = false };
            //Act
            //Assert
            Assert.IsFalse(sut.WaitingForMove);
        }
        /// <summary>
        /// testing NotWaitForMove property
        /// </summary>
        [Test]
        public void MoveStatus_NotWaitForMove_WhenAlreadyMoved()
        {
            //Arrange
            var sut = new RoboMoveStatus { Id = System.Guid.NewGuid(), Skipped = false, Dead = false, Moved = true };
            //Act
            //Assert
            Assert.IsFalse(sut.WaitingForMove);
        }

        /// <summary>
        /// testing NotWaitForMove property
        /// </summary>
        [Test]
        public void MoveStatus_NotWaitForMove_WhenSkipped()
        {
            //Arrange
            var sut = new RoboMoveStatus { Id = System.Guid.NewGuid(), Skipped = true, Dead = false, Moved = false };
            //Act
            //Assert
            Assert.IsFalse(sut.WaitingForMove);
        }

        /// <summary>
        /// testing NotWaitForMove property on all objects
        /// </summary>
        [Test]
        public void MoveStatus_AreAllMoved_WhenAllMoved()
        {
            //Arrange
            var sut = new BattleData();

            sut.AddSpawnedRobot(new RoboInstanceData { Id = System.Guid.NewGuid() });
            sut.AddSpawnedRobot(new RoboInstanceData { Id = System.Guid.NewGuid() });
            sut.AddSpawnedRobot(new RoboInstanceData { Id = System.Guid.NewGuid() });

            //Act
            sut.NextTurn();
            sut.FinishMove();
            sut.FinishMove();

            //Assert
            Assert.IsTrue(sut.AreAllMoved, $"Waiting for move count: {sut.RobotsToMove.Where(r => r.WaitingForMove).Count()}");
        }
    }
}

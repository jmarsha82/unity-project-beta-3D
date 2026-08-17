using NUnit.Framework;
using UnityEngine;

namespace StealthGame.Tests
{
    public class PlayerMovementTests
    {
        GameObject m_PlayerObject;
        global::PlayerMovement m_PlayerMovement;

        [SetUp]
        public void SetUp()
        {
            m_PlayerObject = new GameObject("Player");
            m_PlayerMovement = m_PlayerObject.AddComponent<global::PlayerMovement>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_PlayerObject);
        }

        [Test]
        public void NewPlayerMovement_UsesExpectedDefaultSpeeds()
        {
            Assert.That(m_PlayerMovement.walkSpeed, Is.EqualTo(1.0f));
            Assert.That(m_PlayerMovement.turnSpeed, Is.EqualTo(20f));
            Assert.That(m_PlayerMovement.speedMultiplier, Is.EqualTo(1.0f));
            Assert.That(m_PlayerMovement.CurrentMoveSpeed, Is.EqualTo(1.0f));
        }

        [Test]
        public void SetSpeedMultiplier_UpdatesCurrentMoveSpeed()
        {
            m_PlayerMovement.walkSpeed = 3f;

            m_PlayerMovement.SetSpeedMultiplier(1.7f);

            Assert.That(m_PlayerMovement.CurrentMoveSpeed, Is.EqualTo(5.1f).Within(0.001f));
        }

        [Test]
        public void SetSpeedMultiplier_ClampsNegativeValues()
        {
            m_PlayerMovement.SetSpeedMultiplier(-2f);

            Assert.That(m_PlayerMovement.speedMultiplier, Is.EqualTo(0f));
            Assert.That(m_PlayerMovement.CurrentMoveSpeed, Is.EqualTo(0f));
        }
    }
}

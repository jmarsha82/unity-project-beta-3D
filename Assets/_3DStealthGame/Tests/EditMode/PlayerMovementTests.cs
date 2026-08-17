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
        }
    }
}

using NUnit.Framework;
using UnityEngine;

namespace StealthGame.Tests
{
    public class PlayerMovementTests
    {
        GameObject m_PlayerObject;
        PlayerMovement m_PlayerMovement;

        [SetUp]
        public void SetUp()
        {
            m_PlayerObject = new GameObject("Player");
            m_PlayerMovement = m_PlayerObject.AddComponent<PlayerMovement>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_PlayerObject);
        }

        [Test]
        public void AddKey_MakesMatchingKeyOwned()
        {
            m_PlayerMovement.AddKey("attic-key");

            Assert.That(m_PlayerMovement.OwnKey("attic-key"), Is.True);
        }

        [Test]
        public void OwnKey_ReturnsFalseForUnknownKey()
        {
            m_PlayerMovement.AddKey("attic-key");

            Assert.That(m_PlayerMovement.OwnKey("cellar-key"), Is.False);
        }
    }
}

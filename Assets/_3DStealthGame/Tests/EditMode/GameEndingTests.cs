using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StealthGame.Tests
{
    public class GameEndingTests
    {
        static readonly MethodInfo k_OnTriggerEnter = typeof(global::GameEnding).GetMethod(
            "OnTriggerEnter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo k_PlayEndingAudio = typeof(global::GameEnding).GetMethod(
            "PlayEndingAudio",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_IsPlayerAtExit = typeof(global::GameEnding).GetField(
            "m_IsPlayerAtExit",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_IsPlayerCaught = typeof(global::GameEnding).GetField(
            "m_IsPlayerCaught",
            BindingFlags.Instance | BindingFlags.NonPublic);

        GameObject m_GameEndingObject;
        GameObject m_PlayerObject;
        global::GameEnding m_GameEnding;
        Collider m_PlayerCollider;

        [SetUp]
        public void SetUp()
        {
            m_GameEndingObject = new GameObject("GameEnding");
            m_GameEnding = m_GameEndingObject.AddComponent<global::GameEnding>();

            m_PlayerObject = new GameObject("Player");
            m_PlayerCollider = m_PlayerObject.AddComponent<BoxCollider>();
            m_GameEnding.player = m_PlayerObject;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_GameEndingObject);
            Object.DestroyImmediate(m_PlayerObject);
        }

        [Test]
        public void OnTriggerEnter_WithPlayer_MarksPlayerAtExit()
        {
            k_OnTriggerEnter.Invoke(m_GameEnding, new object[] { m_PlayerCollider });

            Assert.That((bool)k_IsPlayerAtExit.GetValue(m_GameEnding), Is.True);
        }

        [Test]
        public void CaughtPlayer_MarksPlayerCaught()
        {
            m_GameEnding.CaughtPlayer();

            Assert.That((bool)k_IsPlayerCaught.GetValue(m_GameEnding), Is.True);
        }

        [Test]
        public void PlayEndingAudio_WithNoSourceOrClip_DoesNotThrow()
        {
            LogAssert.Expect(LogType.Warning, "GameEnding is missing an end-level audio source or audio clip.");

            Assert.DoesNotThrow(() => k_PlayEndingAudio.Invoke(m_GameEnding, new object[] { null, null }));
        }
    }
}

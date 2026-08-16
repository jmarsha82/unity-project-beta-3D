using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace StealthGame.Tests
{
    public class GameEndingTests
    {
        static readonly MethodInfo k_OnTriggerEnter = typeof(global::StealthGame.GameEnding).GetMethod(
            "OnTriggerEnter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo k_DemoUpdateTimerLabel = typeof(global::StealthGame.GameEnding).GetMethod(
            "Demo_UpdateTimerLabel",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_IsPlayerAtExit = typeof(global::StealthGame.GameEnding).GetField(
            "m_IsPlayerAtExit",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_IsPlayerCaught = typeof(global::StealthGame.GameEnding).GetField(
            "m_IsPlayerCaught",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_DemoGameTimer = typeof(global::StealthGame.GameEnding).GetField(
            "m_Demo_GameTimer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_DemoGameTimerLabel = typeof(global::StealthGame.GameEnding).GetField(
            "m_Demo_GameTimerLabel",
            BindingFlags.Instance | BindingFlags.NonPublic);

        GameObject m_GameEndingObject;
        GameObject m_PlayerObject;
        global::StealthGame.GameEnding m_GameEnding;
        Collider m_PlayerCollider;

        [SetUp]
        public void SetUp()
        {
            m_GameEndingObject = new GameObject("GameEnding");
            m_GameEnding = m_GameEndingObject.AddComponent<global::StealthGame.GameEnding>();

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
        public void DemoUpdateTimerLabel_FormatsTimerToTwoDecimalPlaces()
        {
            var timerLabel = new Label();
            k_DemoGameTimerLabel.SetValue(m_GameEnding, timerLabel);
            k_DemoGameTimer.SetValue(m_GameEnding, 12.345f);

            k_DemoUpdateTimerLabel.Invoke(m_GameEnding, null);

            Assert.That(timerLabel.text, Is.EqualTo("12.35"));
        }
    }
}

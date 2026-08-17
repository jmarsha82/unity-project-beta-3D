using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace StealthGame.Tests
{
    public class ObserverTests
    {
        static readonly MethodInfo k_OnTriggerEnter = typeof(global::Observer).GetMethod(
            "OnTriggerEnter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo k_OnTriggerExit = typeof(global::Observer).GetMethod(
            "OnTriggerExit",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_IsPlayerInRange = typeof(global::Observer).GetField(
            "m_IsPlayerInRange",
            BindingFlags.Instance | BindingFlags.NonPublic);

        GameObject m_ObserverObject;
        GameObject m_PlayerObject;
        global::Observer m_Observer;
        Collider m_PlayerCollider;

        [SetUp]
        public void SetUp()
        {
            m_ObserverObject = new GameObject("Observer");
            m_Observer = m_ObserverObject.AddComponent<global::Observer>();

            m_PlayerObject = new GameObject("Player");
            m_PlayerCollider = m_PlayerObject.AddComponent<BoxCollider>();
            m_Observer.player = m_PlayerObject.transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_ObserverObject);
            Object.DestroyImmediate(m_PlayerObject);
        }

        [Test]
        public void OnTriggerEnter_WithPlayer_MarksPlayerInRange()
        {
            k_OnTriggerEnter.Invoke(m_Observer, new object[] { m_PlayerCollider });

            Assert.That((bool)k_IsPlayerInRange.GetValue(m_Observer), Is.True);
        }

        [Test]
        public void OnTriggerExit_WithPlayer_ClearsPlayerInRange()
        {
            k_OnTriggerEnter.Invoke(m_Observer, new object[] { m_PlayerCollider });
            k_OnTriggerExit.Invoke(m_Observer, new object[] { m_PlayerCollider });

            Assert.That((bool)k_IsPlayerInRange.GetValue(m_Observer), Is.False);
        }

        [Test]
        public void OnTriggerEnter_WithNonPlayer_DoesNotMarkPlayerInRange()
        {
            var nonPlayerObject = new GameObject("NonPlayer");
            var nonPlayerCollider = nonPlayerObject.AddComponent<BoxCollider>();

            try
            {
                k_OnTriggerEnter.Invoke(m_Observer, new object[] { nonPlayerCollider });

                Assert.That((bool)k_IsPlayerInRange.GetValue(m_Observer), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(nonPlayerObject);
            }
        }
    }
}

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace StealthGame.Tests
{
    public class WaypointPatrolTests
    {
        static readonly MethodInfo k_Start = typeof(global::StealthGame.WaypointPatrol).GetMethod(
            "Start",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo k_FixedUpdate = typeof(global::StealthGame.WaypointPatrol).GetMethod(
            "FixedUpdate",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo k_CurrentWaypointIndex = typeof(global::StealthGame.WaypointPatrol).GetField(
            "m_CurrentWaypointIndex",
            BindingFlags.Instance | BindingFlags.NonPublic);

        GameObject m_PatrolObject;
        GameObject m_FirstWaypointObject;
        GameObject m_SecondWaypointObject;
        global::StealthGame.WaypointPatrol m_Patrol;
        Rigidbody m_Rigidbody;

        [SetUp]
        public void SetUp()
        {
            m_PatrolObject = new GameObject("Patrol");
            m_Rigidbody = m_PatrolObject.AddComponent<Rigidbody>();
            m_Rigidbody.position = new Vector3(0.05f, 0f, 0f);
            m_Patrol = m_PatrolObject.AddComponent<global::StealthGame.WaypointPatrol>();

            m_FirstWaypointObject = new GameObject("Waypoint A");
            m_FirstWaypointObject.transform.position = Vector3.zero;
            m_SecondWaypointObject = new GameObject("Waypoint B");
            m_SecondWaypointObject.transform.position = new Vector3(2f, 0f, 0f);

            m_Patrol.waypoints = new[]
            {
                m_FirstWaypointObject.transform,
                m_SecondWaypointObject.transform
            };

            k_Start.Invoke(m_Patrol, null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_PatrolObject);
            Object.DestroyImmediate(m_FirstWaypointObject);
            Object.DestroyImmediate(m_SecondWaypointObject);
        }

        [Test]
        public void FixedUpdate_WhenCloseToWaypoint_AdvancesToNextWaypoint()
        {
            k_FixedUpdate.Invoke(m_Patrol, null);

            Assert.That((int)k_CurrentWaypointIndex.GetValue(m_Patrol), Is.EqualTo(1));
        }

        [Test]
        public void FixedUpdate_WhenLastWaypointReached_WrapsToFirstWaypoint()
        {
            k_CurrentWaypointIndex.SetValue(m_Patrol, 1);
            m_Rigidbody.position = new Vector3(1.95f, 0f, 0f);

            k_FixedUpdate.Invoke(m_Patrol, null);

            Assert.That((int)k_CurrentWaypointIndex.GetValue(m_Patrol), Is.EqualTo(0));
        }
    }
}

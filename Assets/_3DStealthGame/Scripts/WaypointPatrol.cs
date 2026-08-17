using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class WaypointPatrol : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public Transform[] waypoints;

    private Rigidbody m_RigidBody;
    int m_CurrentWaypointIndex;
    bool m_HasLoggedMissingWaypoints;

    void Start ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
        if (!CanPatrol())
        {
            return;
        }

        if (m_CurrentWaypointIndex >= waypoints.Length)
        {
            m_CurrentWaypointIndex = 0;
        }

        Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
        Vector3 currentToTarget = currentWaypoint.position - m_RigidBody.position;

        if (currentToTarget.magnitude < 0.1f)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            currentWaypoint = waypoints[m_CurrentWaypointIndex];
            currentToTarget = currentWaypoint.position - m_RigidBody.position;
        }

        if (currentToTarget.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion forwardRotation = Quaternion.LookRotation(currentToTarget);
        m_RigidBody.MoveRotation(forwardRotation);
        m_RigidBody.MovePosition(m_RigidBody.position + currentToTarget.normalized * moveSpeed * Time.deltaTime);
    }

    bool CanPatrol()
    {
        if (m_RigidBody == null)
        {
            m_RigidBody = GetComponent<Rigidbody>();
        }

        if (m_RigidBody == null || waypoints == null || waypoints.Length == 0)
        {
            LogMissingWaypoints();
            return false;
        }

        for (int index = 0; index < waypoints.Length; index++)
        {
            if (waypoints[index] == null)
            {
                LogMissingWaypoints();
                return false;
            }
        }

        return true;
    }

    void LogMissingWaypoints()
    {
        if (m_HasLoggedMissingWaypoints)
        {
            return;
        }

        Debug.LogWarning($"{name} has a WaypointPatrol component but no complete waypoint list assigned.", this);
        m_HasLoggedMissingWaypoints = true;
    }
}

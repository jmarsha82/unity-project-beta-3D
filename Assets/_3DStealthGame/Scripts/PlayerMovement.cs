using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float speedMultiplier = 1.0f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    Animator m_Animator;

    public float CurrentMoveSpeed => walkSpeed * speedMultiplier;

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool isWalking = m_Movement.sqrMagnitude > 0.001f;

        if (isWalking)
        {
            Vector3 desiredForward = Vector3.RotateTowards(
                transform.forward,
                m_Movement,
                turnSpeed * Time.fixedDeltaTime,
                0f);

            m_Rotation = Quaternion.LookRotation(desiredForward);
            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * CurrentMoveSpeed * Time.fixedDeltaTime);
        }

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        // bool isWalking = hasHorizontalInput || hasVerticalInput;
        if (m_Animator != null)
        {
            m_Animator.SetBool("IsWalking", isWalking);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [Header("Top Down Controller")]
    public float speed = 2;
    [Range(1,99)]public float speedFactor = 1;
    public float acceleration = 1;
    public float deceleration = 1;
    public float maxVelocityChange = 5;
    [Range(0, 1f)] public float smoothTime = 0.1f;


    [Header("Events")]
    public UnityEvent<Vector2> OnMovementEvent = new UnityEvent<Vector2>();

    public bool isMoving { get { return m_inputDirection.magnitude > 0; } }
    public Vector2 direction { get { return m_inputDirection; } }
    public Vector2 velocity { get { return m_velocity; } }
    public Vector2 bodyVelocity { get { return m_body.velocity; } }

    private Rigidbody2D m_body;
    private Vector2 m_inputDirection;
    private Vector2 m_smoothDirection, m_smoothVelocity;
    private Vector2 m_velocity;
    private Animator m_animator;

    private void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        float t = Time.fixedDeltaTime;

        m_smoothDirection = Vector2.SmoothDamp(
            m_smoothDirection,
            m_inputDirection,
            ref m_smoothVelocity,
            smoothTime
        );

        float totalSpeed = speed * speedFactor;

        // Calculamos la velocidad actual del Rigidbody2D en ambos ejes.
        float currentSpeedX = m_body.velocity.x;
        float currentSpeedY = m_body.velocity.y;

        // Calculamos la velocidad objetivo usando la entrada del jugador.
        float targetSpeedX = m_smoothDirection.x * totalSpeed;
        float targetSpeedY = m_smoothDirection.y * totalSpeed;

        // Calculamos la aceleración requerida para alcanzar la velocidad objetivo en ambos ejes.
        float accelx = (targetSpeedX - currentSpeedX) * acceleration;
        float accely = (targetSpeedY - currentSpeedY) * acceleration;

        accelx = Mathf.Clamp(accelx, -maxVelocityChange, maxVelocityChange);
        accely = Mathf.Clamp(accely, -maxVelocityChange, maxVelocityChange);


        // Aplicamos la aceleración necesaria al Rigidbody2D en ambos ejes.
        m_velocity = new Vector2(accelx, accely);
        m_body.AddForce(m_velocity, ForceMode2D.Force);

        
    }

    private void OnMovement(InputValue value)
    {
        m_inputDirection = value.Get<Vector2>();
        m_inputDirection.Normalize();
        OnMovementEvent.Invoke(m_inputDirection);
    }

    private void Update()
    {
        m_animator.SetBool("isMoving", isMoving);
        m_animator.SetFloat("Horizontal", direction.x);
        m_animator.SetFloat("Vertical", direction.y);
    }
}

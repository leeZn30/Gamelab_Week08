using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField][Tooltip("Maximum movement speed")]                     public float maxSpeed = 10f;
    [SerializeField][Tooltip("How fast to reach max speed")]                public float maxAcceleration = 52f;
    [SerializeField][Tooltip("How fast to stop after letting go")]          public float maxDecceleration = 52f;
    [SerializeField][Tooltip("How fast to stop when changing direction")]   public float maxTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction = 0f;

    [Header("Calculations")]
    public float direction;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;

    bool pressingKey = false;
    public float lastLookDirection;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != 0f) 
            pressingKey = true;
        else
            pressingKey = false;

        desiredVelocity = new Vector2(direction, 0f) * Mathf.Max(maxSpeed - friction, 0f);
    }

    private void FixedUpdate()
    {
        if (_playerController.playerContext.GetState().GetType() == typeof(MoveState)
            || _playerController.playerContext.GetState().GetType() == typeof(IdleState))
        {
            if (_playerController.playerContext.GetState().GetType() == typeof(MoveState))
                _animator.SetBool("isMoving", true);
            else if(_playerController.playerContext.GetState().GetType() == typeof(IdleState))
                _animator.SetBool("isMoving", false);

            transform.localScale = new Vector3(direction > 0f ? 0.5f : -0.5f, 0.5f, 0.5f);

            if (pressingKey)
            {
                if (Mathf.Sign(direction) != Mathf.Sign(velocity.x))
                    maxSpeedChange = maxTurnSpeed * 0.02f;
                else
                    maxSpeedChange = maxAcceleration * 0.02f;
            }
            else
                maxSpeedChange = maxDecceleration * 0.02f;

            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            _body.velocity = velocity;
        }
        else
            _body.velocity = Vector2.zero;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            _playerController.playerContext.CanPlayerMove(); // MoveState로 변경 가능한지 확인
            direction = context.ReadValue<float>();
            lastLookDirection = Mathf.Sign(direction);
        }
        else
        {
            direction = 0f;
            if (_playerController.playerContext.GetState().GetType() == typeof(MoveState))
                _playerController.playerContext.CanPlayerIdle();
        }
    }
}

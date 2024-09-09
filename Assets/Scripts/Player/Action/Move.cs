using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction = 0f;

    [Header("Calculations")]
    public float direction;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;

    public float playerScale;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;

    bool pressingKey = false;
    public float lastLookDirection;
    float sprintVariable = 1f;
    public float sprintConstant = 2f;
    bool shiftPress = false;
    public float sprintStaminaPerFrame = 0.2f; // 20 stamina per second

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        playerScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != 0f)
            pressingKey = true;
        else
            pressingKey = false;

        desiredVelocity = new Vector2(direction, 0f) * Mathf.Max(maxSpeed * sprintVariable - friction, 0f);
    }

    private void FixedUpdate()
    {
        if (_playerController.playerContext.GetState().GetType() == typeof(MoveState)
            || _playerController.playerContext.GetState().GetType() == typeof(IdleState))
        {
            if (direction != 0f) // input�� �ִ� ���¶��
                _playerController.playerContext.CanPlayerMove(); // MoveState�� ���� �������� �ѹ� �� Ȯ��

            if (_playerController.playerContext.GetState().GetType() == typeof(MoveState))
                _animator.SetBool("isMoving", true);
            else if (_playerController.playerContext.GetState().GetType() == typeof(IdleState))
                _animator.SetBool("isMoving", false);

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
            velocity.y = _body.velocity.y;
            _body.velocity = velocity;
        }
        else
        {
            velocity.x = 0f;
            velocity.y = _body.velocity.y;
            _body.velocity = velocity;
        }

        if(_body.velocity.x != 0f) // moving right now
            transform.localScale = new Vector3(lastLookDirection > 0f ? -playerScale : playerScale, playerScale, playerScale);

        if (shiftPress) // sprint state
        {
            if (_playerController.currentStamina > sprintStaminaPerFrame && _playerController.usingStamina)
            {
                _playerController.currentStamina -= sprintStaminaPerFrame;
                sprintVariable = sprintConstant;
            }
            else
            {
                _playerController.usingStamina = false;
                sprintVariable = 1f;
            }
        }
        else
            sprintVariable = 1f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            Debug.Log("Move Input");
            _playerController.playerContext.CanPlayerMove(); // MoveState�� ���� �������� Ȯ��
            direction = context.ReadValue<float>();
            lastLookDirection = Mathf.Sign(direction);
        }
        else if (context.canceled)
        {
            direction = 0f;
            if (_playerController.playerContext.GetState().GetType() == typeof(MoveState))
                _playerController.playerContext.CanPlayerIdle();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shiftPress = true;
            _playerController.usingStamina = true;
        }
        else if (context.canceled)
        {
            shiftPress = false;
            _playerController.usingStamina = false;
        }
    }
}

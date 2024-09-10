using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dodge : MonoBehaviour
{
    public float DodgeDistance = 3f;
    public float dodgeTimerLimit = 0.5f;
    public float dodgeStamina = 20f;

    private bool pressButton = false;
    private bool buttonBuffer = false;
    private bool alreadyDodging = false;
    private float _dodgeTimer = 0f;
    private float _dodgeBuffer = 0f;
    private float _dodgeBufferLimit = 0.15f;

    private bool pressStart = false;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;
    private Move _move;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _move = GetComponent<Move>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonBuffer)
        {
            _dodgeBuffer += Time.deltaTime;
            if (_dodgeBuffer > _dodgeBufferLimit)
            {
                buttonBuffer = false;
                _dodgeBuffer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (buttonBuffer)
            _playerController.playerContext.CanPlayerDodge();

        if (_playerController.playerContext.GetState().GetType() == typeof(DodgeState))
        {
            if (!alreadyDodging && (pressButton || buttonBuffer))
            {
                pressButton = false;
                buttonBuffer = false;

                if (_playerController.currentStamina < dodgeStamina)
                {
                    _playerController.playerContext.ChangeState(IdleState.getInstance());
                    return;
                }

                alreadyDodging = true;
                _playerController.currentStamina -= dodgeStamina;
                _playerController.usingStamina = true;
                StartCoroutine(PlayerDodge());
                
            }
        }
    }

    IEnumerator PlayerDodge()
    {
        yield return null;
        Debug.Log("Dodge Start");
        GetComponent<CapsuleCollider2D>().excludeLayers = (int)Mathf.Pow(2, LayerMask.NameToLayer("Boss"));
        _animator.SetBool("dodging", true);
        _animator.SetBool("walkToDodge", true);
        _animator.SetBool("isMoving", false);
        _animator.SetBool("doAttack", false);
        _animator.SetBool("normalAttackMaintain", false);
        _animator.SetBool("chargeAttackMaintain", false);
        _animator.SetBool("fullChargeAttackMaintain", false);
        float positiveDirection = _move.lastLookDirection; // (_body.velocity.x == 0 ? _move.lastLookDirection : Mathf.Sign(_body.velocity.x));
        transform.localScale = new Vector3(positiveDirection > 0f ? -_move.playerScale : _move.playerScale, _move.playerScale, _move.playerScale);
        Vector2 destination = new Vector2((positiveDirection > 0f ? _body.position.x + DodgeDistance : _body.position.x - DodgeDistance), _body.position.y);
        _body.velocity = Vector2.zero; // ���� �ӵ� �ʱ�ȭ
        Vector2 pos = _body.position;

        _dodgeTimer = 0f;
        while (Mathf.Abs(pos.x - destination.x) > 0.01f && _dodgeTimer < dodgeTimerLimit)
        {
            pos = _body.position;
            _body.position = new Vector2(Mathf.MoveTowards(pos.x, destination.x, Time.deltaTime * DodgeDistance), pos.y);
            yield return null;
            if(_playerController.usingStamina)
            {
                _playerController.usingStamina = false;
            }
            _dodgeTimer += Time.deltaTime;
        }

        Debug.Log("Dodge End");
        _animator.SetBool("dodging", false);
        _animator.SetBool("walkToDodge", false);
        _playerController.playerContext.CanPlayerIdle();
        GetComponent<CapsuleCollider2D>().excludeLayers = (int)Mathf.Pow(2, LayerMask.NameToLayer("Nothing"));
        alreadyDodging = false;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!alreadyDodging)
        {
            if (context.started || context.performed)
                pressStart = true;
            if (context.canceled && !pressStart)
                return;

            pressStart = false;
            _playerController.playerContext.CanPlayerDodge();
            pressButton = context.ReadValueAsButton();

            if(!buttonBuffer)
                _dodgeBuffer = 0f;
            buttonBuffer = true;
        }
        else
        {
            if(context.performed)
            {
                if (!buttonBuffer)
                    _dodgeBuffer = 0f;
                buttonBuffer = true;
            }
        }
    }
}

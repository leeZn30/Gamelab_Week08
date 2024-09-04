using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dodge : MonoBehaviour
{
    public float DodgeDistance = 3f;
    public float dodgeTimerLimit = 0.8f;

    private bool pressButton = false;
    private bool buttonBuffer = false;
    private bool alreadyDodging = false;
    private float _dodgeTimer = 0f;
    private float _dodgeBuffer = 0f;
    private float _dodgeBufferLimit = 0.15f;

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
        if(buttonBuffer)
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
        if (_playerController.playerContext.GetState().GetType() == typeof(DodgeState))
        {
            if (pressButton || buttonBuffer)
            {
                alreadyDodging = true;
                StartCoroutine(PlayerDodge());
                pressButton = false;
                buttonBuffer = false;
            }
        }   
    }

    IEnumerator PlayerDodge()
    {
        Debug.Log("Dodge Start");
        GetComponent<CapsuleCollider2D>().excludeLayers = (int)Mathf.Pow(2, LayerMask.NameToLayer("Boss"));
        _animator.SetBool("dodging", true);
        _animator.SetBool("walkToDodge", true);
        _animator.SetBool("isMoving", false);
        float positiveDirection = (_body.velocity.x == 0 ? _move.lastLookDirection : Mathf.Sign(_body.velocity.x));
        Vector2 destination = new Vector2((positiveDirection > 0f? _body.position.x + DodgeDistance : _body.position.x - DodgeDistance), _body.position.y);
        _body.velocity = Vector2.zero; // 기존 속도 초기화
        Vector2 pos = _body.position;

        _dodgeTimer = 0f;
        while (Mathf.Abs(pos.x - destination.x) > 0.01f && _dodgeTimer < _dodgeTimerLimit)
        {
            pos = _body.position;
            _body.position = new Vector2(Mathf.MoveTowards(pos.x, destination.x, Time.deltaTime * DodgeDistance), pos.y);
            yield return null;
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
        _playerController.playerContext.CanPlayerDodge();
        if (!alreadyDodging)
        {
            pressButton = context.ReadValueAsButton();

            buttonBuffer = true;
            _dodgeBuffer = 0f;
        }
    }
}

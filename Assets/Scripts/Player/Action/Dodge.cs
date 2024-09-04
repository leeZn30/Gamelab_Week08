using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dodge : MonoBehaviour
{
    public float DodgeDistance = 3f;

    private bool pressButton = false;
    private bool alreadyDodging = false;

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
        
    }

    private void FixedUpdate()
    {
        if (_playerController.playerContext.GetState().GetType() == typeof(DodgeState))
        {
            if (pressButton)
            {
                alreadyDodging = true;
                StartCoroutine(PlayerDodge());
                pressButton = false;
            }
        }   
    }

    IEnumerator PlayerDodge()
    {
        Debug.Log("Dodge Start");
        _animator.SetBool("Dodge", true);
        _animator.SetBool("isMoving", false);
        float positiveDirection = (_body.velocity.x == 0 ? _move.lastLookDirection : Mathf.Sign(_body.velocity.x));
        Vector2 destination = new Vector2((positiveDirection > 0f? _body.position.x + DodgeDistance : _body.position.x - DodgeDistance), _body.position.y);
        _body.velocity = Vector2.zero; // 기존 속도 초기화
        Vector2 pos = _body.position;

        while (Mathf.Abs(pos.x - destination.x) > 0.01f)
        {
            pos = _body.position;
            _body.position = new Vector2(Mathf.MoveTowards(pos.x, destination.x, Time.deltaTime * 15), pos.y);
            yield return null;
        }

        alreadyDodging = false;
        Debug.Log("Dodge End");
        _animator.SetBool("Dodge", false);
        _playerController.playerContext.CanPlayerIdle();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        _playerController.playerContext.CanPlayerDodge();
        if (!alreadyDodging)
        {
            pressButton = context.ReadValueAsButton();
        }
    }
}

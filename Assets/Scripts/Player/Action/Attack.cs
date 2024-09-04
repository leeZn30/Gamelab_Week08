using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    private float pressingTime = 0f;
    private bool pressingButton = false;
    private bool alreadyAttacking = false;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;

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
        if (_playerController.playerContext.GetState().GetType() == typeof(AttackState))
        {
            if (pressingButton)
            {
                _body.velocity = Vector2.zero;
                pressingTime += Time.deltaTime;
                _animator.SetBool("isAttack", true);
                _animator.SetBool("isMoving", false);
            }
            else
            {
                if (pressingTime > 0f && pressingTime <= 1f)
                {
                    pressingTime = 0f;
                    StartCoroutine(DoBasicAttack());
                }
                else if (pressingTime > 1f && pressingTime <= 2.5f)
                {
                    pressingTime = 0f;
                    StartCoroutine(DoChargeAttack());
                }
                else if (pressingTime > 2.5f)
                {
                    pressingTime = 0f;
                    StartCoroutine(DoFullChargeAttack());
                }
            }
        } 
    }

    IEnumerator DoBasicAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Basic Attack");
        _animator.SetBool("isAttack", false);

        yield return new WaitForSeconds(0.5f);
        _playerController.playerContext.CanPlayerIdle();
        alreadyAttacking = false;
    }

    IEnumerator DoChargeAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Charge Attack");
        _animator.SetBool("isAttack", false);

        yield return new WaitForSeconds(0.5f);
        _playerController.playerContext.CanPlayerIdle();
        alreadyAttacking = false;
    }

    IEnumerator DoFullChargeAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Full Charge Attack");
        _animator.SetBool("isAttack", false);

        yield return new WaitForSeconds(0.5f);
        _playerController.playerContext.CanPlayerIdle();
        alreadyAttacking = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _playerController.playerContext.CanPlayerAttack();
        if(!alreadyAttacking)
        {
            pressingButton = context.ReadValueAsButton();
        }
    }
}

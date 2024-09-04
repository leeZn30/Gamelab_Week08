using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerContext playerContext;
    public int currentHP = 2;
    private Vector3 pos;

    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        playerContext = new PlayerContext();
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if(playerContext.GetState().GetType() == typeof(DeadState))
        {
            if(context.started)
            {
                _animator.SetBool("isMoving", false);
                _animator.SetBool("dodging", false);
                _animator.SetBool("walkToDodge", false);
                _animator.SetBool("doAttack", false);
                _animator.SetBool("doNormalAttack", false);
                _animator.SetBool("doChargeAttack", false);
                _animator.SetBool("doFullChargeAttack", false);
                _animator.SetBool("beDamaged", false);
                _animator.SetBool("isDead", false);
                _animator.SetBool("Corpse", false);

                currentHP = 2;
                transform.position = pos;
                _animator.Play("idle");
                playerContext.ChangeState(IdleState.getInstance());


            }
        }
    }
}

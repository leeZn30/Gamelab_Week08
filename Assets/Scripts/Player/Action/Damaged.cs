using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Damaged : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;

    private float hurtDelay = 0f;
    private float hurtDelayLimit = 0.55f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hurtDelay < hurtDelayLimit)
            hurtDelay += Time.deltaTime;
    }

    IEnumerator DamageDelay()
    {
        hurtDelay = 0f;
        _playerController.playerContext.ChangeState(DamagedState.getInstance()); // 강제 데미지 상태로 변경
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("beDamaged", false);
        _playerController.playerContext.CanPlayerIdle();
    }

    // Test 코드 : 강제로 Damage를 입히는 코드, 추후 아래 Trigger나 ColliderEnter로 변경할 것 
    public void OnSelfDamage(InputAction.CallbackContext context)
    {
        if (hurtDelay < hurtDelayLimit)
            return; // 피격 쿨타임

        if(context.started)
        {
            _playerController.playerContext.CanPlayerDamaged();

            if (_playerController.playerContext.GetInvincible()) 
                return;
            else // 무적이 아니면 일단 맞는다.
            {
                _playerController.currentHP -= 1;
                if (_playerController.currentHP <= 0)
                {
                    _playerController.playerContext.CanPlayerDied();
                    // 죽는 애니메이션
                    _animator.SetBool("isMoving", false);
                    _animator.SetBool("dodging", false);
                    _animator.SetBool("walkToDodge", false);
                    _animator.SetBool("doAttack", false);
                    _animator.SetBool("doNormalAttack", false);
                    _animator.SetBool("doChargeAttack", false);
                    _animator.SetBool("doFullChargeAttack", false);
                    _animator.SetBool("beDamaged", false);

                    _animator.SetBool("isDead", true);
                    StartCoroutine(StateChangeforDeath());
                    // 일정 시간 후 리스폰

                    return;
                }

                if (_playerController.playerContext.GetHurtEffect()) // 피격 이펙트가 있어야 한다.
                {
                    _animator.SetBool("isMoving", false);
                    _animator.SetBool("dodging", false);
                    _animator.SetBool("walkToDodge", false);
                    _animator.SetBool("doAttack", false);
                    _animator.SetBool("doNormalAttack", false);
                    _animator.SetBool("doChargeAttack", false);
                    _animator.SetBool("doFullChargeAttack", false);

                    _animator.SetBool("beDamaged", true);
                    // 타격 당하는 애니메이션 필요
                    StartCoroutine(DamageDelay());
                }
            }
        }
    }
    //

    IEnumerator StateChangeforDeath()
    {
        yield return new WaitForSeconds(1.5f);

        _animator.SetBool("isDead", false);
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            _animator.SetBool("Corpse", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}

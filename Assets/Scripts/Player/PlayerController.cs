using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerContext playerContext;
    public int currentHP = 2;
    private Vector3 pos;

    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaConstant = 10f;

    private Animator _animator;
    [SerializeField] private GameObject _hammerCollider;
    [SerializeField] private GameObject _StaminaSlider;
    [SerializeField] private GameObject _HpSprite1;
    [SerializeField] private GameObject _HpSprite2;
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
        switch(currentHP)
        {
            case 2:
                _HpSprite1.SetActive(true);
                _HpSprite2.SetActive(true);
                break;
            case 1:
                _HpSprite1.SetActive(true);
                _HpSprite2.SetActive(false);
                break;
            case 0:
                _HpSprite1.SetActive(false);
                _HpSprite2.SetActive(false);
                break;
            default:
                _HpSprite1.SetActive(false);
                _HpSprite2.SetActive(false);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentStamina < maxStamina)
            currentStamina += Time.fixedDeltaTime * staminaConstant;

        _StaminaSlider.GetComponent<Slider>().value = currentStamina;
    }

    public void TurnOffHammerCollider()
    {
        _hammerCollider.SetActive(false);
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
                currentStamina = maxStamina;
                transform.position = pos;
                _animator.Play("idle");
                playerContext.ChangeState(IdleState.getInstance());
            }
        }
    }
}

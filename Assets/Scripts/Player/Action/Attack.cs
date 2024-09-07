using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Attack;

public class Attack : MonoBehaviour
{
    public float pressingTime = 0f;
    public bool pressingButton = false;
    public bool buttonBuffer = false;
    public bool attackByUsingBuffer = false;
    private bool alreadyAttacking = false;
    private float _attackBuffer = 0f;
    private float _attackBufferLimit = 0.15f;

    public float attackReadyStamina = 10f;
    public float normalAttackStamina = 10f;
    public float chargeAttackStamina = 15f;
    public float fullChargeAttackStamina = 20f;

    public bool pressStart = false;
    private bool canDoNextComboAttack = false;
    private bool doNextComboAttack = false;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;
    private CinemachineVirtualCamera _virtualCamera;
    CinemachineBasicMultiChannelPerlin _perlin;

    [SerializeField]
    private GameObject _hammerCollider;

    public enum AttackVariable
    {
        None, Normal, Charge, FullCharge
    }

    public AttackVariable attackVariable { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (buttonBuffer)
        {
            _attackBuffer += Time.deltaTime;
            if (_attackBuffer > _attackBufferLimit)
            {
                buttonBuffer = false;
                _attackBuffer = 0f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (buttonBuffer)
            _playerController.playerContext.CanPlayerAttack();

        if (_playerController.playerContext.GetState().GetType() == typeof(AttackState))
        {
            if (!alreadyAttacking && (pressingButton || buttonBuffer))
            {
                if (buttonBuffer && !pressingButton)
                    attackByUsingBuffer = true;

                buttonBuffer = false;

                if (_playerController.currentStamina < attackReadyStamina + normalAttackStamina)
                {
                    _playerController.playerContext.ChangeState(IdleState.getInstance());
                    return;
                }
                else if(_playerController.currentStamina > attackReadyStamina + normalAttackStamina && _playerController.currentStamina < attackReadyStamina + chargeAttackStamina
                    && pressingTime > 1f)
                {
                    pressingButton = false;
                    pressingTime = 1f;
                }
                else if(_playerController.currentStamina > attackReadyStamina + chargeAttackStamina && _playerController.currentStamina < attackReadyStamina + fullChargeAttackStamina
                    && pressingTime > 2.5f)
                {
                    pressingButton = false;
                    pressingTime = 2.5f;
                }
                    

                _body.velocity = Vector2.zero;

                pressingTime += Time.deltaTime;

                _animator.SetBool("doAttack", true);
                if (pressingTime >= 5f)
                    pressingButton = false;
            }
            else
            {
                if (!pressStart && !attackByUsingBuffer)
                    return;

                _animator.SetBool("isMoving", false);
                if (pressingTime > 0f && pressingTime <= 1f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + normalAttackStamina;
                    _playerController.usingStamina = true;
                    StartCoroutine(DoBasicAttack());
                }
                else if (pressingTime > 1f && pressingTime <= 2.5f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + chargeAttackStamina;
                    _playerController.usingStamina = true;
                    StartCoroutine(DoChargeAttack());
                }
                else if (pressingTime > 2.5f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + fullChargeAttackStamina;
                    _playerController.usingStamina = true;
                    StartCoroutine(DoFullChargeAttack());
                }
                else
                {
                    if(!alreadyAttacking) // 공격 버튼도 눌리지 않은 상태인데, 눌린 시간조차 0초이고, 현재 공격 중도 아닌데 Attack State다? -> 입력이 너무 빨리 들어와 State 변경이 원활하지 못함
                        _playerController.playerContext.CanPlayerIdle();
                }
            }
        }
        else
        {
            pressStart = false;
            //buttonBuffer = false;
            pressingButton = false;
            pressingTime = 0f;
        }
    }

    IEnumerator DoBasicAttack()
    {
        attackVariable = AttackVariable.Normal;

        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Basic Attack");
        _animator.SetBool("doNormalAttack", true);
        _animator.SetBool("doAttack", false);
        _playerController.playerContext.CanPlayerHit();

        yield return new WaitForSeconds(0.1f);
        _playerController.usingStamina = false;
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 0.25f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.1f);
        _playerController.TurnOffHammerCollider();
        yield return new WaitForSeconds(0.1f);
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        // combo attack 
        float deltaTime = 0f;
        canDoNextComboAttack = true;
        while(deltaTime < 0.5f)
        {
            if (doNextComboAttack)
            {
                if (_playerController.currentStamina >= normalAttackStamina)
                {
                    doNextComboAttack = false; // if you want to remove combo attack buffer, take this code in front of the this "if" code;
                    _playerController.currentStamina -= normalAttackStamina * 1.2f;
                    _playerController.usingStamina = true;
                    StartCoroutine(DoBasicAttack_Combo());
                    yield break;
                }
            }
            deltaTime += Time.deltaTime;
            yield return null;
        }
        canDoNextComboAttack = false;
        //yield return new WaitForSeconds(0.5f);
        
        doNextComboAttack = false;
        _animator.SetBool("doNormalAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }
    
    IEnumerator DoBasicAttack_Combo()
    {
        canDoNextComboAttack = false;
        Debug.Log("State : Basic Attack Combo");
        //_animator.SetBool("doNormalAttack", true); // must change to "doComboNormalAttack" animation trigger
        //_animator.SetBool("doAttack", false); //  must change from "doAttack" to "doNormalAttack"
        _animator.Play("normalAttack", 0, 0f);
        _playerController.playerContext.CanPlayerHit();

        yield return new WaitForSeconds(0.1f);
        _playerController.usingStamina = false;
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 0.25f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.1f);
        _playerController.TurnOffHammerCollider();
        yield return new WaitForSeconds(0.1f);
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        float deltaTime = 0f;
        canDoNextComboAttack = true;
        while (deltaTime < 0.5f)
        {
            if (doNextComboAttack)
            {
                if(_playerController.currentStamina >= normalAttackStamina)
                {
                    doNextComboAttack = false; // if you want to remove combo attack buffer, take this code in front of the this "if" code;
                    _playerController.currentStamina -= normalAttackStamina * 1.2f;
                    _playerController.usingStamina = true;
                    StartCoroutine(DoBasicAttack_LastCombo());
                    yield break;
                }
            }
            deltaTime += Time.deltaTime;
            yield return null;
        }
        canDoNextComboAttack = false;
        //yield return new WaitForSeconds(0.5f);

        doNextComboAttack = false;
        _animator.SetBool("doNormalAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }

    IEnumerator DoBasicAttack_LastCombo()
    {
        canDoNextComboAttack = false;
        Debug.Log("State : Basic Attack Last");
        //_animator.SetBool("doNormalAttack", true); // must change to "doLastNormalAttack" animation trigger
        //_animator.SetBool("doAttack", false); //  must change from "doAttack" to "doComboNormalAttack"
        _animator.Play("normalAttack", 0, 0f);
        _playerController.playerContext.CanPlayerHit();

        yield return new WaitForSeconds(0.1f);
        _playerController.usingStamina = false;
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 0.25f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.1f);
        _playerController.TurnOffHammerCollider();
        yield return new WaitForSeconds(0.1f);
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;
        yield return new WaitForSeconds(0.5f);

        _animator.SetBool("doNormalAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }

    IEnumerator DoChargeAttack()
    {
        attackVariable = AttackVariable.Charge;

        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Charge Attack");
        _animator.SetBool("doChargeAttack", true);
        _animator.SetBool("doAttack", false);
        _playerController.playerContext.CanPlayerHit();

        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _playerController.usingStamina = false;
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 0.6f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.1f);
        _playerController.TurnOffHammerCollider();
        yield return new WaitForSeconds(0.1f);
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;
        yield return new WaitForSeconds(0.7f);

        _animator.SetBool("doChargeAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }

    IEnumerator DoFullChargeAttack()
    {
        attackVariable = AttackVariable.FullCharge;

        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Full Charge Attack");
        _animator.SetBool("doFullChargeAttack", true);
        _animator.SetBool("doAttack", false);
        _playerController.playerContext.CanPlayerHit();

        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _playerController.usingStamina = false;
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 1f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.1f);
        _playerController.TurnOffHammerCollider();
        yield return new WaitForSeconds(0.1f);
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        yield return new WaitForSeconds(0.9f);
        _animator.SetBool("doFullChargeAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!alreadyAttacking) // start to Attack
        {
            doNextComboAttack = false;
            if (context.started || context.performed)
                pressStart = true;
            if (context.canceled && !pressStart)
                return;

            _playerController.playerContext.CanPlayerAttack();
            if(_playerController.playerContext.GetState().GetType() == typeof(AttackState))
                pressingButton = context.ReadValueAsButton();

            if(!buttonBuffer)
                _attackBuffer = 0f;
            buttonBuffer = true;
        }
        else // already attacking
        {
            if(attackVariable == AttackVariable.Normal && context.started && canDoNextComboAttack) // do normal attack
            {
                doNextComboAttack = true;
            }
            else
            {
                if(context.performed)
                {
                    if(!buttonBuffer)
                        _attackBuffer = 0f;
                    buttonBuffer = true;
                }
            }
        }
    }
}

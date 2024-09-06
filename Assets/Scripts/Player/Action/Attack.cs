using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    private float pressingTime = 0f;
    private bool pressingButton = false;
    private bool buttonBuffer = false;
    private bool alreadyAttacking = false;
    private float _attackBuffer = 0f;
    private float _attackBufferLimit = 0.15f;

    public float attackReadyStamina = 10f;
    public float normalAttackStamina = 10f;
    public float chargeAttackStamina = 20f;
    public float fullChargeAttackStamina = 40f;

    private bool pressStart = false;

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
        if (_playerController.playerContext.GetState().GetType() == typeof(AttackState))
        {
            if (pressingButton || buttonBuffer)
            {
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
                if (!pressStart)
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
                    if(!alreadyAttacking) // ���� ��ư�� ������ ���� �����ε�, ���� �ð����� 0���̰�, ���� ���� �ߵ� �ƴѵ� Attack State��? -> �Է��� �ʹ� ���� ���� State ������ ��Ȱ���� ����
                        _playerController.playerContext.CanPlayerIdle();
                }
            }
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
        yield return new WaitForSeconds(0.3f);

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
        yield return new WaitForSeconds(0.5f);

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

        yield return new WaitForSeconds(0.7f);
        _animator.SetBool("doFullChargeAttack", false);
        alreadyAttacking = false;
        attackVariable = AttackVariable.None;
        pressStart = false;
        _playerController.playerContext.CanPlayerIdle();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!alreadyAttacking)
        { 
            if(context.started || context.performed)
                pressStart = true;
            if (context.canceled && !pressStart)
                return;

            _playerController.playerContext.CanPlayerAttack();
            pressingButton = context.ReadValueAsButton();

            buttonBuffer = true;
            _attackBuffer = 0f;
        }
    }
}

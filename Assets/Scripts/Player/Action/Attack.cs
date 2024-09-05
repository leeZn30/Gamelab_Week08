using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
    public float chargeAttackStamina = 15f;
    public float fullChargeAttackStamina = 20f;

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerController _playerController;
    private CinemachineVirtualCamera _virtualCamera;
    CinemachineBasicMultiChannelPerlin _perlin;

    [SerializeField]
    private GameObject _hammerCollider;

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
                if (pressingTime >= 3f)
                    pressingButton = false;
            }
            else
            {
                _animator.SetBool("isMoving", false);
                if (pressingTime > 0f && pressingTime <= 1f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + normalAttackStamina;
                    StartCoroutine(DoBasicAttack());
                }
                else if (pressingTime > 1f && pressingTime <= 2.5f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + chargeAttackStamina;
                    StartCoroutine(DoChargeAttack());
                }
                else if (pressingTime > 2.5f)
                {
                    pressingTime = 0f;
                    _playerController.currentStamina -= attackReadyStamina + fullChargeAttackStamina;
                    StartCoroutine(DoFullChargeAttack());
                }
                else
                {
                    if(!alreadyAttacking) // 공격 버튼도 눌리지 않은 상태인데, 눌린 시간조차 0초이고, 현재 공격 중도 아닌데 Attack State다? -> 입력이 너무 빨리 들어와 State 변경이 원활하지 못함
                        _playerController.playerContext.CanPlayerIdle();
                }
            }
        } 
    }

    IEnumerator DoBasicAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Basic Attack");
        _animator.SetBool("doNormalAttack", true);
        _animator.SetBool("doAttack", false);

        yield return new WaitForSeconds(0.1f);
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _perlin.m_AmplitudeGain = 0.3f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.2f);
        _playerController.TurnOffHammerCollider();
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        yield return new WaitForSeconds(0.9f);
        _playerController.playerContext.CanPlayerIdle();
        _animator.SetBool("doNormalAttack", false);
        alreadyAttacking = false;
    }

    IEnumerator DoChargeAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Charge Attack");
        _animator.SetBool("doChargeAttack", true);
        _animator.SetBool("doAttack", false);
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        _perlin.m_AmplitudeGain = 0.6f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.2f);
        _playerController.TurnOffHammerCollider();
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        yield return new WaitForSeconds(0.9f);
        _playerController.playerContext.CanPlayerIdle();
        _animator.SetBool("doChargeAttack", false);
        alreadyAttacking = false;
    }

    IEnumerator DoFullChargeAttack()
    {
        _body.velocity = Vector3.zero;
        alreadyAttacking = true;
        Debug.Log("State : Full Charge Attack");
        _animator.SetBool("doFullChargeAttack", true);
        _animator.SetBool("doAttack", false);
        _hammerCollider.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        _perlin.m_AmplitudeGain = 0.9f;
        _perlin.m_FrequencyGain = 1f;
        yield return new WaitForSeconds(0.2f);
        _playerController.TurnOffHammerCollider();
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;

        yield return new WaitForSeconds(0.9f);
        _playerController.playerContext.CanPlayerIdle();
        _animator.SetBool("doFullChargeAttack", false);
        alreadyAttacking = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _playerController.playerContext.CanPlayerAttack();
        if(!alreadyAttacking)
        {
            pressingButton = context.ReadValueAsButton();

            buttonBuffer = true;
            _attackBuffer = 0f;
        }
    }
}

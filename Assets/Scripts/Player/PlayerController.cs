using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerContext playerContext;
    public float currentHP = 2;
    private float MaxHp;
    private Vector3 pos;
    public bool usingStamina = false;
    private bool staminaRegain = true;

    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaConstant = 10f;

    public float normalAttackDamage = 10f;
    public float normalAttackComboDamage = 6f;
    public float normalAttackLastComboDamage = 15f;
    public float chargeAttackDamage = 20f;
    public float fullChargeAttackDamage = 40f;

    private Animator _animator;
    private GameObject _canvas;
    private Vector3 _hpIconInitPos = new Vector3(-900f, 480f, 0f);
    [SerializeField] private GameObject _hammerCollider;
    [SerializeField] private GameObject _StaminaSlider;
    [SerializeField] private GameObject _HpSlider;
    [SerializeField] private GameObject _PlayerWeapon;
    private PlayerInput _PlayerInput;
    // Start is called before the first frame update
    void Start()
    {
        _canvas = GameObject.Find("Canvas");
        _animator = GetComponent<Animator>();
        playerContext = new PlayerContext();
        pos = transform.position;
        MaxHp = currentHP;
        ResetPlayerHp();
        StartCoroutine(StaminaRegainCheck());
        _PlayerInput = GetComponent<PlayerInput>();
    }

    IEnumerator StaminaRegainCheck()
    {
        float deltaTime = 0f;
        while (true)
        {
            if(usingStamina)
            {
                staminaRegain = false;
                deltaTime = 0f;
            }
            else
            {
                if(!staminaRegain) // regain false -> using stamina right before this frame
                {
                    deltaTime += Time.deltaTime;
                    if(deltaTime > 1f)
                    {
                        staminaRegain = true;
                        deltaTime = 0f;
                    }
                }
            }
            yield return null;
        }
    }

    public void ResetPlayerHp()
    {
        
        _HpSlider.GetComponent<Slider>().value = currentHP = MaxHp; // slider max value
    }

    public string GetAttackVariable()
    {
        // None, Normal, Charge, FullCharge
        return GetComponent<Attack>().attackVariable.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _HpSlider.GetComponent<Slider>().value = currentHP;

        Debug.Log(GetComponent<Attack>().attackVariable.ToString());
    }

    private void FixedUpdate()
    {
        if (currentStamina < maxStamina && staminaRegain)
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

                currentHP = MaxHp;
                currentStamina = maxStamina;
                transform.position = pos;
                ResetPlayerHp();
                _animator.Play("idle");
                playerContext.ChangeState(IdleState.getInstance());

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void OnDamaged(float damage)
    {
        GetComponent<Damaged>().OnDamaged(damage);
    }

    public void PlayerGrabbed(bool grabStart)
    {
        if(grabStart)
        {
            _PlayerInput.enabled = false;
            _PlayerWeapon.SetActive(false);
        }
        else
        {
            _PlayerInput.enabled = true;
            _PlayerWeapon.SetActive(true);
        }
    }
}

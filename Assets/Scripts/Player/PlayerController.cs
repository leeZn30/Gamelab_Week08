using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerContext playerContext;
    public int currentHP = 2;
    private int MaxHp;
    private Vector3 pos;
    public bool usingStamina = false;
    private bool staminaRegain = true;

    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaConstant = 10f;

    public float normalAttackDamage = 10f;
    public float chargeAttackDamage = 20f;
    public float fullChargeAttackDamage = 30f;

    private Animator _animator;
    private GameObject _canvas;
    private Vector3 _hpIconInitPos = new Vector3(-900f, 480f, 0f);
    [SerializeField] private GameObject _hammerCollider;
    [SerializeField] private GameObject _StaminaSlider;
    [SerializeField] private GameObject _HpPrefab;
    Stack<GameObject> _HpContainer = new Stack<GameObject>();
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
    }

    IEnumerator StaminaRegainCheck()
    {
        while (true)
        {
            if(usingStamina)
            {
                staminaRegain = false;
            }
            else
            {
                if(!staminaRegain) // regain false -> using stamina right before this frame
                {
                    yield return new WaitForSeconds(1f);
                    staminaRegain = true;
                }
            }
            yield return null;
        }
    }

    public void ResetPlayerHp()
    {
        for(int i=0; i<currentHP; i++)
        {
            //GameObject instance = Instantiate(_HpPrefab, _canvas.transform);
            //_HpContainer.Push(instance);
            //instance.transform.localPosition = _hpIconInitPos + new Vector3(100f * i, 0f, 0f);
        }
    }

    public string GetAttackVariable()
    {
        // None, Normal, Charge, FullCharge
        return GetComponent<Attack>().attackVariable.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(_HpContainer.Count != currentHP)
        {
            while(currentHP < _HpContainer.Count)
            {
                if (_HpContainer.Count == 0)
                    break;
                Destroy(_HpContainer.Pop());
            }
        }

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
            }
        }
    }
}

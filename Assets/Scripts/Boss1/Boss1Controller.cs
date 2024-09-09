using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Controller : Boss
{
    [Header("정보")]
    // [SerializeField] float hp = 200;
    Slider hpGauge;
    bool isAlive = true;

    [Header("상태 머신")]
    public Boss1State nowState = Boss1State.Idle;

    [Header("패턴")]
    [SerializeField] List<Boss1State> phase1 = new List<Boss1State>();
    [SerializeField] List<Boss1State> phase2 = new List<Boss1State>();

    [Header("턴")]
    Vector2 forceDirection;

    [Header("이동")]
    [SerializeField] float minDistance = 3f;
    [SerializeField] float moveSpeed = 2f;

    [Header("패턴1")]
    bool isWalk;

    [Header("패턴2")]
    [SerializeField] bool isPattern2 = false;
    [SerializeField] Slash slash;
    [SerializeField] Transform slashPosition;

    [Header("시퀀스 패턴")]
    [SerializeField] UpSlash upSlash;
    [SerializeField] Transform upSlashPosition;

    PlayerController pc;
    Transform player;
    [SerializeField] Rigidbody2D rigid;
    public Animator anim;

    [SerializeField] Collider2D weaponCollider;


    private CinemachineVirtualCamera _virtualCamera;
    CinemachineBasicMultiChannelPerlin _perlin;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        pc = player.GetComponent<PlayerController>();
        hpGauge = GameObject.Find("BossHp").GetComponent<Slider>();
        hpGauge.maxValue = hp;
        hpGauge.value = hp;
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Start()
    {
        // DoDelay();
        StartCoroutine(startDelay());
    }

    void FixedUpdate()
    {
        if (isAlive && isWalk)
        {
            turn();

            rigid.velocity = new Vector2(forceDirection.x * moveSpeed, 0);
        }
    }

    void turn()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        forceDirection = new Vector2(directionToPlayer.x, 0).normalized;

        if (forceDirection.x >= 0 && transform.rotation != Quaternion.Euler(0f, 180f, 0f))
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void ChangeState()
    {
        Debug.Log("Change State");
        Boss1State inputState;
        // inputState = Boss1State.SwingAttack;
        // inputState = phase2[Random.Range(0, phase2.Count)];

        if (hp >= 100)
        {
            inputState = phase1[Random.Range(0, phase1.Count)];
        }
        else
        {
            inputState = phase2[Random.Range(0, phase2.Count)];
        }

        switch (inputState)
        {
            case Boss1State.Idle:
                break;

            case Boss1State.Walk:
                if (nowState != Boss1State.Idle)
                    return;
                break;
            case Boss1State.Jump:
                break;

            case Boss1State.Pattern1:
                nowState = Boss1State.Pattern1;
                StartCoroutine(Pattern1());
                break;

            case Boss1State.Pattern2:
                turn();
                nowState = Boss1State.Pattern2;
                // anim.Play("PrePattern2");
                StartCoroutine(Pattern2());
                break;

            case Boss1State.NormalAttack:
                nowState = Boss1State.NormalAttack;
                StartCoroutine(NormalAttack());
                break;

            case Boss1State.SequenceAttack:
                nowState = Boss1State.SequenceAttack;
                StartCoroutine(SequenceAttack());
                break;

            case Boss1State.SwingAttack:
                nowState = Boss1State.SwingAttack;
                StartCoroutine(SwingAttack());
                break;

            case Boss1State.BackAttack:
                nowState = Boss1State.BackAttack;
                StartCoroutine(BackAttack());
                break;
        }
    }

    public void OnAttack(float dmg = 0)
    {
        float damage = dmg;

        if (damage == 0)
        {
            switch (nowState)
            {
                case Boss1State.Pattern1:
                    damage = 20f;
                    break;

                case Boss1State.SequenceAttack:
                    damage = 25f;
                    break;

                case Boss1State.Pattern2:
                    damage = 10f;
                    break;

                case Boss1State.NormalAttack:
                    damage = 20f;
                    break;

                case Boss1State.BackAttack:
                    damage = 20f;
                    break;

                case Boss1State.SwingAttack:
                    damage = 25f;
                    break;
            }
        }

        pc.OnDamaged(damage);
    }

    IEnumerator Pattern1()
    {
        isWalk = true;
        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("Pattern1");
    }

    IEnumerator Pattern2()
    {
        // 플레이어가 패턴2 distance안에 있으며 minDistance보다 멀리 있다면 그냥 실행
        if (Vector2.Distance(player.position, transform.position) > 5f && Vector2.Distance(player.position, transform.position) < 8f)
        {
            turn();
            anim.Play("PrePattern2");
        }
        else
        {
            if (Vector2.Distance(player.position, transform.position) < 5f)
            {
                Vector2 directionToPlayer = player.position - transform.position;
                forceDirection = -new Vector2(directionToPlayer.x, 0).normalized;

                if (forceDirection.x >= 0 && transform.rotation != Quaternion.Euler(0f, 180f, 0f))
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }

            anim.Play("Roll");

            yield return null;

        }
    }

    IEnumerator NormalAttack()
    {
        isWalk = true;
        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("NormalAttack");
    }


    IEnumerator SequenceAttack()
    {
        isWalk = true;
        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("SequenceAttack");
    }

    IEnumerator SwingAttack()
    {
        isWalk = true;
        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("SwingAttack");
    }

    IEnumerator BackAttack()
    {
        isWalk = true;
        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("BackAttack");
    }



    void CreateSlash()
    {
        Instantiate(slash, slashPosition.position, Quaternion.identity, null);
    }
    void CreateUpSlash()
    {
        Instantiate(upSlash, upSlashPosition.position, Quaternion.identity, null);
    }

    void DoDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        weaponCollider.enabled = true;

        yield return null;
        // yield return new WaitForSeconds(Random.Range(2, 4));

        ChangeState();
    }

    IEnumerator startDelay()
    {
        yield return new WaitForSeconds(3f);

        ChangeState();
    }

    public override void OnDamaged(float damage)
    {
        if (hp > 0)
        {
            hp -= damage;
            hpGauge.value = hp;
        }

        if (hp <= 0)
        {
            isAlive = false;
            StopAllCoroutines();
            anim.Play("Dead");
        }
    }


    void OnDead()
    {
        StartCoroutine(waitForDeath());
    }

    IEnumerator waitForDeath()
    {
        yield return new WaitForSeconds(2f);

        if (GameManager.Instance != null)
            GameManager.Instance.showBossInfo("Boss1 Cleared! \n Let's Go to Next Boss");

        yield return new WaitForSeconds(2f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.hideBossInfo();

            GameManager.Instance.OnBoss1Cleared();
        }

        Destroy(gameObject);
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("PlayerAttack"))
    //     {
    //         PlayerController pc = FindObjectOfType<PlayerController>();
    //         if (pc.GetAttackVariable() == "Normal")
    //         {
    //             OnDamaged(pc.normalAttackDamage);
    //         }
    //         else if (pc.GetAttackVariable() == "Charge")
    //         {
    //             OnDamaged(pc.chargeAttackDamage);
    //         }
    //         else if (pc.GetAttackVariable() == "FullCharge")
    //         {
    //             OnDamaged(pc.fullChargeAttackDamage);
    //         }

    //         other.gameObject.SetActive(false);
    //     }
    // }

    void OnCameraShake()
    {
        StartCoroutine(ShakeCamera());
    }

    IEnumerator ShakeCamera()
    {
        _perlin.m_AmplitudeGain = 0.3f;
        _perlin.m_FrequencyGain = 1f;

        yield return new WaitForSeconds(1f);

        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;
    }

    void OnTurn()
    {
        // transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        Quaternion targetRotation = Quaternion.Euler(0f, 180f, 0f);
        float tolerance = 0.0001f; // 허용 오차 범위

        if (Quaternion.Angle(transform.rotation, targetRotation) > tolerance)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[Serializable]
public class patternWeight
{
    public Boss1TState state;
    public int weight;
    // 생성자 추가
    public patternWeight(Boss1TState state, int weight)
    {
        this.state = state;
        this.weight = weight;
    }
}

public class Boss1 : Boss
{
    [Header("상태")]
    Slider hpGauge;
    bool isTurn = false;
    public bool isChopCombo = false;
    bool isAlive = true;

    [Header("Player")]
    PlayerController player;
    float playerDistance => Mathf.Abs(player.transform.position.x - transform.position.x);
    Vector2 playerXDirection => GetPlayerXDirectionNormalizedAs1();

    [Header("패턴")]
    [SerializeField] Boss1TState nowState = Boss1TState.Idle;
    Coroutine nowStateCoroutine;

    [Header("페이즈")]
    [SerializeField]
    List<patternWeight> phase1 = new List<patternWeight>
    {
        new patternWeight(Boss1TState.ChopAttack, 34),
        new patternWeight(Boss1TState.SwingAttack, 33),
        new patternWeight(Boss1TState.SequnceAttack, 33),
    };

    [SerializeField]
    List<patternWeight> phase2_short = new List<patternWeight>
    {
        new patternWeight(Boss1TState.ChopAttack, 20),
        new patternWeight(Boss1TState.SwingAttack, 20),
        new patternWeight(Boss1TState.SequnceAttack, 20),
    };

    [SerializeField]
    List<patternWeight> phase2_long = new List<patternWeight>
    {
        new patternWeight(Boss1TState.WaveAttack, 50),
        new patternWeight(Boss1TState.UppercutAttack, 50),
    };

    List<patternWeight> nowPhase => hp < 100f ? (playerDistance > shortDistance ? phase2_long : phase2_short) : phase1;


    [Header("변수")]
    [SerializeField] float shortDistance = 4f;
    [SerializeField] float longDistance = 4f;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float rollSpeed = 10f;
    [SerializeField] float rollStandardDistance = 4f;
    [SerializeField] float rollCoolTime = 2f;
    float currentRollCoolTime = 0f;
    [SerializeField] Vector2 targetRollPosition;
    [SerializeField] float turnDelayTime = 0.2f;

    [Header("프리팹")]
    [SerializeField] UpSlash upSlash;
    [SerializeField] Slash slash;

    [SerializeField] Collider2D weaponCollider;
    Animator anim;
    Rigidbody2D rigid;
    [SerializeField] Transform slashPosition;
    [SerializeField] Transform upSlashPosition;
    private CinemachineVirtualCamera _virtualCamera;
    CinemachineBasicMultiChannelPerlin _perlin;

    void Awake()
    {
        hpGauge = GameObject.Find("BossHp").GetComponent<Slider>();
        hpGauge.maxValue = hp;
        hpGauge.value = hp;

        player = FindObjectOfType<PlayerController>();

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Start()
    {
        StartCoroutine(startDelay());
    }

    void Update()
    {
        if (isAlive)
        {
            if (!anim.GetBool("Attacking"))
                Turn();

            if (currentRollCoolTime > 0f)
            {
                currentRollCoolTime -= Time.deltaTime;

                if (currentRollCoolTime < 0f)
                    currentRollCoolTime = 0f;
            }

        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            // 걷기
            if (anim.GetFloat("Distance") == 1 && !isTurn)
            {
                int walkDirection;
                if (playerXDirection.x >= 0)
                {
                    walkDirection = 1;
                }
                else
                {
                    walkDirection = -1;
                }
                rigid.velocity = new Vector2(walkDirection * walkSpeed, rigid.velocity.y);
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }
    }

    IEnumerator startDelay()
    {
        yield return new WaitForSeconds(2f);
        CallState(ChangeState());
    }

    void Turn()
    {
        StartCoroutine(turnDelay(turnDelayTime));
    }

    IEnumerator turnDelay(float delayTime)
    {
        // 턴딜레이 살짝 주기
        if (playerXDirection.x > 0 && transform.localScale.x == 1)
        {
            isTurn = true;
            yield return new WaitForSeconds(delayTime);

            isTurn = false;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (playerXDirection.x <= 0 && transform.localScale.x == -1)
        {
            isTurn = true;
            yield return new WaitForSeconds(delayTime);

            isTurn = false;
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    void CallState(Boss1TState inputState)
    {
        // StopAllCoroutines();
        anim.SetFloat("Distance", 0);
        nowState = inputState;

        switch (inputState)
        {
            case Boss1TState.Idle:
                DoIdleState();
                break;

            case Boss1TState.Walk:
                DoWalkState();
                break;

            case Boss1TState.Roll:
                DoRollState();
                break;

            case Boss1TState.ChopAttack:
                DoChopAttackState();
                break;

            case Boss1TState.SwingAttack:
                DoSwingAttackState();
                break;

            case Boss1TState.SequnceAttack:
                DoSequenceAttackState();
                break;

            case Boss1TState.JumpAttack:
                DoJumpAttackState();
                break;


            case Boss1TState.BackAttack:
                DoBackAttackState();
                break;


            case Boss1TState.WaveAttack:
                DoWaveAttackState();
                break;


            case Boss1TState.UppercutAttack:
                DoUppercutAttackState();
                break;

        }
    }

    Boss1TState ChangeState()
    {
        // 연속 공격
        if (isChopCombo)
        {
            // 후방 공격
            if (isPlayerBehind())
            {
                return Boss1TState.BackAttack;
            }
            // 점프 공격
            else
            {
                return Boss1TState.JumpAttack;
            }
        }

        if (nowPhase == phase1)
        {
            if (playerDistance > shortDistance)
            {
                if (nowState == Boss1TState.Walk)
                {
                    int pick = UnityEngine.Random.Range(0, 2);

                    // 플레이어 가까이로 구르기
                    if (pick == 0 && currentRollCoolTime == 0f)
                    {
                        return Boss1TState.Roll;
                    }
                    else
                    {
                        return Boss1TState.Walk;
                    }
                }
                else
                {
                    return Boss1TState.Walk;
                }
            }
        }
        else
        {
            // 너무 멀면 걸어가기
            if (playerDistance > longDistance)
            {
                if (nowState == Boss1TState.Walk)
                {
                    int pick = UnityEngine.Random.Range(0, 2);

                    // 플레이어 가까이로 구르기
                    if (pick == 0 && currentRollCoolTime == 0f)
                    {
                        return Boss1TState.Roll;
                    }
                    else
                    {
                        return Boss1TState.Walk;
                    }
                }
                else
                {
                    return Boss1TState.Walk;
                }
            }

            // 적당히 멀면 원거리 공격
            if (playerDistance > shortDistance)
            {
                // 0~100, 101~120
                int pick = UnityEngine.Random.Range(0, 101);
                int sum = 0;

                foreach (patternWeight pw in nowPhase)
                {
                    if (pick <= pw.weight + sum)
                    {
                        if (pw.state == Boss1TState.Roll)
                        {
                            if (rollCoolTime != 0f)
                                return Boss1TState.Walk;
                            else return Boss1TState.Roll;
                        }
                        return pw.state;
                    }

                    sum += pw.weight;
                }
            }
        }

        if (playerDistance < 1f)
        {
            if (currentRollCoolTime == 0f)
                return Boss1TState.Roll;
            else return Boss1TState.ChopAttack;
        }


        if (playerDistance < shortDistance)
        {
            int pick = UnityEngine.Random.Range(0, 101);
            int sum = 0;

            foreach (patternWeight pw in nowPhase)
            {
                if (pick <= pw.weight + sum)
                {
                    return pw.state;
                }

                sum += pw.weight;
            }
        }

        // return Boss1TState.ChopAttack;
        return Boss1TState.Idle;
    }

    void changePatternWeight(int index, int decreaseAmount)
    {
        int n = nowPhase.Count;  // 총 n개의 값

        // 감소시킬 값이 리스트 값보다 크면 에러 방지
        if (decreaseAmount > nowPhase[index].weight)
        {
            Console.WriteLine("Error: Decrease amount is larger than the value at the given index.");
            return;
        }

        // 선택된 인덱스 값에서 decreaseAmount만큼 감소
        nowPhase[index].weight -= decreaseAmount;

        // 감소된 값을 나머지 n-1개의 값에 균등하게 분배
        int increasePerItem = decreaseAmount / (n - 1);
        int remainingIncrease = decreaseAmount % (n - 1);  // 나머지 값 처리

        // 나머지 요소들에 동일하게 increasePerItem만큼 추가
        for (int i = 0; i < n; i++)
        {
            if (i != index)  // 감소시킨 인덱스를 제외한 나머지
            {
                nowPhase[i].weight += increasePerItem;
            }
        }

        // 남은 값을 앞에서부터 분배
        for (int i = 0; i < n && remainingIncrease > 0; i++)
        {
            if (i != index)
            {
                nowPhase[i].weight++;
                remainingIncrease--;
            }
        }
    }

    void OnStateEnd(float delayTime)
    {
        StartCoroutine(StateChangeDelay(delayTime));
    }

    IEnumerator StateChangeDelay(float delayTime)
    {
        if (anim.GetBool("Attacking"))
            anim.SetBool("Attacking", false);

        yield return new WaitForSeconds(delayTime);

        CallState(ChangeState());
    }


    void DoIdleState()
    {
        anim.SetFloat("Distance", 0);
    }

    void DoWalkState()
    {
        anim.SetFloat("Distance", 1);

        if (nowPhase == phase2_long)
        {
            changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.Walk), 10);
        }
    }

    void DoRollState()
    {
        anim.Play("Roll");
        currentRollCoolTime = rollCoolTime;
        StartCoroutine(RollMovement());

        if (nowPhase == phase2_long)
        {
            changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.Roll), 10);
        }
    }

    IEnumerator RollMovement()
    {
        // 애니메이터의 현재 상태 정보를 가져오기 위해 대기
        yield return new WaitForEndOfFrame();  // 애니메이션이 시작되도록 한 프레임 대기

        float adjustedAnimationLength = GetCurrentAnimationAdjustedLength();

        // 애니메이션 실행 시간 동안 방향으로 이동
        float elapsedTime = 0f;

        // 플레이어 가까이 있으면 멀리, 멀리 있으면 가까이
        Vector2 rollDirection;
        if (playerDistance > rollStandardDistance)
        {
            rollDirection = playerXDirection;
        }
        else
        {
            rollDirection = -playerXDirection;
        }

        while (elapsedTime < adjustedAnimationLength)
        {
            // 매 프레임마다 지정된 방향으로 이동
            transform.Translate(rollDirection * rollSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;  // 한 프레임 대기
        }
    }

    void DoChopAttackState()
    {
        anim.Play("ChopAttack");
        anim.SetBool("Attacking", true);
        changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.ChopAttack), 20);
    }

    void CheckCombo()
    {
        // 콤보를 할 것인가
        if (nowPhase != phase1 && playerDistance < shortDistance + 2f)
        {
            isChopCombo = true;
        }
    }

    bool isPlayerBehind()
    {
        if (transform.localScale.x == 1)
        {
            if (playerXDirection.x == 1)
                return true;
            else return false;
        }
        else
        {
            if (playerXDirection.x == -1)
                return true;
            else return false;
        }
    }


    void DoSwingAttackState()
    {
        anim.Play("SwingAttack");
        anim.SetBool("Attacking", true);
        changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.SwingAttack), 20);
    }

    void DoSequenceAttackState()
    {
        anim.Play("SequenceAttack");
        anim.SetBool("Attacking", true);
        changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.SequnceAttack), 20);
    }

    void onCreateUpSlash()
    {
        Instantiate(upSlash, upSlashPosition.position, Quaternion.identity, null);
    }

    void DoJumpAttackState()
    {
        isChopCombo = false;
        anim.SetTrigger("JumpAttack");
        anim.SetBool("Attacking", true);
    }

    void DoBackAttackState()
    {
        isChopCombo = false;
        anim.SetTrigger("BackAttack");
        anim.SetBool("Attacking", true);
    }

    void DoWaveAttackState()
    {
        anim.Play("WaveAttack");
        anim.SetBool("Attacking", true);
        changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.WaveAttack), 10);
    }

    void CreateSlash()
    {
        Instantiate(slash, slashPosition.position, Quaternion.identity, null);
    }

    void DoUppercutAttackState()
    {
        anim.SetBool("Attacking", true);
        anim.Play("UppercutMoving");
        StartCoroutine(UppercutMovement());
        changePatternWeight(nowPhase.FindIndex(e => e.state == Boss1TState.UppercutAttack), 10);
    }

    IEnumerator UppercutMovement()
    {
        // 애니메이터의 현재 상태 정보를 가져오기 위해 대기
        yield return new WaitForEndOfFrame();  // 애니메이션이 시작되도록 한 프레임 대기

        // 애니메이션 실행 시간 동안 방향으로 이동
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(player.transform.position.x + (-playerXDirection.x * 2f), transform.position.y, transform.position.z);
        float stoppingDistance = 0.1f;  // 오차 범위 (목표와의 최소 거리)


        // 이동이 완료될 때까지 실행
        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            // 현재 위치에서 목표 지점으로 MoveTowards를 사용해 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Vector3.Distance(startPosition, targetPosition) * Time.deltaTime * 1.5f);

            elapsedTime += Time.deltaTime;
            yield return null;  // 한 프레임 대기
        }

        // 마지막으로 목표 지점에 정확히 도달시키기
        transform.position = targetPosition;

        anim.SetTrigger("UppercutAttack");
    }


    void JustTurn()
    {
        if (transform.localScale.x == 1)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    Vector2 GetPlayerXDirectionNormalizedAs1()
    {
        if ((player.transform.position - transform.position).x >= 0)
        {
            return new Vector2(1, 0);
        }
        else
        {
            return new Vector2(-1, 0);
        }
    }

    float GetCurrentAnimationAdjustedLength()
    {
        // 애니메이션 상태 정보 가져옴
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 현재 실행 중인 애니메이션의 길이 가져오기
        float baseAnimationLength = stateInfo.length;

        // 현재 애니메이션 재생 속도 적용
        float animationSpeed = stateInfo.speed;

        return baseAnimationLength / animationSpeed;
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
            StopAllCoroutines();
            isAlive = false;
            weaponCollider.enabled = false;
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

    public void OnAttack(float dmg = 0)
    {
        float damage = dmg;

        if (damage == 0)
        {
            switch (nowState)
            {
                case Boss1TState.ChopAttack:
                    damage = 20;
                    break;

                case Boss1TState.SwingAttack:
                    damage = 10;
                    break;

                case Boss1TState.SequnceAttack:
                    damage = 10;
                    break;

                case Boss1TState.JumpAttack:
                    damage = 30;
                    break;

                case Boss1TState.BackAttack:
                    damage = 15;
                    break;

                case Boss1TState.WaveAttack:
                    damage = 10;
                    break;

                case Boss1TState.UppercutAttack:
                    damage = 15;
                    break;
            }

            player.OnDamaged(damage);
        }
    }


}

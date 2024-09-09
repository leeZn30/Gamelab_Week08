using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss2_ai : Boss
{
    [Header("HP")]
    [SerializeField] private float phaseSwapHp = 100;
    

    [Header("Speed")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float backStepSpeed = 8;
    [SerializeField] private float rushPatternSpeed = 15;
    [SerializeField] private float speedLerpFactor_P3;
    [SerializeField] private float speedLerpFactor_P3_1;
    [SerializeField] private float speedLerpFactor_P3_2;
    [SerializeField] private float speedLerpFactor_P3_3;
    [SerializeField] private float speedLerpFactor_P4_1 = 1;

    [Header("Pattern")]
    [SerializeField] private int lookingDir;
    [SerializeField] private int currentPhase;
    [SerializeField] private PatternConditionState currentPatternConditionState;
    [SerializeField] private string currentPatternName;

    [SerializeField] private PatternProbabilityScriptableObj[] phase1_PatternProbabilities;
    [SerializeField] private PatternProbabilityScriptableObj[] phase2_PatternProbabilities;

    [Header("Pattern Check Area")]
    [SerializeField] private float rightBehindDistCheckFactor;
    [SerializeField] private float closeDistCheckFactor;
    [SerializeField] private float normalDistCheckFactor;

    [Header("ETC")]
    [SerializeField] private Slider hpGauge;
    [SerializeField] private Rigidbody2D rb;
    private float speedLerpValue = 1;
    private Vector3 playerCalcualtePosition = Vector3.zero;
    private Coroutine patternCoroutine;
    [SerializeField] private Animator objAnimator;

    private GameObject playerObj;
    private PlayerController playerController;
    private Transform playerTf;

    [Header("Boolean")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isSliding = false; 

    private void Awake()
    {
        if (objAnimator == null)
            objAnimator = GetComponent<Animator>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        playerObj = GameObject.FindWithTag("Player");
        if (playerTf == null)
            playerTf = playerObj.GetComponent<Transform>();
        if (playerController == null)
            playerController = playerObj.GetComponent<PlayerController>();

        hpGauge.maxValue = hp;
        hpGauge.value = hp;
    }

    private void Start()
    {
        PlayIntro();
    }

    private void Update()
    {
        //CheckPatternConditionState();
    }

    private void FixedUpdate()
    {
        if (currentPatternName == "W")
        {
            transform.position += Vector3.right * walkSpeed * lookingDir * Time.fixedDeltaTime;
            CheckPatternConditionState();
            if (currentPatternConditionState != PatternConditionState.Far && currentPatternConditionState != PatternConditionState.Normal)
            {
                StopAllCoroutines();
                PlayRandomPattern(currentPhase);
            }
        }
        else if (currentPatternName == "WR")
        {
            transform.position += Vector3.right * walkSpeed * -lookingDir * Time.fixedDeltaTime;
            CheckPatternConditionState();
            if (currentPatternConditionState != PatternConditionState.Close && currentPatternConditionState != PatternConditionState.Normal)
            {
                StopAllCoroutines();
                PlayRandomPattern(currentPhase);
            }
        }
        else if (currentPatternName == "P4")
        {
            transform.position += Vector3.right * runSpeed * lookingDir * Time.fixedDeltaTime;
            CheckPatternConditionState();
            if (currentPatternConditionState != PatternConditionState.Far)
            {
                StopAllCoroutines();
                PlayPattern_4_1();
                speedLerpValue = 1.5f;
            }
        }
        else if (currentPatternName == "P4_1")
        {
            speedLerpValue = Mathf.Lerp(speedLerpValue, 0, Time.fixedDeltaTime * speedLerpFactor_P4_1);
            transform.position += Vector3.right * runSpeed * speedLerpValue * lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3")
        {
            speedLerpValue = Mathf.Lerp(speedLerpValue, 1, Time.fixedDeltaTime * speedLerpFactor_P3);
            transform.position += Vector3.right * backStepSpeed * speedLerpValue * -lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3_1")
        {
            speedLerpValue = Mathf.Lerp(speedLerpValue, 0, Time.fixedDeltaTime * speedLerpFactor_P3_1);
            transform.position += Vector3.right * rushPatternSpeed * speedLerpValue * lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3_2")
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, playerCalcualtePosition.x, Time.fixedDeltaTime * speedLerpFactor_P3_2),
                transform.position.y,
                transform.position.z);
        }
        else if (currentPatternName == "P3_3")
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, playerCalcualtePosition.x, Time.fixedDeltaTime * speedLerpFactor_P3_3),
                transform.position.y,
                transform.position.z);
        }
    }

    public override void OnDamaged(float damage)
    {
        if (hp > 0)
        {
            hp -= damage;
        }

        hpGauge.value = hp;

        if (hp < phaseSwapHp)
            currentPhase = 1;

        if (hp <= 0 && !isDead) 
        {
            isDead = true;
            PlayDeath();
        }
    }

    private void PlayDeath()
    {
        StopAllCoroutines();
        currentPatternName = "None";
        rb.gravityScale = 1;
        objAnimator.Play("Boss2_Die");
        patternCoroutine = StartCoroutine(waitForDeath());
    }

    IEnumerator waitForDeath()
    {
        yield return new WaitForSeconds(2f);

        GameManager.Instance.showBossInfo("Game Cleared! \n Congratulations!");

        yield return new WaitForSeconds(2f);

        GameManager.Instance.hideBossInfo();

        // 게임매니저에 알리기
        //GameManager.Instance.OnBoss2Cleared();

        Destroy(gameObject);
    }

    private void PlayRandomPattern(int phase)
    {
        CheckPatternConditionState();
        PatternProbabilityScriptableObj _patternPool;
        
        if (phase == 0)
            _patternPool = phase1_PatternProbabilities[((int)currentPatternConditionState)];
        else
        {
            _patternPool = phase2_PatternProbabilities[((int)currentPatternConditionState)];
        }

        int _randomNum = UnityEngine.Random.Range(0, 100);
        int _addNum = _patternPool.Probability[0];
        for(int i = 0; i < _patternPool.Probability.Length; i++)
        {
            Debug.Log($"{_randomNum} {_addNum}");
            if (_randomNum < _addNum)
            {
                Debug.Log($"Dist State: {currentPatternConditionState}  /  Pattern State: {_patternPool.PatternName[i]}");
                PlayPattern(_patternPool.PatternName[i]);
                break;
            }

            _addNum += _patternPool.Probability[i + 1];
        }
    }

    private void PlayIntro()
    {
        PlayIdle(3);
    }

    private void PlayPattern(string patternName)
    {
        PatternState state = PatternState.idle;
        Enum.TryParse(patternName, out state);

        switch (state) { 
            case PatternState.idle:
                PlayIdle(0);
                break;
            case PatternState.pattern1:
                PlayPattern_1();
                break;
            case PatternState.pattern2:
                PlayPattern_2();
                break;
            case PatternState.pattern2_1:
                //PlayPattern_2_1();
                break;
            case PatternState.pattern3:
                PlayPattern_3();
                break;
            case PatternState.pattern4:
                PlayPattern_4();
                break;
            case PatternState.pattern5:
                PlayPattern_5();
                break;
            case PatternState.pattern6:
                PlayPattern_6();
                break;
            case PatternState.pattern7:
                PlayPattern_7();
                break;
            case PatternState.walk:
                PlayWalk();
                break;
            case PatternState.turn:
                Turn();
                break;
            case PatternState.walkReverse:
                PlayWalkReverse();
                break;
            default:
                break;
        }
    }

    #region Pattern Method
    private void PlayPattern_1()
    {
        currentPatternName = "P1";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern1());
    }

    private void PlayPattern_1_1()
    {
        currentPatternName = "P1_1";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern1_1());
    }

    private void PlayPattern_1_2()
    {
        currentPatternName = "P1_2";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern1_2());
    }

    private void PlayPattern_2()
    {
        currentPatternName = "P2";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern2());
    }

    private void PlayPattern_3()
    {
        currentPatternName = "P3";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern3());
    }

    private void PlayPattern_3_1()
    {
        currentPatternName = "P3_1";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern3_1());
    }

    private void PlayPattern_3_2()
    {
        currentPatternName = "P3_2";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern3_2());
    }

    private void PlayPattern_3_3()
    {
        currentPatternName = "P3_3";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern3_3());
    }

    private void PlayPattern_4()
    {
        currentPatternName = "P4";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern4());
    }

    private void PlayPattern_4_1()
    {
        currentPatternName = "P4_1";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern4_1());
    }

    private void PlayPattern_5()
    {
        currentPatternName = "P5";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern5());


    }

    private void PlayPattern_6()
    {
        currentPatternName = "P6";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern6());
    }

    private void PlayPattern_7()
    {
        currentPatternName = "P7";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern7());
    }

    private void PlayWalk()
    {
        currentPatternName = "W";
        patternCoroutine = StartCoroutine(IE_Walk(0));
    }

    private void PlayWalkReverse()
    {
        currentPatternName = "WR";
        patternCoroutine = StartCoroutine(IE_WalkReverse(0));
    }

    private void PlayIdle(float waitSec)
    {
        currentPatternName = "I";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Idle(waitSec));
    }

    private void Turn()
    {
        lookingDir *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        PlayRandomPattern(currentPhase);
    }
    #endregion

    #region Pattern Coroutine

    private IEnumerator IE_Pattern1()
    {
        objAnimator.Play("Boss2_Pattern1");

        yield return new WaitForSecondsRealtime(1.27f);

        if (currentPatternConditionState == PatternConditionState.Close)
        {
            PlayPattern_1_1();
            yield break;
        }
        else
        {
            PlayIdle(0);
        }
    }

    private IEnumerator IE_Pattern1_1()
    {
        objAnimator.Play("Boss2_Pattern1_1");

        yield return new WaitForSecondsRealtime(1.25f);

        if (currentPhase == 0)
            PlayIdle(0);
        else if (currentPatternConditionState == PatternConditionState.Close)
        {
            PlayPattern_1_2();
            yield break;
        }
        else if (currentPatternConditionState == PatternConditionState.RightBehind)
        {
            PlayPattern_5();
            yield break;
        }
        else
        {
            PlayIdle(0);
        }
    }

    private IEnumerator IE_Pattern1_2()
    {
        objAnimator.Play("Boss2_Pattern1_2");

        yield return new WaitForSecondsRealtime(2f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern2()
    {
        objAnimator.Play("Boss2_Pattern2");

        yield return new WaitForSecondsRealtime(2.65f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern3()
    {
        objAnimator.Play("Boss2_Pattern3");
        speedLerpValue = 0;
        rb.gravityScale = 0;

        yield return new WaitForSecondsRealtime(1.817f);

        rb.gravityScale = 1;

        if (UnityEngine.Random.Range(0, 2)  == 0)
        {
            PlayPattern_3_1();
        }
        else
        {
            if (currentPhase == 0)
                PlayPattern_3_2();
            else
                PlayPattern_3_3();
        }
    }

    private IEnumerator IE_Pattern3_1()
    {
        objAnimator.Play("Boss2_Pattern3_1");
        speedLerpValue = 3.5f;

        yield return new WaitForSecondsRealtime(3.6f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern3_2()
    {
        rb.gravityScale = 0;
        objAnimator.Play("Boss2_Pattern3_2");
        playerCalcualtePosition = playerTf.position;

        yield return new WaitForSecondsRealtime(2.9f);

        rb.gravityScale = 1;
        PlayIdle(0);
    }

    private IEnumerator IE_Pattern3_3()
    {
        rb.gravityScale = 0;
        objAnimator.Play("Boss2_Pattern3_3");
        playerCalcualtePosition = playerTf.position;

        yield return new WaitForSecondsRealtime(2.9f);

        rb.gravityScale = 1;
        PlayIdle(0);
    }

    private IEnumerator IE_Pattern4()
    {
        objAnimator.Play("Boss2_Pattern4");
        yield break;
    }

    private IEnumerator IE_Pattern4_1()
    {
        objAnimator.Play("Boss2_Pattern4_1");

        yield return new WaitForSecondsRealtime(1.2f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern5()
    {
        objAnimator.Play("Boss2_Pattern5");

        yield return new WaitForSecondsRealtime(1.017f);

        lookingDir *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        PlayIdle(0);
    }

    private IEnumerator IE_Pattern6()
    {
        objAnimator.Play("Boss2_Pattern6");

        yield return new WaitForSecondsRealtime(2.65f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern7()
    {
        objAnimator.Play("Boss2_Pattern7");

        yield return new WaitForSecondsRealtime(2.5f);

        PlayIdle(0);
    }

    private IEnumerator IE_Idle(float waitSec)
    {
        objAnimator.Play("Boss2_Idle");

        if (waitSec == 0)
        {
            float randSec = UnityEngine.Random.Range(0.2f, 0.3f);
            Debug.Log($"Wait For {randSec}sec");
            yield return new WaitForSecondsRealtime(randSec);
        }
        else
        {
            Debug.Log($"Wait For {waitSec}sec");
            yield return new WaitForSecondsRealtime(waitSec);
        }

        PlayRandomPattern(currentPhase);
    }

    private IEnumerator IE_Walk(float walkSec)
    {
        objAnimator.Play("Boss2_Walk");

        if (walkSec == 0)
        {
            float randSec = UnityEngine.Random.Range(1f, 2f);
            Debug.Log($"Walk For {randSec}sec");
            yield return new WaitForSecondsRealtime(randSec);
        }
        else
        {
            Debug.Log($"Walk For {walkSec}sec");
            yield return new WaitForSecondsRealtime(walkSec);
        }

        PlayRandomPattern(currentPhase);
    }

    private IEnumerator IE_WalkReverse(float walkSec)
    {
        objAnimator.Play("Boss2_WalkReverse");

        if (walkSec == 0)
        {
            float randSec = UnityEngine.Random.Range(0.3f, 0.8f);
            Debug.Log($"Walk Backward For {randSec}sec");
            yield return new WaitForSecondsRealtime(randSec);
        }
        else
        {
            Debug.Log($"Walk Backward For {walkSec}sec");
            yield return new WaitForSecondsRealtime(walkSec);
        }

        PlayRandomPattern(currentPhase);
    }

    #endregion

    private void CheckPatternConditionState()
    {
        float distFromBoss = (playerTf.position.x - transform.position.x) * lookingDir;

        if (distFromBoss <= 0 && distFromBoss > rightBehindDistCheckFactor)
        {
            currentPatternConditionState = PatternConditionState.RightBehind;
        }
        else if (distFromBoss > 0 && distFromBoss < closeDistCheckFactor) 
        {
            currentPatternConditionState = PatternConditionState.Close;
        }
        else if (distFromBoss >= closeDistCheckFactor && distFromBoss < normalDistCheckFactor)
        {
            currentPatternConditionState = PatternConditionState.Normal;
        }
        else if (distFromBoss <= rightBehindDistCheckFactor)
        {
            currentPatternConditionState = PatternConditionState.Behind;
        }
        else
        {
            currentPatternConditionState = PatternConditionState.Far;
        }
    }

    private enum PatternState
    {
        idle,
        walk,
        walkReverse,
        turn,
        pattern1,
        pattern1_1,
        pattern1_2,
        pattern2,
        pattern2_1,
        pattern3,
        pattern3_1,
        pattern3_2,
        pattern3_3,
        pattern4,
        pattern5,
        pattern6,
        pattern7,
    }
}

public enum PatternConditionState
{
    Normal,
    RightBehind,
    Close,
    Far,
    Behind,
}
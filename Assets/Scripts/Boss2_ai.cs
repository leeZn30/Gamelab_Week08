using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss2_ai : MonoBehaviour
{
    [SerializeField] private float bossHp = 250;
    [SerializeField] private float phaseSwapHp = 100;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private float backStepSpeed = 8;
    [SerializeField] private int lookingDir;
    [SerializeField] private int currentPhase;
    [SerializeField] private PatternConditionState currentPatternConditionState;
    [SerializeField] private string currentPatternName;

    [SerializeField] private PatternProbabilityScriptableObj[] phase1_PatternProbabilities;
    [SerializeField] private PatternProbabilityScriptableObj[] phase2_PatternProbabilities;

    [SerializeField] private float rightBehindDistCheckFactor;
    [SerializeField] private float closeDistCheckFactor;
    [SerializeField] private float normalDistCheckFactor;

    [SerializeField] private float speedLerpFactor = 1;
    [SerializeField] private Slider hpGauge;
    private float speedLerpValue = 1;
    private Vector3 playerCalcualtePosition = Vector3.zero;
    private Coroutine patternCoroutine;
    private Animator objAnimator;
    private Transform playerTf;

    private void Awake()
    {
        objAnimator = GetComponent<Animator>();
        playerTf = GameObject.FindWithTag("Player").GetComponent<Transform>();
        hpGauge.maxValue = bossHp;
        hpGauge.value = bossHp;
    }

    private void Start()
    {
        PlayIntro();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            PlayerController pc = GetComponent<PlayerController>();
            if (pc.GetAttackVariable() == "Normal")
            {
                OnDamaged(pc.normalAttackDamage);
            }
            else if (pc.GetAttackVariable() == "Charge")
            {
                OnDamaged(pc.chargeAttackDamage);
            }
            else if (pc.GetAttackVariable() == "FullCharge")
            {
                OnDamaged(pc.fullChargeAttackDamage);
            }
            other.gameObject.SetActive(false);
        }
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
            speedLerpValue = Mathf.Lerp(speedLerpValue, 0, Time.fixedDeltaTime * speedLerpFactor);
            transform.position += Vector3.right * runSpeed * speedLerpValue * lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3")
        {
            speedLerpValue = Mathf.Lerp(speedLerpValue, 1, Time.fixedDeltaTime * speedLerpFactor * 2.5f);
            transform.position += Vector3.right * runSpeed * speedLerpValue * -lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3_1")
        {
            speedLerpValue = Mathf.Lerp(speedLerpValue, 0, Time.fixedDeltaTime * speedLerpFactor * 1.5f);
            transform.position += Vector3.right * backStepSpeed * speedLerpValue * lookingDir * Time.fixedDeltaTime;
        }
        else if (currentPatternName == "P3_2")
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, playerCalcualtePosition.x, Time.fixedDeltaTime * speedLerpFactor * 1.2f),
                transform.position.y,
                transform.position.z);
        }
    }

    void OnDamaged(float damage)
    {
        if (bossHp > 0)
        {
            bossHp -= damage;
        }

        hpGauge.value = bossHp;

        if (bossHp < 0) 
        {
            PlayDeath();
        }
    }

    private void PlayDeath()
    {

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
        PlayIdle(5);
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
            case PatternState.walk:
                PlayWalk();
                break;
            case PatternState.turn:
                Turn();
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
        currentPatternName = "P1-1";
        StopAllCoroutines();
        patternCoroutine = StartCoroutine(IE_Pattern1_1());
    }

    private void PlayPattern_1_2()
    {
        currentPatternName = "P1-2";
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

    private void PlayWalk()
    {
        currentPatternName = "W";
        patternCoroutine = StartCoroutine(IE_Walk(0));
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

        yield return new WaitForSecondsRealtime(1.817f);

        if (UnityEngine.Random.Range(0, 2)  == 0)
        {
            PlayPattern_3_1();
        }
        else
        {
            PlayPattern_3_2();
        }
    }

    private IEnumerator IE_Pattern3_1()
    {
        objAnimator.Play("Boss2_Pattern3_1");
        speedLerpValue = 2;

        yield return new WaitForSecondsRealtime(3.6f);

        PlayIdle(0);
    }

    private IEnumerator IE_Pattern3_2()
    {
        objAnimator.Play("Boss2_Pattern3_2");
        playerCalcualtePosition = playerTf.position;

        yield return new WaitForSecondsRealtime(2.083f);

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

        yield return null;

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

    private IEnumerator IE_Idle(float waitSec)
    {
        objAnimator.Play("Boss2_Idle");

        if (waitSec == 0)
        {
            float randSec = UnityEngine.Random.Range(0.3f, 1f);
            Debug.Log($"Wait For {randSec}Sec");
            yield return new WaitForSecondsRealtime(randSec);
        }
        else
        {
            Debug.Log($"Wait For {waitSec}Sec");
            yield return new WaitForSecondsRealtime(waitSec);
        }

        PlayRandomPattern(currentPhase);
    }

    private IEnumerator IE_Walk(float walkSec)
    {
        objAnimator.Play("Boss2_Walk");

        if (walkSec == 0)
        {
            float randSec = UnityEngine.Random.Range(1.5f, 2.5f);
            Debug.Log($"Walk For {randSec}Sec");
            yield return new WaitForSecondsRealtime(randSec);
        }
        else
        {
            Debug.Log($"Walk For {walkSec}Sec");
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
        turn,
        pattern1,
        pattern1_1,
        pattern1_2,
        pattern2,
        pattern2_1,
        pattern3,
        pattern3_1,
        pattern3_2,
        pattern4,
        pattern5,
        pattern6,
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
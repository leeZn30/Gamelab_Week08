using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_ai : MonoBehaviour
{
    private int currentPhase;

    private PatternConditionState currentPatternConditionState;

    [SerializeField] private PatternProbabilityScriptableObj[] phase1_PatternProbabilities;
    [SerializeField] private PatternProbabilityScriptableObj[] phase2_PatternProbabilities;

    private void PlayRandomPattern(int phase)
    {
        PatternProbabilityScriptableObj _patternPool;
        if (phase == 0)
            _patternPool = phase1_PatternProbabilities[((int)currentPatternConditionState)];
        else
        {
            _patternPool = phase2_PatternProbabilities[((int)currentPatternConditionState)];
        }

        int _randomNum = UnityEngine.Random.Range(0, 100);
        int _addNum = 0;
        for(int i = 0; i < phase1_PatternProbabilities.Length; i++)
        {
            if (_randomNum < _patternPool.Probability[i])
            {
                PlayPattern(_patternPool.PatternName[i]);
                break;
            }

            _addNum += _patternPool.Probability[i];
        }
    }

    private void PlayIntro()
    {

    }

    private void PlayPattern(string patternName)
    {
        PatternState state = PatternState.idle;
        Enum.TryParse(patternName, out state);

        switch (state) { 
            case PatternState.idle:
                PlayIdle();
                break;
            case PatternState.pattern1:
                PlayPattern_1();
                break;
            case PatternState.pattern2:
                PlayPattern_2();
                break;
            case PatternState.pattern2_1:
                PlayPattern_2_1();
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

    private void PlayPattern_1()
    {

    }

    private void PlayPattern_2()
    {

    }

    private void PlayPattern_2_1()
    {

    }

    private void PlayPattern_3()
    {

    }

    private void PlayPattern_4()
    {

    }

    private void PlayPattern_5()
    {

    }

    private void PlayPattern_6()
    {

    }

    private void PlayWalk()
    {
        UnityEngine.Random.Range(0.5f, 1.5f);
    }

    private void PlayIdle()
    {

    }

    private void Turn()
    {

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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_ai : MonoBehaviour
{
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
    Idle,
    RightBehind,
    Close,
    Far,
}
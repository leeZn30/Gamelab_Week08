using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Controller : Singleton<Boss1Controller>
{
    [Header("상태 머신")]
    public Boss1State nowState = Boss1State.Idle;
    Boss1Control nowControl;

    [Header("애니메이션")]
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        nowControl = GetComponent<Boss1Idle>();
    }

    public void AskPermission(Boss1Control inputControl)
    {
        ChangeState(inputControl);
    }

    public void PostCompleion(Boss1Control inputControl)
    {
        // Idle로 변경
        switch (inputControl.State)
        {
            case Boss1State.Walk:
                ChangeState(GetComponent<Boss1Pattern1>());
                break;

            default:
                ChangeState(GetComponent<Boss1Idle>());
                break;
        }
    }

    void ChangeState(Boss1Control inputControl)
    {
        if (inputControl.priority < nowControl.priority && inputControl == nowControl)
            return;

        switch (inputControl.State)
        {
            case Boss1State.Idle:
                break;

            case Boss1State.Walk:
                nowControl.Stop();

                nowControl = inputControl;
                inputControl.Do();
                nowState = inputControl.State;
                break;

            case Boss1State.Jump:
                break;

            case Boss1State.Pattern1:
                nowControl.Stop();

                nowControl = inputControl;
                inputControl.Do();
                nowState = inputControl.State;
                break;

            case Boss1State.Patter2:
                break;

            default:
                break;
        }
    }
}

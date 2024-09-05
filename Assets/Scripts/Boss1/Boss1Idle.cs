using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Boss = Boss1Controller;

public class Boss1Idle : MonoBehaviour, Boss1Control
{
    [SerializeField] int _priority;
    [SerializeField] Boss1State _state;
    public Boss1State State { get => _state; set => _state = Boss1State.Idle; }
    public int priority { get => _priority; set => priority = _priority; }

    public void Do()
    {
        Boss.Instance.anim.Play("Idle");
    }

    public void Stop()
    {
    }
}

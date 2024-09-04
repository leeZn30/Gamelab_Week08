using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boss = Boss1Controller;

public class Boss1Jump : MonoBehaviour, Boss1Control
{
    [SerializeField] int _priority;
    [SerializeField] Boss1State _state;
    public Boss1State State { get => _state; set => _state = Boss1State.Jump; }
    public int priority { get => _priority; set => priority = _priority; }

    Transform player;

    [Header("이동")]
    [SerializeField] float distance = 0.5f;

    public void Do()
    {
    }

    public void Stop()
    {

    }

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > distance)
        {
            Boss.Instance.AskPermission(this);
        }
    }
}

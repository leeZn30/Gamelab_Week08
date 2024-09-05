using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boss = Boss1Controller;

public class Boss1Walk : MonoBehaviour, Boss1Control
{
    [SerializeField] int _priority;
    [SerializeField] Boss1State _state;
    public Boss1State State { get => _state; set => _state = Boss1State.Walk; }
    public int priority { get => _priority; set => priority = _priority; }

    Transform player;
    Rigidbody2D rigid;

    [Header("이동")]
    bool isMovable = false;
    [SerializeField] float distance = 3f;
    [SerializeField] float speed = 1f;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Do()
    {
        isMovable = true;
        Boss.Instance.anim.Play("Walk");
    }

    public void Stop()
    {
        isMovable = false;
    }

    void Update()
    {
        // if (Vector3.Distance(transform.position, player.position) > distance)
        // {
        //     Boss.Instance.AskPermission(this);
        // }
        // else
        // {
        //     Stop();
        //     Boss.Instance.PostCompleion(this);
        // }
    }

    void FixedUpdate()
    {
        if (isMovable)
        {
            Vector2 directionToPlayer = player.position - transform.position;
            Vector2 forceDirection = new Vector2(directionToPlayer.x, 0).normalized;

            if (forceDirection.x >= 0 && transform.rotation != Quaternion.Euler(0f, 180f, 0f))
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            rigid.velocity = new Vector2(forceDirection.x * speed, 0);
        }

    }
}

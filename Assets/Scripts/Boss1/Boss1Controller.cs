using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Controller : Singleton<Boss1Controller>
{
    [Header("상태 머신")]
    public Boss1State nowState = Boss1State.Idle;

    [Header("패턴")]
    [SerializeField] List<Boss1State> phase1 = new List<Boss1State>();

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

    Transform player;
    [SerializeField] Rigidbody2D rigid;
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        ChangeState();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (isWalk)
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
        Boss1State inputState = phase1[Random.Range(0, phase1.Count)];
        // Boss1State inputState = phase1[1];

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
                anim.Play("PrePattern2");
                break;
        }
    }

    IEnumerator Pattern1()
    {
        isWalk = true;

        anim.Play("Walk");

        yield return new WaitUntil(() => Vector2.Distance(player.position, transform.position) <= minDistance);

        isWalk = false;
        anim.SetTrigger("Pattern1");
    }

    void CreateSlash()
    {
        Instantiate(slash, slashPosition);
    }

    void DoDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        Debug.Log("Delay");

        yield return new WaitForSeconds(Random.Range(1, 2));

        ChangeState();
    }
}

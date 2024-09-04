using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Boss1Controller : Singleton<Boss1Controller>
{
    [Header("상태 머신")]
    public Boss1State nowState = Boss1State.Idle;

    [Header("패턴")]
    [SerializeField] List<Boss1State> phase1 = new List<Boss1State>();

    [Header("이동")]
    [SerializeField] float minDistance = 3f;
    [SerializeField] float moveSpeed = 2f;

    [Header("패턴1")]
    bool isWalk;

    [Header("패턴2")]
    bool isGround;

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
        // ChangeState(Boss1State.Pattern1);
        ChangeState(Boss1State.Pattern2);
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 3f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3f);
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            isGround = true;
        }
        else
        {
            anim.Play("Jump");
            isGround = false;
        }
    }

    void FixedUpdate()
    {
        if (isWalk)
        {
            Vector2 directionToPlayer = player.position - transform.position;
            Vector2 forceDirection = new Vector2(directionToPlayer.x, 0).normalized;

            Debug.Log(forceDirection);

            if (forceDirection.x >= 0 && transform.rotation != Quaternion.Euler(0f, 180f, 0f))
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            rigid.velocity = new Vector2(forceDirection.x * moveSpeed, 0);
        }


        if (isGround && ispattern1)

    }

    void ChangeState(Boss1State inputState)
    {
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
                nowState = Boss1State.Pattern2;
                StartCoroutine(Pattern2());
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


    IEnumerator Pattern2()
    {
        rigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        yield return new WaitUntil(() => isGround);

        anim.SetTrigger("Pattern2");
    }


    void DoDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(Random.Range(1, 4));
    }
}

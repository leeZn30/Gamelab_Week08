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
    Rigidbody2D rigid;
    Collider2D collider;

    [Header("점프")]
    [SerializeField] float distance = 3f;
    bool isJump = false;
    [SerializeField] float upForce = 5f;

    public void Do()
    {
        isJump = true;
        Boss.Instance.anim.Play("Jump");
        Vector2 directionToReversePlayer = transform.position - player.position;
        Vector2 targetDirection = new Vector2(directionToReversePlayer.x, upForce);
        rigid.AddForce(targetDirection, ForceMode2D.Impulse);
    }

    public void Stop()
    {
        isJump = false;
        // Boss.Instance.PostCompleion(this);
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < distance && !isJump)
        {
            // Boss.Instance.AskPermission(this);
        }

        Debug.DrawLine(transform.position, Vector2.down, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3f);
        if (hit.collider != null && hit.collider.CompareTag("Ground") && isJump)
        {
            Stop();
        }
    }
}

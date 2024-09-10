using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : MonoBehaviour
{
    [SerializeField] float damage;
    Boss1 boss;

    void Awake()
    {
        boss = FindObjectOfType<Boss1>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.OnAttack(damage);
        }
    }
}

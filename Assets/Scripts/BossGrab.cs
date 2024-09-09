using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGrab : MonoBehaviour
{
    public bool hasGrabed = false;
    [SerializeField] private PlayerController pc;

    private void Start()
    {
        if (pc == null)
            pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasGrabed = true;
        }
    }
}

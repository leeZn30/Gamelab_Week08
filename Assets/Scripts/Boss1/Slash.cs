using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    void Start()
    {

    }

    // IEnumerator Move()
    // {

    // }

    void OnSlashEnd()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected float hp;

    public virtual void OnDamaged(float damage) { }
}

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


[CanBeNull]
public interface Boss1Control
{
    Boss1State State { get; set; }
    int priority { get; set; }
    void Do();
    void Stop();
}

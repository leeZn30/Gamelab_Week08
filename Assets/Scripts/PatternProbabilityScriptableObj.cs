using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pattern Probability", menuName = "Scriptable Object/Pattern Probability List")]
public class PatternProbabilityScriptableObj : ScriptableObject
{
    public string[] PatternName;
    public int[] Probability;
}
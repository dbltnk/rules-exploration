using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rule Object", menuName = "Rule Object")]
public class RuleObject : ScriptableObject
{
    public Condition[] conditions;
    public Result[] results;
    public bool deathRule;
}

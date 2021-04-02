using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rule", menuName = "Rule")]
public class Rule : ScriptableObject
{
    public Condition[] conditions;
    public Result[] results;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NEIGHBOR_STYLE
{
    ALL,
    CARDINAL_ONLY,
    DIAGONAL_ONLY,
}

[CreateAssetMenu(fileName = "New Rule Object", menuName = "Rule Object")]
public class RuleObject : ScriptableObject
{
    public Condition[] conditions;
    public Result[] results;
    public NEIGHBOR_STYLE neighborStyle = NEIGHBOR_STYLE.ALL;
    public bool wallsAreAlive;
    public RULE_CLASSIFICATION classification;
}

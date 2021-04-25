using UnityEngine;

public enum NEIGHBOR_STYLE
{
    ALL,
    CARDINAL_ONLY,
    DIAGONAL_ONLY,
    RANDOM//This must be the last option on the list. Add above this one.
}

public enum RO_WALLS_ALIVE
{
    FALSE,
    TRUE,
    RANDOM//This must be the last option on the list. Add above this one.
}

[CreateAssetMenu(fileName = "New Rule Object", menuName = "Rule Object")]
public class RuleObject : ScriptableObject
{
    public Vector2Int possibleConditionAmounts = new Vector2Int(1, 1);
    public bool matchMySpeciesGroups = true;
    public bool addRandomSpeciesGroups = false;
    public bool randomizeCompareInts = false;
    public Vector2Int compareIntsRandomMinRange = new Vector2Int(0, 3);
    public Vector2Int compareIntsRandomMaxRange = new Vector2Int(3, 8);
    public Condition[] possibleConditions;
    public Result[] possibleResults;
    public NEIGHBOR_STYLE neighborStyle = NEIGHBOR_STYLE.ALL;
    public RO_WALLS_ALIVE wallsAreAlive;
    public RULE_CLASSIFICATION classification;
}

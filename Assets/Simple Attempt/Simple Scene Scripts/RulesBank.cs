using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesBank : MonoBehaviour
{
    [SerializeField] GameManagerScript gameManager;

    [SerializeField] Rule[] birthRules;
    [SerializeField] Rule[] deathRules;

    public Rule GetBirthRule(int index)
    {
        return birthRules[index];
    }

    public Rule GetDeathRule(int index)
    {
        return deathRules[index];
    }

    public Rule GetRandomBirthRule()
    {
        return birthRules[Random.Range(0, birthRules.Length)];
    }

    public Rule GetRandomDeathRule()
    {
        return deathRules[Random.Range(0, deathRules.Length)];
    }

    public int GetIndexOfBirthRule(Rule rule)
    {
        string ruleName = rule.name;

        for(int i = 0; i < birthRules.Length; i++)
        {
            if(ruleName == birthRules[i].name)
            {
                return i;
            }
        }

        Debug.LogError("The given rule was not found within birth rules.");

        return -6;
    }

    public int GetIndexOfDeathRule(Rule rule)
    {
        string ruleName = rule.name;

        for(int i = 0; i < deathRules.Length; i++)
        {
            if(ruleName == deathRules[i].name)
            {
                return i;
            }
        }

        Debug.LogError("The given rule was not found within death rules.");

        return -6;
    }
}

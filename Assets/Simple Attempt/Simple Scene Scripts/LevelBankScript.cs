using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBankScript : MonoBehaviour
{
    [SerializeField] Level[] allLevels = null;

    public void AssignLevels(Level[] levelArray)
    {
        allLevels = levelArray;
    }

    public Level[] GetLevels()
    {
        return allLevels;
    }

    public Level GetLevelByIndex(int index)
    {
        return allLevels[index];
    }

    public Level GetRandomLevel()
    {
        return allLevels[Random.Range(0, allLevels.Length)];
    }
}

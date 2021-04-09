using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelBankScript : MonoBehaviour
{
    [SerializeField] Level[] allLevels = null;

    public void AssignLevels(Level[] levelArray)
    {
        allLevels = levelArray;
        RemoveNullReferences();
    }

    public Level[] GetLevels()
    {
        RemoveNullReferences();
        return allLevels;
    }

    public Level GetLevelByIndex(int index)
    {
        RemoveNullReferences();
        return allLevels[index];
    }

    public Level GetRandomLevel()
    {
        RemoveNullReferences();
        return allLevels[Random.Range(0, allLevels.Length)];
    }

    public void RemoveNullReferences() {
        allLevels = allLevels.Where(t => t != null).ToArray();
    }
}

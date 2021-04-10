using UnityEngine;
using System.Linq;

public class LevelBankScript : MonoBehaviour
{
    [SerializeField] Level[] allLevels = null;

    public void Awake () {
        allLevels = Resources.LoadAll("Levels", typeof (Level)).Cast<Level>().ToArray();
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

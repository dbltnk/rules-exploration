using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesList : MonoBehaviour
{
    public GameObject ListEntry;

    // Start is called before the first frame update
    void Start()
    {
        string allHashes = PlayerPrefs.GetString("KnownSpeciesHashes", null);
        List<string> list = Nickname.StringToList(allHashes, Nickname.Separator);
        foreach (string s in list) {
            GameObject go = Instantiate(ListEntry);
            go.transform.SetParent(transform);
            Text t = go.GetComponentInChildren<Text>();
            string nickName = Nickname.GetNickname(s);
            t.text = s + ":\n" + nickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

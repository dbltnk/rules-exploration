using ProceduralToolkit.Samples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    public Text Hash;
    public Text PlaceholderName;
    public Text NickName;
    private InputField inputField;
    public const string Separator = "$";
    private CellularAutomatonConfigurator cac;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
        cac = FindObjectOfType<CellularAutomatonConfigurator>();
    }

    // Update is called once per frame
    void Update() {
        RenderName();
        cac.IsTyping = inputField.isFocused;
    }

    public void RenderName () {
        string nn = GetNickname(Hash.text);
        if (!inputField.isFocused && nn != NickName.text) {
            if (nn == null || nn == "") {
                PlaceholderName.text = "Name this species";
                inputField.SetTextWithoutNotify("");
            } else {
                PlaceholderName.text = "";
                inputField.SetTextWithoutNotify(nn);
            }
        }
    }

    public void UpdateNickname() {
        PlayerPrefs.SetString(Hash.text, NickName.text);
        RenderName();
        SaveHashAsKnown(Hash.text);
    }

    public static string GetNickname (string hash) {
        return PlayerPrefs.GetString(hash, null);
    }

    private void SaveHashAsKnown (string hash) {
        if (hash == "" || hash == null) return;
        string allHashes = PlayerPrefs.GetString("KnownSpeciesHashes", null);
        List<string> list = StringToList(allHashes, Separator);
        bool hashFound = false;
        foreach (string s in list) {
            if (s == hash) hashFound = true;
        }
        if (!hashFound) PlayerPrefs.SetString("KnownSpeciesHashes", hash + Separator + allHashes);
    }

    public static List<String> StringToList (string message, string seperator) {
        List<string> list = new List<string>();
        string tok = "";
        foreach (char character in message) {
            tok = tok + character;
            if (tok.Contains(seperator)) {
                tok = tok.Replace(seperator, "");
                list.Add(tok);
                tok = "";
            }
        }
        return list;
    }
}

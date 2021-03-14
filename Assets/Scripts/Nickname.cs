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

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update() {
        RenderName();
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
    }

    public string GetNickname (string hash) {
        return PlayerPrefs.GetString(hash, null);
    }
}

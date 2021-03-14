using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionButton : MonoBehaviour
{
    public void ShowCollection() {
        SceneManager.LoadScene("KnownSpeciesCollection", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

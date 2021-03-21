using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionButton : MonoBehaviour
{
    public void ShowCollection() {
        SceneManager.LoadScene("KnownSpeciesCollection", LoadSceneMode.Additive);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphButton : MonoBehaviour
{
    public string SceneToLoad;

    public void LoadScene() {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
    }

    public void LoadSceneAdditive () {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    static public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    static public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    static public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // will break on last scene, maybe loop back to main menu
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    static public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

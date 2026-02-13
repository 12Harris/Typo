using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button options;
    [SerializeField] private Button quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (start != null)
        {
            start.onClick.AddListener(startGame);
        }
        if (options != null)
        {
            options.onClick.AddListener(OpenOptions);
        }
        if (quit != null)
        {
            quit.onClick.AddListener(QuitGame);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void startGame()
    {
        Debug.Log("game start");
        //SceneLoader.LoadNextScene();
        SceneManager.LoadScene("JustType2");
    }
    void OpenOptions()
    {
        Debug.Log("options");
        SceneManager.LoadScene("OptionsPanel");
    }


    void QuitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button back;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (start != null)
        {
            start.onClick.AddListener(startGame);
        }

        if (back != null)
        {
            back.onClick.AddListener(Back);
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


    void Back()
    {
        SceneManager.LoadScene("StartGame");
    }
}

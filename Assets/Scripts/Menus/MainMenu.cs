using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.instance.StartNewGame();
    }

    public void LoadGame()
    {
        GameManager.instance.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void LoadGameButton()
    {
        SaveLoadManager.instance.LoadGame();
    }

    public void NewGameButton()
    {
        SaveLoadManager.instance.StartNewGame();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}

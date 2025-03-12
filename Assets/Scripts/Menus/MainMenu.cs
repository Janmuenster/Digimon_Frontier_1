using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MainMenu : MonoBehaviour
{
    public async void NewGame()
    {
        Debug.Log("Starte neues Spiel...");

        try
        {
            // Initialisiere einen neuen Spielstand
            await GameManager.instance.StartNewGameAsync();

            // Lade die Overworld-Szene
            SceneManager.LoadScene("Overworld");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Fehler beim Starten eines neuen Spiels: {e.Message}");
        }
    }


    public async void LoadGame()
    {
        await GameManager.instance.LoadGameAsync();
    }

    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet.");
        Application.Quit();
    }
}

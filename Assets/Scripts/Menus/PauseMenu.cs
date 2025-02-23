using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI resumeText;
    [SerializeField] TextMeshProUGUI homeText;
    [SerializeField] TextMeshProUGUI optionsText;
    [SerializeField] TextMeshProUGUI saveText; // Neuer Text für den Speicher-Button

    private bool isResumeConfirmation = false;
    private bool isHomeConfirmation = false;
    private bool isOptionsConfirmation = false;
    private bool isSaveConfirmation = false; // Neues Flag für den Speicher-Button

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        ResetAllButtons();
    }

    public void Resume()
    {
        if (!isResumeConfirmation)
        {
            resumeText.text = "Resume?";
            isResumeConfirmation = true;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            ResetAllButtons();
        }
    }

    public void Home()
    {
        if (!isHomeConfirmation)
        {
            homeText.text = "Home?";
            isHomeConfirmation = true;
        }
        else
        {
            SceneManager.LoadScene(1); // Zurück ins Hauptmenü
            Time.timeScale = 1;
            ResetAllButtons();
        }
    }

    public void Options()
    {
        if (!isOptionsConfirmation)
        {
            optionsText.text = "Options?";
            isOptionsConfirmation = true;
        }
        else
        {
            SceneManager.LoadScene(2); // Optionsmenü laden
            Time.timeScale = 1;
            ResetAllButtons();
        }
    }

    public void SaveGame()
    {
        if (!isSaveConfirmation)
        {
            saveText.text = "Save?";
            isSaveConfirmation = true;
        }
        else
        {
            Debug.Log("Save-Button bestätigt, Spiel wird gespeichert...");
            GameManager.instance.SaveGame(); // Spiel speichern
            saveText.text = "Saved!"; // Feedback geben
            Invoke("ResetAllButtons", 1.5f); // Nach 1,5 Sek. zurücksetzen
        }
    }


    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetAllButtons()
    {
        resumeText.text = "Resume";
        homeText.text = "";
        optionsText.text = "";
        saveText.text = "Save"; // Setzt den Speicher-Button zurück

        isResumeConfirmation = false;
        isHomeConfirmation = false;
        isOptionsConfirmation = false;
        isSaveConfirmation = false;
    }
}

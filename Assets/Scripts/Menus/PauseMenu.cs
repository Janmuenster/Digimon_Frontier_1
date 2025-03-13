using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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

    public async void SaveGame()
    {
        if (!isSaveConfirmation)
        {
            saveText.text = "Save?";
            isSaveConfirmation = true;
        }
        else
        {
            Debug.Log("Save-Button bestätigt, Spiel wird gespeichert...");
            saveText.text = "Saving..."; // Feedback während des Speicherns

            try
            {
                await GameManager.instance.SaveGameAsync();
                saveText.text = "Saved!";
                Debug.Log("Spiel erfolgreich gespeichert.");
            }
            catch (Exception e)
            {
                saveText.text = "Save Failed!";
                Debug.LogError($"Fehler beim Speichern des Spiels: {e.Message}");
            }

            Invoke("ResetAllButtons", 1.5f);
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

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI resumeText;
    [SerializeField] TextMeshProUGUI homeText; // Neue Referenz f�r den Home-Button Text
    [SerializeField] TextMeshProUGUI optionsText; 
    private bool isResumeConfirmation = false;
    private bool isHomeConfirmation = false; // Neues Flag f�r den Home-Button

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        ResetAllButtons();
    }
  
    public void Home()
    {
        if (!isHomeConfirmation)
        {
            // Erste Bet�tigung: Zeige Best�tigung
            homeText.text = "Home?";
            isHomeConfirmation = true;
        }
        else
        {
            // Zweite Bet�tigung: F�hre Home-Aktion aus
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
            ResetAllButtons();
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

    public void Options()
    {
           if (!isResumeConfirmation)
        {
            resumeText.text = "Options?";
            isResumeConfirmation = true;
        }
        else
        {
            SceneManager.LoadScene(2);
            Time.timeScale = 1;
            ResetAllButtons();
        }
    }

    private void ResetAllButtons()
    {
        // Setzt den Text und den Zustand aller Buttons zur�ck
        resumeText.text = "Resume";
        homeText.text = "";
        optionsText.text = "";
        isResumeConfirmation = false;
        isHomeConfirmation = false;
    }
}

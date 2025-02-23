using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class StatMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject charcaterPanel;
    [SerializeField] Image charcaterscreenImage;
    [SerializeField] private Image TakuyaImage;
    [SerializeField] private Image ZoeImage;
    [SerializeField] Button[] characterButtons;
    [SerializeField] Button Button;
    //[SerializeField] TextMeshProUGUI resumeText;
    //[SerializeField] TextMeshProUGUI homeText; // Neue Referenz für den Home-Button Text
    //[SerializeField] TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI takuyaText;
    [SerializeField] private TextMeshProUGUI ZoeText;
    //private bool isResumeConfirmation = false;
    //private bool isHomeConfirmation = false; // Neues Flag für den Home-Button
    private bool isCharacterPanelOpen = false;
    public bool isTakuyaActive = false;
    public bool isZoeActive = false;
    public void Stats()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        ResetAllButtons();
    }


    public void Character()
    {
        isCharacterPanelOpen = !isCharacterPanelOpen; // Toggle den Zustand

        charcaterPanel.SetActive(isCharacterPanelOpen);
        charcaterscreenImage.gameObject.SetActive(isCharacterPanelOpen);

        foreach (Button button in characterButtons)
        {
            button.gameObject.SetActive(isCharacterPanelOpen);
        }

        // Optional: Pause das Spiel, wenn das Panel geöffnet wird
        Time.timeScale = isCharacterPanelOpen ? 0 : 1;
    }


    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void CharacterStats()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Button.interactable = true;
        ResetAllButtons();
    }

    public void Takuya()
    {
        isTakuyaActive = !isTakuyaActive; // Toggle den Zustand

        // Aktiviere oder deaktiviere das Image
        if (TakuyaImage != null)
        {
            TakuyaImage.gameObject.SetActive(isTakuyaActive);
        }
        else
        {
            Debug.LogWarning("Takuya Image is not assigned!");
        }

        // Aktiviere oder deaktiviere den Text
        if (takuyaText != null)
        {
            takuyaText.gameObject.SetActive(isTakuyaActive);
        }
        else
        {
            Debug.LogWarning("Takuya Text is not assigned!");
        }
    }
    public void Zoe()
    {
        isZoeActive = !isZoeActive; // Toggle den Zustand

        // Aktiviere oder deaktiviere das Image
        if (ZoeImage != null)
        {
            ZoeImage.gameObject.SetActive(isZoeActive);
        }
        else
        {
            Debug.LogWarning("Zoe Image is not assigned!");
        }

        // Aktiviere oder deaktiviere den Text
        if (ZoeText != null)
        {
            ZoeText.gameObject.SetActive(isZoeActive);
        }
        else
        {
            Debug.LogWarning("Zoe Text is not assigned!");
        }
    }


    private void ResetAllButtons()
    {
        // Setzt den Text und den Zustand aller Buttons zurück
        //resumeText.text = "Resume";
        //homeText.text = "";
        //optionsText.text = "";
        //isResumeConfirmation = false;
        //isHomeConfirmation = false;
    }
    public void Close()
    {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            ResetAllButtons();
}
}

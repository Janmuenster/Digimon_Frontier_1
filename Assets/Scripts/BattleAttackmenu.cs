using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleAttackmenu : MonoBehaviour
{
    public Button toggleButton;     // Referenz auf den Toggle-Button
    public Image imageToShow;       // Referenz auf das Image
    public List<Button> buttonsToShow = new List<Button>();  // Liste von Buttons, die ein-/ausgeblendet werden sollen
    public GameObject parentToShow; // Referenz auf das Parent-GameObject, das ein-/ausgeblendet werden soll

    void Start()
    {
        // Überprüfen, ob die notwendigen Komponenten zugewiesen sind
        if (toggleButton == null || imageToShow == null)
        {
            Debug.LogError("Erforderliche Komponenten fehlen!");
            return;
        }

        // Image initial ausblenden
        imageToShow.enabled = false;

        // Alle Buttons in der Liste ausblenden
        foreach (Button button in buttonsToShow)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }

        // Parent ausblenden, falls eines zugewiesen wurde
        if (parentToShow != null)
        {
            parentToShow.SetActive(false);
        }

        // Listener für den Toggle-Button-Click hinzufügen
        toggleButton.onClick.AddListener(ToggleElements);
    }

    public void ToggleElements()
    {
        // Image ein- oder ausblenden
        imageToShow.enabled = !imageToShow.enabled;

        // Alle Buttons in der Liste ein- oder ausblenden
        foreach (Button button in buttonsToShow)
        {
            if (button != null)
            {
                button.gameObject.SetActive(!button.gameObject.activeSelf);
            }
        }

        // Parent ein- oder ausblenden, falls eines zugewiesen wurde
        if (parentToShow != null)
        {
            parentToShow.SetActive(!parentToShow.activeSelf);
        }
    }
}

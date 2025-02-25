using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject attackPanel;
    public GameObject digivolvePanel;
    public GameObject itemPanel;

    [Header("Buttons")]
    public Button attackButton;
    public Button digivolveButton;
    public Button itemButton;
    public Button fleeButton;

    private GameObject activePanel = null;
    private bool isBossBattle = false; // Setze dies entsprechend der Kampfsituation

    void Start()
    {
        attackButton.onClick.AddListener(() => TogglePanel(attackPanel));
        digivolveButton.onClick.AddListener(() => TogglePanel(digivolvePanel));
        itemButton.onClick.AddListener(() => TogglePanel(itemPanel));
        fleeButton.onClick.AddListener(TryToFlee);
    }

    void TogglePanel(GameObject panel)
    {
        if (activePanel == panel)
        {
            panel.SetActive(!panel.activeSelf);
            if (!panel.activeSelf)
                activePanel = null;
        }
        else
        {
            if (activePanel != null)
                activePanel.SetActive(false);

            panel.SetActive(true);
            activePanel = panel;
        }
    }

    void TryToFlee()
    {
        if (isBossBattle)
        {
            Debug.Log("Flucht ist in Bosskämpfen nicht möglich!");
        }
        else
        {
            Debug.Log("Fluchtversuch!");
            // Implementiere Flucht-Logik hier
        }
    }
    public void CloseAllPanels()
    {
        attackPanel.SetActive(false);
        itemPanel.SetActive(false);
        digivolvePanel.SetActive(false);
    }
}

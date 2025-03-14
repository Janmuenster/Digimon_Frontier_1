using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StatsMenu : MonoBehaviour
{
    public GameObject statsPanel;
    public TextMeshProUGUI[] statTexts;
    public TextMeshProUGUI characterNameText;
    public List<CharacterStats> allCharacters = new List<CharacterStats>();
    public Button nextButton;
    public Button previousButton;

    private int currentCharacterIndex = 0;
    private bool isStatsOpen = false;

    void Start()
    {
        statsPanel.SetActive(false);
        UpdateNavigationButtons();
        nextButton.onClick.AddListener(NextCharacter);
        previousButton.onClick.AddListener(PreviousCharacter);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatsMenu();
        }

        if (isStatsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Q)) PreviousCharacter();
            if (Input.GetKeyDown(KeyCode.E)) NextCharacter();
        }
    }

    public void ToggleStatsMenu()
    {
        isStatsOpen = !isStatsOpen;
        statsPanel.SetActive(isStatsOpen);
        if (isStatsOpen) UpdateDisplay();
    }

    void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % allCharacters.Count;
        UpdateDisplay();
    }

    void PreviousCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0) currentCharacterIndex = allCharacters.Count - 1;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (allCharacters.Count == 0) return;

        var currentStats = allCharacters[currentCharacterIndex];
        characterNameText.text = currentStats.characterName;

        statTexts[0].text = "Level: " + currentStats.level;
        statTexts[1].text = "HP: " + currentStats.currentHP + "/" + currentStats.maxHP;
        statTexts[2].text = "Speed: " + currentStats.speed;
        statTexts[3].text = "Attack: " + currentStats.attack;
        statTexts[4].text = "Xp: " + currentStats.xp;
        statTexts[5].text = "Xp needed: " + currentStats.xpToNextLevel;

        UpdateNavigationButtons();
    }

    void UpdateNavigationButtons()
    {
        previousButton.interactable = allCharacters.Count > 1;
        nextButton.interactable = allCharacters.Count > 1;
    }
}

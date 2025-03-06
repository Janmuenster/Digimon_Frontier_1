using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    public GameObject attackButton; // Referenz auf die Angriffs-Schaltfläche

    [Header("Spieler UI Elemente")]
    public List<CharacterUI> playerUIElements;

    [Header("Gegner UI Elemente")]
    public List<CharacterUI> enemyUIElements;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetupUI(List<CharacterStats> playerTeam, List<CharacterStats> enemyTeam)
    {

        for (int i = 0; i < playerUIElements.Count; i++)
        {
            if (i < playerTeam.Count)
            {
                playerUIElements[i].Setup(playerTeam[i]);
            }
            else
            {
                playerUIElements[i].Hide();
            }
        }

        for (int i = 0; i < enemyUIElements.Count; i++)
        {
            if (i < enemyTeam.Count)
            {
                enemyUIElements[i].Setup(enemyTeam[i]);
            }
            else
            {
                enemyUIElements[i].Hide();
            }
            Debug.Log("SetupUI wird ausgeführt. Spieler: " + playerTeam[i].characterName + ", Level: " + playerTeam[i].level);

        }
    }


    public void UpdateCharacterUI(CharacterStats character)
    {
        foreach (var ui in playerUIElements)
        {
            if (ui.character == character)
            {
                ui.UpdateUI();
                return;
            }
        }
        foreach (var ui in enemyUIElements)
        {
            if (ui.character == character)
            {
                ui.UpdateUI();
                return;
            }
        }
    }


    public void ShowAttackButton(bool show)
    {
        // Zeige oder verstecke die Angriffs-Schaltfläche
        attackButton.SetActive(show);
    }

    public void OnAttackButtonClicked()
    {
        // Wenn der Angriffsbutton geklickt wird, führe den Angriff aus
        BattleManager1.instance.PlayerAttack(); // Hier rufst du die PlayerAttack-Methode auf
    }

    [System.Serializable]
    public class CharacterUI
    {
        public GameObject uiPanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public Image hpBar; // Ändere Slider zu Image
        public Slider xpSlider;
        public CharacterStats character;

        public void Setup(CharacterStats character)
        {
            this.character = character;
            uiPanel.SetActive(true);
            nameText.text = character.characterName;
            levelText.text = "Lvl: " + character.level;

            UpdateHealthBar();

            if (xpSlider != null)
            {
                xpSlider.maxValue = character.xpToNextLevel;
                xpSlider.value = character.xp;
            }
        }

        public void UpdateUI()
        {
            if (character == null) return;
            nameText.text = character.characterName;
            levelText.text = "Lvl: " + character.level;
            UpdateHealthBar();

            if (xpSlider != null)
            {
                xpSlider.value = character.xp;
            }
        }

        private void UpdateHealthBar()
        {
            if (hpBar != null)
            {
                hpBar.fillAmount = (float)character.currentHP / character.maxHP;
            }
        }

        public void Hide()
        {
            uiPanel.SetActive(false);
        }
    }
}

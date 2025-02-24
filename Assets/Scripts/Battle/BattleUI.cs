using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHPBar;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public Slider playerHPBar;
    public Transform attackButtonPanel; // Hier kommen die Buttons hin
    public GameObject attackButtonPrefab; // UI-Button Vorlage
    public GameObject digivolveButton;
    private CharacterStats currentPlayer;


    public void SetupUI(List<CharacterStats> playerTeam, List<CharacterStats> enemyTeam)
    {
        Debug.Log($"playerTeam Count: {playerTeam.Count}");
        if (playerTeam.Count > 0)
        {
            Debug.Log($"Player gefunden: {playerTeam[0].characterName}, HP: {playerTeam[0].currentHP}/{playerTeam[0].maxHP}");
            if (playerTeam == null)
            {
                Debug.LogError("Fehler: player ist NULL!");
                return;
            }

        }
        else
        {
            Debug.LogError("Fehler: playerTeam ist leer!");
        }

        if (playerTeam.Count > 0 && enemyTeam.Count > 0)
        {
            CharacterStats player = playerTeam[0];
            CharacterStats enemy = enemyTeam[0];

            // Spielerinformationen setzen
            playerNameText.text = player.characterName + " - Lv. " + player.level;
            playerHPText.text = "HP: " + player.currentHP + "/" + player.maxHP;
            playerHPBar.maxValue = player.maxHP;  // Maximalwert für HP-Bar setzen
            playerHPBar.value = player.currentHP; // Aktuellen Wert setzen

            // Gegnerinformationen setzen
            enemyNameText.text = enemy.characterName + " - Lv. " + enemy.level;
            enemyHPText.text = "HP: " + enemy.currentHP + "/" + enemy.maxHP;
            enemyHPBar.maxValue = enemy.maxHP;
            enemyHPBar.value = enemy.currentHP;

            Debug.Log($"Spieler: {player.characterName} (HP: {player.currentHP}/{player.maxHP})");
            Debug.Log($"Gegner: {enemy.characterName} (HP: {enemy.currentHP}/{enemy.maxHP})");

            Debug.Log($"Setting Player Name: {player.characterName}");
            Debug.Log($"Setting Player HP: {player.currentHP}/{player.maxHP}");
            Debug.Log($"Setting Enemy Name: {enemy.characterName}");
            Debug.Log($"Setting Enemy HP: {enemy.currentHP}/{enemy.maxHP}");

        }
        else
        {
            Debug.LogError("SetupUI: PlayerTeam oder EnemyTeam ist leer!");
        }
    }


public void ShowBattleOptions(CharacterStats player)
    {
        digivolveButton.SetActive(!player.isDigitized); // Zeigt den Button nur, wenn der Spieler noch nicht digitisiert ist
    }

    public void OnDigivolveButtonPressed()
    {
        BattleManager.instance.DigivolvePlayer();
    }
    public void ShowAttackOptions(CharacterStats player)
    {
        currentPlayer = player;

        foreach (Transform child in attackButtonPanel)
            Destroy(child.gameObject);

        foreach (Attack attack in player.attacks)
        {
            GameObject button = Instantiate(attackButtonPrefab, attackButtonPanel);
            button.GetComponentInChildren<Text>().text = attack.attackName;
            button.GetComponent<Button>().onClick.AddListener(() => SelectAttack(attack));
        }
    }

    void SelectAttack(Attack attack)
    {
        BattleManager.instance.PlayerAttack(currentPlayer, BattleManager.instance.enemyTeam[0], attack);
    }
}

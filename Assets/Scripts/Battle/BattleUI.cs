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
        if (playerTeam.Count > 0 && enemyTeam.Count > 0)
        {
            playerNameText.text = playerTeam[0].characterName;
            playerHPText.text = "HP: " + playerTeam[0].currentHP + "/" + playerTeam[0].maxHP;

            enemyNameText.text = enemyTeam[0].characterName;
            enemyHPText.text = "HP: " + enemyTeam[0].currentHP + "/" + enemyTeam[0].maxHP;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public BattleUI battleUI;

    [Header("Spieler & Gegner")]
    public CharacterStats currentPlayerTurn;
    public CharacterStats enemy;

    public List<CharacterStats> playerTeam;  // Deine aktiven Charaktere
    public List<CharacterStats> enemyTeam;   // Gegner im Kampf

    private int currentTurn = 0;
    private bool playerTurn = true; // Startet mit der Spielerseite

    [Header("UI-Elemente")]
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHPBar;
    public TextMeshProUGUI playerNameText;
    public Slider playerHPBar;

    [Header("Attacken-Buttons")]
    public Button attack1Button;
    public Button attack2Button;
    public Button attack3Button;
    public Button attack4Button;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SetupBattle();
    }

    void SetupBattle()
    {
        if (playerTeam.Count == 0 || enemyTeam.Count == 0)
        {
            Debug.LogError("Fehler: Spieler- oder Gegner-Team ist leer!");
            return; // Beende die Methode, um Abstürze zu vermeiden
        }
        currentPlayerTurn = playerTeam[0]; // Sicherstellen, dass es ein gültiges Element gibt
        enemy = enemyTeam[0];

        battleUI.SetupUI(playerTeam, enemyTeam);
        Debug.Log("Kampf beginnt mit " + currentPlayerTurn.characterName + " gegen " + enemy.characterName);
    
    Debug.Log("Kampf beginnt!");
        currentTurn = 0;
        playerTurn = true;

        enemy = enemyTeam[0];  // Gegner aus der Liste auswählen
        currentPlayerTurn = playerTeam[0];  // Spielercharakter auswählen

        // UI setzen
        enemyNameText.text = enemy.characterName;
        enemyHPBar.maxValue = enemy.maxHP;
        enemyHPBar.value = enemy.currentHP;

        playerNameText.text = currentPlayerTurn.characterName;
        playerHPBar.maxValue = currentPlayerTurn.maxHP;
        playerHPBar.value = currentPlayerTurn.currentHP;

        // Attacken-Buttons setzen
        attack1Button.GetComponentInChildren<TextMeshProUGUI>().text = currentPlayerTurn.attacks[0].attackName;
        attack2Button.GetComponentInChildren<TextMeshProUGUI>().text = currentPlayerTurn.attacks[1].attackName;

        attack1Button.onClick.AddListener(() => PlayerAttack(currentPlayerTurn, enemy, currentPlayerTurn.attacks[0]));
        attack2Button.onClick.AddListener(() => PlayerAttack(currentPlayerTurn, enemy, currentPlayerTurn.attacks[1]));

        StartCoroutine(NextTurn());
    }

    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(1f);

        if (playerTurn)
        {
            Debug.Log("Spieler ist am Zug.");
        }
        else
        {
            Debug.Log("Gegner ist am Zug.");
            StartCoroutine(EnemyTurn());
        }
    }

    public void PlayerAttack(CharacterStats attacker, CharacterStats target, Attack chosenAttack)
    {
        ElementType targetElement;
        if (!System.Enum.TryParse(target.element, out targetElement))
        {
            Debug.LogError("Ungültiges Element: " + target.element);
            targetElement = ElementType.Free; // Falls Fehler, setze Standardwert
        }

        float elementMultiplier = GetElementMultiplier(chosenAttack.element, targetElement);
        int finalDamage = Mathf.RoundToInt(chosenAttack.baseDamage * elementMultiplier);

        target.TakeDamage(finalDamage);
        enemyHPBar.value = target.currentHP;

        Debug.Log(attacker.characterName + " benutzt " + chosenAttack.attackName + " gegen " + target.characterName +
                  "! Schaden: " + finalDamage + " (x" + elementMultiplier + ")");

        if (target.currentHP <= 0)
        {
            EndBattle(true);
        }
        else
        {
            NextRound();
        }
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        int damage = enemy.attack - currentPlayerTurn.defense / 2;
        if (damage < 1) damage = 1;

        currentPlayerTurn.TakeDamage(damage);
        playerHPBar.value = currentPlayerTurn.currentHP;

        Debug.Log(enemy.characterName + " greift an! Schaden: " + damage);

        if (currentPlayerTurn.currentHP <= 0)
        {
            EndBattle(false);
        }
        else
        {
            NextRound();
        }
    }

    void NextRound()
    {
        currentTurn++;

        // Prüfen, ob Kampf vorbei ist
        if (CheckWinCondition())
        {
            return;
        }

        playerTurn = !playerTurn; // Spieler- und Gegnerzug wechseln
        StartCoroutine(NextTurn());
    }

    bool CheckWinCondition()
    {
        if (playerTeam.Count == 0)
        {
            Debug.Log("Spieler haben verloren!");
            return true;
        }
        if (enemyTeam.Count == 0)
        {
            Debug.Log("Spieler haben gewonnen!");
            return true;
        }
        return false;
    }

    public void DigivolvePlayer()
    {
        if (currentPlayerTurn != null)
        {
            currentPlayerTurn.ToggleDigitation(true);
            battleUI.ShowBattleOptions(currentPlayerTurn); // UI-Update nach Digitation
        }
    }
    public void OnAttackButtonPressed()
    {
        Debug.Log("Attack-Button wurde geklickt!");

        if (currentPlayerTurn == null || enemy == null)
        {
            Debug.LogError("Fehler: currentPlayerTurn oder enemy ist null!");
            return;
        }

        if (currentPlayerTurn.attacks.Count == 0)
        {
            Debug.LogError("Fehler: currentPlayerTurn hat keine Attacken!");
            return;
        }

        PlayerAttack(currentPlayerTurn, enemy, currentPlayerTurn.attacks[0]); // Nutzt die erste Attacke in der Liste
    }

    void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Spieler hat gewonnen!" : "Spieler hat verloren!");

        // Digitation zurücksetzen
        foreach (CharacterStats player in playerTeam)
        {
            if (player.isDigitized)
                player.ToggleDigitation(false);
        }

        SceneManager.LoadScene("Overworld"); // Zurück zur Overworld
    }

    public float GetElementMultiplier(ElementType attacker, ElementType defender)
    {
        if (attacker == ElementType.Fire && defender == ElementType.Water) return 0.5f;
        if (attacker == ElementType.Water && defender == ElementType.Fire) return 2f;
        if (attacker == ElementType.Electric && defender == ElementType.Water) return 2f;
        if (attacker == ElementType.Earth && defender == ElementType.Air) return 2f;
        return 1f; // Standardfall: Kein Bonus oder Malus
    }
}

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
    public BattleUIManager battleUIManager;

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
        Debug.Log("Spieler-Team Größe: " + playerTeam.Count);
        Debug.Log("Gegner-Team Größe: " + enemyTeam.Count);
        SetupBattle();
    }
    void Update()
    {
        if (playerTeam.Count == 0)
        {
            Debug.LogError("Achtung: playerTeam wurde geleert!");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentPlayerTurn != null && currentPlayerTurn.isDigitized)
            {
                Debug.Log("R-Taste gedrückt: Rückdigitation wird eingeleitet.");
                currentPlayerTurn.ToggleDigitation(false);
                UpdateUIAfterDigivolution(); // UI nach Rückdigitieren aktualisieren
            }
        }
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

        Debug.Log("Aktueller Spieler: " + (currentPlayerTurn != null ? currentPlayerTurn.characterName : "NULL"));
        Debug.Log("HP des aktuellen Spielers: " + (currentPlayerTurn != null ? currentPlayerTurn.currentHP.ToString() : "NULL"));


        Debug.Log("Aufruf von battleUI.SetupUI()");
        StartCoroutine(DelayedSetupUI());

        Debug.Log("Kampf beginnt mit " + currentPlayerTurn.characterName + " gegen " + enemy.characterName);
    
    Debug.Log("Kampf beginnt!");
        currentTurn = 0;
        playerTurn = true;



        // Lade gespeicherte HP
        if (PlayerPrefs.HasKey("CurrentHP_" + currentPlayerTurn.characterName))
        {
            currentPlayerTurn.currentHP = PlayerPrefs.GetInt("CurrentHP_" + currentPlayerTurn.characterName);
            currentPlayerTurn.maxHP = PlayerPrefs.GetInt("MaxHP_" + currentPlayerTurn.characterName);
            Debug.Log("Gespeicherte HP geladen für " + currentPlayerTurn.characterName + ": " + currentPlayerTurn.currentHP + "/" + currentPlayerTurn.maxHP);
        }
        else
        {
            currentPlayerTurn.currentHP = currentPlayerTurn.maxHP;
            Debug.Log("Keine gespeicherten HP gefunden. Setze auf: " + currentPlayerTurn.maxHP);
        }

        // UI setzen
        enemyNameText.text = enemy.characterName;
        enemyHPBar.maxValue = enemy.maxHP;
        enemyHPBar.value = enemy.currentHP;

        playerNameText.text = currentPlayerTurn.characterName;
        playerHPBar.maxValue = currentPlayerTurn.maxHP;
        playerHPBar.value = currentPlayerTurn.currentHP;

        // Attacken-Buttons setzen
        if (currentPlayerTurn.attacks.Count > 0)
        {
            attack1Button.GetComponentInChildren<TextMeshProUGUI>().text = currentPlayerTurn.attacks[0].attackName;
            attack1Button.onClick.RemoveAllListeners();
            attack1Button.onClick.AddListener(() => PlayerAttack(currentPlayerTurn, enemy, currentPlayerTurn.attacks[0]));
        }
        if (currentPlayerTurn.attacks.Count > 1)
        {
            attack2Button.GetComponentInChildren<TextMeshProUGUI>().text = currentPlayerTurn.attacks[1].attackName;
            attack1Button.onClick.RemoveAllListeners();
            attack2Button.onClick.AddListener(() => PlayerAttack(currentPlayerTurn, enemy, currentPlayerTurn.attacks[1]));
        }

        
        StartCoroutine(NextTurn());
    }

    IEnumerator DelayedSetupUI()
    {
        yield return null; // Warten, bis Werte gesetzt sind
        battleUI.SetupUI(playerTeam, enemyTeam);
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
            playerTurn = false; // Gegner ist jetzt dran!
            StartCoroutine(NextTurn());
            battleUIManager.CloseAllPanels();
        }
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f); // Kurz warten, bevor Gegner angreift

        if (enemy.attacks.Count > 0)
        {
            // Zufällige Attacke des Gegners auswählen
            Attack chosenAttack = enemy.attacks[Random.Range(0, enemy.attacks.Count)];
            int damage = Mathf.RoundToInt(chosenAttack.baseDamage);

            Debug.Log(enemy.characterName + " benutzt " + chosenAttack.attackName + " gegen " + currentPlayerTurn.characterName + "! Schaden: " + damage);
            currentPlayerTurn.TakeDamage(damage);
        }
        else
        {
            // Standardangriff falls keine Attacken definiert sind
            int damage = enemy.attack - currentPlayerTurn.defense / 2;
            if (damage < 1) damage = 1;

            Debug.Log(enemy.characterName + " greift normal an! Schaden: " + damage);
            currentPlayerTurn.TakeDamage(damage);
        }

        playerHPBar.value = currentPlayerTurn.currentHP; // HP-Bar updaten

        if (currentPlayerTurn.currentHP <= 0)
        {
            EndBattle(false);
        }
        else
        {
            playerTurn = true; // Spieler ist wieder dran
            StartCoroutine(NextTurn());
        }
    }
    public void UpdateHPUI(CharacterStats character)
    {
        if (character == null) return;

        // Hier müsstest du dein konkretes UI-Element updaten. 
        // Angenommen du hast eine HP-Bar als Slider:
        if (character.characterName == "Takuya") // Falls du speziell den Main-Charakter meinst
        {
            playerHPBar.value = (float)character.currentHP / character.maxHP;
        }

        Debug.Log(character.characterName + " HP aktualisiert: " + character.currentHP + "/" + character.maxHP);
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
        bool allPlayersDefeated = playerTeam.TrueForAll(player => player.currentHP <= 0);
        bool allEnemiesDefeated = enemyTeam.TrueForAll(enemy => enemy.currentHP <= 0);

        if (allPlayersDefeated)
        {
            Debug.Log("Spieler haben verloren!");
            EndBattle(false);
            return true;
        }
        if (allEnemiesDefeated)
        {
            Debug.Log("Spieler haben gewonnen!");
            EndBattle(true);
            return true;
        }
        return false;
    }


    public void DigivolvePlayer()
    {
        if (currentPlayerTurn != null)
        {
            currentPlayerTurn.ToggleDigitation(true);
            battleUI.ShowBattleOptions(currentPlayerTurn); // UI updaten
            battleUIManager.CloseAllPanels();

            // UI nach Digitation aktualisieren
            UpdateUIAfterDigivolution();
        }
    }


    public void UpdateUIAfterDigivolution()
    {
        if (currentPlayerTurn != null)
        {
            // UI mit den neuen Werten updaten
            battleUI.SetupUI(playerTeam, enemyTeam);
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

    IEnumerator EndBattleCoroutine(bool playerWon)
    {
        Debug.Log(playerWon ? "Spieler hat gewonnen!" : "Spieler hat verloren!");

        // Falls Spieler gewonnen hat, XP geben
        if (playerWon)
        {
            foreach (CharacterStats player in playerTeam)
            {
                player.GainXP(50); // Hier könnte eine variable XP-Belohnung rein
            }
        }

        yield return new WaitForSeconds(2f); // Kurze Wartezeit für Effekte

        SceneManager.LoadScene("Overworld"); // Zurück zur Overworld
    }
    void EndBattle(bool playerWon)
    {
        StartCoroutine(EndBattleCoroutine(playerWon));

        // Speichern der aktuellen HP vor dem Szenenwechsel
        SavePlayerHP();

        // Speichern der restlichen Charakterdaten
        SaveCharacterStats();
    }
    private void SavePlayerHP()
    {
        foreach (CharacterStats player in playerTeam)
        {
            PlayerPrefs.SetInt("CurrentHP_" + player.characterName, player.currentHP);
            PlayerPrefs.SetInt("MaxHP_" + player.characterName, player.maxHP);
            PlayerPrefs.Save();
            Debug.Log("HP gespeichert: " + player.characterName + " -> " + player.currentHP + "/" + player.maxHP);
        }
    }
    private void SaveCharacterStats()
    {
        // Speichern der aktuellen Charakterdaten (z.B. nach dem Kampf)
        foreach (CharacterStats player in playerTeam)
        {
            // Hier könntest du alle relevanten Daten wie XP, Level, HP, etc. speichern
            Debug.Log("Speichern von: " + player.characterName);
            SaveData data = new SaveData();
            data.characterName = player.characterName;
            data.level = player.level;
            data.maxHP = player.maxHP;
            data.currentHP = player.currentHP;
            data.attack = player.attack;
            data.defense = player.defense;
            data.element = player.element;
            data.type = player.type;
            data.xp = player.xp;
            data.xpToNextLevel = player.xpToNextLevel;

            // Speichern mit dem SaveManager
            SaveManager.instance.SaveGame(data);
        }
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

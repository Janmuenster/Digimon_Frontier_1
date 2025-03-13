using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import für TextMeshPro
using UnityEngine.UI; // Wichtig für UI-Elemente!
using UnityEngine.SceneManagement;

public class BattleManager1 : MonoBehaviour
{
    public static BattleManager1 instance;
    public BattleUIManager battleUIManager;
    public TextMeshProUGUI turnIndicatorText;  // Das TextMeshPro-UI-Element, das den aktuellen Zug anzeigt

    // UI Buttons
    public Button attackButton;
    public Button defendButton;
    public Button skillButton;

    // UI Panels (Fenster für Aktionen)
    public GameObject attackPanel;
    public GameObject defendPanel;
    public GameObject skillPanel;

    private int currentTurn = 0; // Gibt an, wessen Zug es gerade ist (0 = Spieler, 1 = Gegner)
    private bool isPlayerTurn = true; // Gibt an, ob der Spieler am Zug ist

    private bool isBossFight;

    public List<GameObject> enemyPrefabs; // Liste von Gegner-Prefabs
    public List<Transform> enemySpawnPositions; // Liste von Positionen, an denen die Gegner spawnen sollen.
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public List<GameObject> selectedPlayers = new List<GameObject>(); // Die 3 aktiven Kämpfer
    private List<GameObject> playerObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();

    private string currentArea; // Variable, die das aktuelle Gebiet speichert
    private CharacterStats savedCharacterStats;

    void Start()
    {
        StartBattle();

        // Event Listener für Buttons
        attackButton.onClick.AddListener(() => TogglePanel(attackPanel));
        defendButton.onClick.AddListener(() => TogglePanel(defendPanel));
        skillButton.onClick.AddListener(() => TogglePanel(skillPanel));
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void StartBattle()
    {
        Debug.Log("StartBattle wird aufgerufen."); // Debug-Log
        currentArea = GameManager.instance.GetCurrentArea();  // Hole das aktuelle Gebiet vom GameManager
        Debug.Log("Aktuelles Gebiet: " + currentArea);

        selectedPlayers = new List<GameObject>(PartyManager.instance.battleParty);
        GameManager.instance.SetEnemySpawnPositions(enemySpawnPositions);

        LoadEnemiesForBattle();

        // Prüfe, ob Spawn-Positionen korrekt zugewiesen wurden
        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen für Gegner zugewiesen!");
        }
        else
        {
            Debug.Log("Anzahl der Spawn-Positionen: " + enemySpawnPositions.Count);
        }

        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Keine Gegner-Prefabs gefunden!");
            return;
        }
        Debug.Log("Fehler durch enemyPrefabs, aber nach Aufruf von LoadEnemiesForBattle.");
        SpawnCharacters(); // Gegner und Spieler spawnen
    }

    void LoadEnemiesForBattle()
    {
        List<GameObject> enemies = EnemyManager.instance.GetEnemiesForBattle(GameManager.BattleType.WildBattle, currentArea);

        if (enemies.Count > 0)
        {
            enemyPrefabs = new List<GameObject>(enemies);
            Debug.Log("Gegner erfolgreich in enemyPrefabs gespeichert! Anzahl: " + enemyPrefabs.Count);
        }
        else
        {
            Debug.LogError("Keine Gegner für das Gebiet " + currentArea + " gefunden!");
        }
    }

    void SpawnCharacters()
    {
        Debug.Log("SpawnCharacters wird aufgerufen."); // Debug-Log

        Debug.Log("Anzahl der ausgewählten Spieler: " + GameManager.instance.selectedPlayerPrefabs.Count);

        // Spieler instanziieren
        for (int i = 0; i < GameManager.instance.selectedPlayerPrefabs.Count; i++)
        {
            GameObject playerPrefab = GameManager.instance.selectedPlayerPrefabs[i];

            // Prefab direkt instanziieren
            Vector3 spawnPos = playerSpawnPoint.position + new Vector3(i * 2.0f, 0, 0);
            GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            if (newPlayer == null)
            {
                Debug.LogError("Spieler-Instanziierung fehlgeschlagen für: " + playerPrefab.name);
            }
            else
            {
                Debug.Log("Spieler erfolgreich gespawnt: " + newPlayer.name);
            }

            // Charakterdaten setzen, aber nur, wenn nicht schon vorhanden
            CharacterStats stats = newPlayer.GetComponent<CharacterStats>();
            if (stats == null)
            {
                Debug.LogError("CharacterStats fehlt bei: " + newPlayer.name);
            }
            else
            {
                Character character = stats.character;
                if (newPlayer.GetComponent<CharacterDisplay>() != null)
                {
                    newPlayer.GetComponent<CharacterDisplay>().SetCharacter(character);
                }
                // Füge Spieler zu einer Liste hinzu
                playerObjects.Add(newPlayer);
            }
        }

        // Gegner spawnen
        SpawnRandomEnemies();

        // Nachdem alle Charaktere gespawnt sind, initialisiere die UI
        StartCoroutine(DelayedUIInitialization());
    }

    IEnumerator DelayedUIInitialization()
    {
        // Warte einen Frame, um sicherzustellen, dass die Charakterwerte gesetzt sind
        yield return null;

        // Initialisiere die UI nach der Verzögerung
        InitializeBattleTeams();
    }

    void InitializeBattleTeams()
    {
        // Initialisiere die Listen playerTeam und enemyTeam
        List<CharacterStats> playerTeam = new List<CharacterStats>();
        foreach (var player in playerObjects)
        {
            CharacterStats stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                playerTeam.Add(stats); // Füge den Charakter zum Spielerteam hinzu
            }
        }

        List<CharacterStats> enemyTeam = new List<CharacterStats>();
        foreach (var enemy in enemyObjects)
        {
            CharacterStats stats = enemy.GetComponent<CharacterStats>();
            if (stats != null)
            {
                enemyTeam.Add(stats); // Füge den Gegner zum Gegnerteam hinzu
            }
        }

        // Rufe SetupUI mit den richtigen Teams auf
        battleUIManager.SetupUI(playerTeam, enemyTeam);
    }

    void SpawnRandomEnemies()
    {
        Debug.Log("Spawnenemies wird aufgerufen");

        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen zugewiesen!");
            return;
        }

        // Hole die normalen Gegner aus dem EnemyManager basierend auf dem Gebiet
        List<GameObject> wildEnemiesForArea = EnemyManager.instance.GetEnemiesForBattle(GameManager.BattleType.WildBattle, currentArea);

        // Debugging: Prüfen, ob Gegner gefunden wurden
        if (wildEnemiesForArea.Count == 0)
        {
            Debug.LogError("Keine normalen Gegner im aktuellen Gebiet zugewiesen! Gebiet: " + currentArea);
            return; // Kein Spawn von Gegnern, wenn keine gefunden wurden
        }

        // Prüfen, ob genügend Spawn-Positionen vorhanden sind
        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen zugewiesen!");
            return; // Kein Spawn, wenn keine Positionen da sind
        }

        // Zufällige Anzahl an Gegnern (z.B. zwischen 1 und 3) nachdem .Min( muss eine 4 für 3 random gegner
        int enemyCount = Random.Range(1, Mathf.Min(2, enemySpawnPositions.Count + 1));
        Debug.Log("Anzahl der zu spawnenden Gegner: " + enemyCount);

        // Stelle sicher, dass die Positionen gemischt werden
        List<Transform> availableSpawnPositions = new List<Transform>(enemySpawnPositions);
        ShuffleList(availableSpawnPositions); // Positionen mischen

        // Spawn der Gegner
        for (int i = 0; i < enemyCount; i++)
        {
            int randomIndex = Random.Range(0, wildEnemiesForArea.Count);
            GameObject enemyPrefab = wildEnemiesForArea[randomIndex];

            if (enemyPrefab == null)
            {
                Debug.LogError("Fehler: Das Gegner-Prefab ist null! Überprüfe die Gegnerliste.");
                continue; // Falls das Prefab null ist, überspringen
            }

            Transform spawnPoint = availableSpawnPositions[i];
            Debug.Log("Gegner wird gespawnt: " + enemyPrefab.name + " an Position: " + spawnPoint.position);

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            if (newEnemy == null)
            {
                Debug.LogError("Fehler beim Instanziieren des Gegners!");
            }
            else
            {
                Debug.Log("Gegner erfolgreich gespawnt: " + newEnemy.name);
                enemyObjects.Add(newEnemy);
            }
        }
    }

    // Methode zum Mischen der Liste (Fisher-Yates Algorithmus)
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void StartTurn()
    {
        if (isPlayerTurn)
        {
            battleUIManager.ShowAttackButton(true);
            turnIndicatorText.text = "Spieler ist am Zug";
        }
        else
        {
            ExecuteEnemyTurn();
            turnIndicatorText.text = "Gegner ist am Zug";
        }
    }

    // **Fenster umschalten, sodass immer nur eines aktiv ist**
    private void TogglePanel(GameObject panelToToggle)
    {
        // Falls das gewünschte Panel bereits aktiv ist, dann schließen
        if (panelToToggle.activeSelf)
        {
            panelToToggle.SetActive(false);
        }
        else
        {
            CloseAllPanels(); // Erst alle schließen
            panelToToggle.SetActive(true); // Dann nur das gewünschte öffnen
        }
    }


    // **Alle Aktionsfenster schließen**
    private void CloseAllPanels()
    {
        attackPanel.SetActive(false);
        defendPanel.SetActive(false);
        skillPanel.SetActive(false);
    }
    // Methode für den Spielerangriff
    public void PlayerAttack()
    {
        if (isPlayerTurn)
        {
            // Spieler greift an
            ExecuteAttack(selectedPlayers[0], enemyObjects[0]);

            // Nach dem Angriff ist der Zug vorbei und der Gegner ist dran
            isPlayerTurn = false;
            ExecuteAction(); // Fenster schließen nach der Aktion
            EndTurn();
        }
    }

    public void ExecuteAction()
    {
        CloseAllPanels(); // Fenster schließen
    }


    // Methode zum Ausführen des Angriffs
    void ExecuteAttack(GameObject attacker, GameObject defender)
    {
        CharacterStats attackerStats = attacker.GetComponent<CharacterStats>();
        CharacterStats defenderStats = defender.GetComponent<CharacterStats>();

        // Angriffs-Logik: Schaden berechnen
        int damage = CalculateDamage(attackerStats, defenderStats);

        // Schaden anwenden
        defenderStats.TakeDamage(damage);

        // Aktualisiere die UI
        battleUIManager.UpdateCharacterUI(defenderStats);

        Debug.Log($"{attackerStats.characterName} hat {defenderStats.characterName} {damage} Schaden zugefügt!");
    }

    // Beispiel für die Schadensberechnung
    int CalculateDamage(CharacterStats attacker, CharacterStats defender)
    {
        // Beispielhafte Berechnung des Schadens
        int damage = attacker.attack - defender.defense;
        return Mathf.Max(damage, 0); // Schaden kann nicht negativ sein
    }

    // Methode, die das Ende des Zuges behandelt
    void EndTurn()
    {
        // Zum nächsten Zug wechseln
        currentTurn++;
        if (currentTurn % 2 == 0)
        {
            isPlayerTurn = true; // Spieler ist wieder dran
        }
        else
        {
            isPlayerTurn = false; // Gegner ist dran
        }

        // Starte den nächsten Zug
        StartTurn();
    }

    // Methode, die den Gegnerzug ausführt
    void ExecuteEnemyTurn()
    {
        if (enemyObjects.Count == 0 || playerObjects.Count == 0)
        {
            Debug.LogError("Es gibt keine Gegner oder Spieler für den Kampf!");
            return;
        }

        // Gegner greift den ersten Spieler an
        GameObject enemy = enemyObjects[0];
        GameObject player = playerObjects[0]; // Nimm das tatsächlich instanzierte Objekt!

        ExecuteAttack(enemy, player); // Gegner greift den Spieler an

        // Der Zug ist vorbei, der Spieler ist wieder dran
        isPlayerTurn = true;
        EndTurn();
    }
    private bool AreAllEnemiesDefeated()
    {
        return enemyObjects.Count == 0;
    }

    // Wird aufgerufen, wenn ein Gegner besiegt wurde
    public void OnEnemyDefeated(GameObject defeatedEnemy)
    {
        enemyObjects.Remove(defeatedEnemy); // Entferne den besiegten Gegner aus der Liste
        //battleUIManager.UpdateEnemyUI(); // Aktualisiere die UI, um den entfernten Gegner widerzuspiegeln

        if (AreAllEnemiesDefeated())
        {
            StartCoroutine(EndBattleSequence()); // Starte die Endbattle-Sequenz, wenn alle Gegner besiegt sind
        }
    }

    // Coroutine für die Endbattle-Sequenz
    private IEnumerator EndBattleSequence()
    {
        Debug.Log("Alle Gegner besiegt! Kampf endet...");
        turnIndicatorText.text = "Alle Gegner besiegt!"; // Aktualisiere den Text, um das Kampfende anzuzeigen

        // Speichere die aktualisierten Charakterstatistiken im GameManager
        SaveCharacterStats(); // Stelle sicher, dass hier die Speichermethode aufgerufen wird!

        List<string> enemiesToDestroy = new List<string>();
        foreach (var enemy in enemyObjects)
        {
            enemiesToDestroy.Add(enemy.name);
        }
        GameManager.instance.enemiesToDestroy = enemiesToDestroy;
        SaveManager.Instance.SaveCharacterStats();
        yield return new WaitForSeconds(0.5f); // Warte 0.5 Sekunden, bevor der nächste Schritt ausgeführt wird

        ReturnToPreviousScene(); // Rufe die Methode auf, um zur vorherigen Szene zurückzukehren
    }


    private void SaveCharacterStats()
    {
        // Wenn die Charakterdaten gespeichert werden sollen, rufe die Methode auf, die sie speichert
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterStats characterStats = player.GetComponent<CharacterStats>();
            if (characterStats != null)
            {
                characterStats.GetSaveData();  // Speichert die Daten
                Debug.Log("Charakterdaten nach dem Kampf gespeichert.");
            }
            else
            {
                Debug.LogError("CharacterStats-Komponente nicht gefunden!");
            }
        }
        else
        {
            Debug.LogError("Spieler mit dem Tag 'Player' nicht gefunden!");
        }
    }


    // Methode, um zur vorherigen Szene zurückzukehren
    private void ReturnToPreviousScene()
    {
        Debug.Log("Rückkehr zur vorherigen Szene...");

        // Stelle sicher, dass GameManager existiert
        if (GameManager.instance != null)
        {
            // Lade die vorherige Szene
            SceneManager.LoadScene(GameManager.instance.previousSceneName);

            // Starte eine Coroutine, um die Spielerposition nach dem Laden der Szene zu setzen
            StartCoroutine(RepositionPlayerAfterSceneLoad());

            // Zerstöre das BattleManager-Objekt, um Duplikate zu vermeiden
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("GameManager-Instanz nicht gefunden!");
        }
    }

    private System.Collections.IEnumerator RepositionPlayerAfterSceneLoad()
    {
        // Warte, bis das Ende des Frames erreicht ist.  Das gibt der Szene Zeit zum Laden.
        yield return new WaitForEndOfFrame();

        // Finde das Spielerobjekt (wieder) in der neuen Szene
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Setze die Position des Spielers
            player.transform.position = GameManager.instance.lastPlayerPosition;
            Debug.Log("Spieler repositioniert zu: " + GameManager.instance.lastPlayerPosition);
        }
        else
        {
            Debug.LogError("Spieler-Objekt mit Tag 'Player' nicht gefunden!");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager1 : MonoBehaviour
{
    public static BattleManager1 instance;
    public BattleUIManager battleUIManager;

    private int currentTurn = 0; // Gibt an, wessen Zug es gerade ist (0 = Spieler, 1 = Gegner)
    private bool isPlayerTurn = true; // Gibt an, ob der Spieler am Zug ist

    private bool isBossFight;

    public List<GameObject> enemyPrefabs; // Liste von Gegner-Prefabs
    public List<Transform> enemySpawnPositions; // Liste von Positionen, an denen die Gegner spawnen sollen.
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public List<GameObject> selectedPlayers = new List<GameObject>(); // Die 3 aktiven K�mpfer
    private List<GameObject> playerObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();

    private string currentArea; // Variable, die das aktuelle Gebiet speichert

    void Start()
    {
        StartBattle();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void StartBattle()
    {
        Debug.Log("StartBattle wird aufgerufen."); // Debug-Log
        currentArea = GameManager.instance.GetCurrentArea();  // Hole das aktuelle Gebiet vom GameManager
        Debug.Log("Aktuelles Gebiet: " + currentArea);

        selectedPlayers = new List<GameObject>(PartyManager.instance.battleParty);
        GameManager.instance.SetEnemySpawnPositions(enemySpawnPositions);

        LoadEnemiesForBattle();

        // Pr�fe, ob Spawn-Positionen korrekt zugewiesen wurden
        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen f�r Gegner zugewiesen!");
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
            Debug.LogError("Keine Gegner f�r das Gebiet " + currentArea + " gefunden!");
        }
    }


    void SpawnCharacters()
    {
        Debug.Log("SpawnCharacters wird aufgerufen."); // Debug-Log

        Debug.Log("Anzahl der ausgew�hlten Spieler: " + GameManager.instance.selectedPlayerPrefabs.Count);

        // Spieler instanziieren
        for (int i = 0; i < GameManager.instance.selectedPlayerPrefabs.Count; i++)
        {
            GameObject playerPrefab = GameManager.instance.selectedPlayerPrefabs[i];

            // Prefab direkt instanziieren
            Vector3 spawnPos = playerSpawnPoint.position + new Vector3(i * 2.0f, 0, 0);
            GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            if (newPlayer == null)
            {
                Debug.LogError("Spieler-Instanziierung fehlgeschlagen f�r: " + playerPrefab.name);
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
                // F�ge Spieler zu einer Liste hinzu
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

        // Initialisiere die UI nach der Verz�gerung
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
                playerTeam.Add(stats); // F�ge den Charakter zum Spielerteam hinzu
            }
        }

        List<CharacterStats> enemyTeam = new List<CharacterStats>();
        foreach (var enemy in enemyObjects)
        {
            CharacterStats stats = enemy.GetComponent<CharacterStats>();
            if (stats != null)
            {
                enemyTeam.Add(stats); // F�ge den Gegner zum Gegnerteam hinzu
            }
        }

        // Rufe SetupUI mit den richtigen Teams auf
        battleUIManager.SetupUI(playerTeam, enemyTeam);
    }


    void SpawnRandomEnemies()
    {

        Debug.LogError("Spawnenemies wird aufgerufen");

        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen zugewiesen!");
            return;
        }

        // Hole die normalen Gegner aus dem EnemyManager basierend auf dem Gebiet
        List<GameObject> wildEnemiesForArea = EnemyManager.instance.GetEnemiesForBattle(GameManager.BattleType.WildBattle, currentArea);

        // Debugging: Pr�fen, ob Gegner gefunden wurden
        if (wildEnemiesForArea.Count == 0)
        {
            Debug.LogError("Keine normalen Gegner im aktuellen Gebiet zugewiesen! Gebiet: " + currentArea);
            return; // Kein Spawn von Gegnern, wenn keine gefunden wurden
        }

        // Pr�fen, ob gen�gend Spawn-Positionen vorhanden sind
        if (enemySpawnPositions.Count == 0)
        {
            Debug.LogError("Keine Spawn-Positionen zugewiesen!");
            return; // Kein Spawn, wenn keine Positionen da sind
        }

        // Zuf�llige Anzahl an Gegnern (z.B. zwischen 1 und 3)
        int enemyCount = Random.Range(1, Mathf.Min(4, enemySpawnPositions.Count + 1));
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
                Debug.LogError("Fehler: Das Gegner-Prefab ist null! �berpr�fe die Gegnerliste.");
                continue; // Falls das Prefab null ist, �berspringen
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
            // Zeige dem Spieler die m�glichen Aktionen (z.B. Angriff, Verteidigung)
            battleUIManager.ShowAttackButton(true); // Zeige die Angriffs-Schaltfl�che an
        }
        else
        {
            // Der Gegner f�hrt eine Aktion aus
            ExecuteEnemyTurn();
        }
    }

    // Methode f�r den Spielerangriff
    public void PlayerAttack()
    {
        if (isPlayerTurn)
        {
            // Spieler greift an
            // W�hlen Sie hier den Angriff und die Schadensberechnung aus.
            ExecuteAttack(selectedPlayers[0], enemyObjects[0]);

            // Nach dem Angriff ist der Zug vorbei und der Gegner ist dran
            isPlayerTurn = false;
            EndTurn();
        }
    }

    // Methode zum Ausf�hren des Angriffs
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

        Debug.Log($"{attackerStats.characterName} hat {defenderStats.characterName} {damage} Schaden zugef�gt!");
    }

    // Beispiel f�r die Schadensberechnung
    int CalculateDamage(CharacterStats attacker, CharacterStats defender)
    {
        // Beispielhafte Berechnung des Schadens
        int damage = attacker.attack - defender.defense;
        return Mathf.Max(damage, 0); // Schaden kann nicht negativ sein
    }

    // Methode, die das Ende des Zuges behandelt
    void EndTurn()
    {
        // Zum n�chsten Zug wechseln
        currentTurn++;
        if (currentTurn % 2 == 0)
        {
            isPlayerTurn = true; // Spieler ist wieder dran
        }
        else
        {
            isPlayerTurn = false; // Gegner ist dran
        }

        // Starte den n�chsten Zug
        StartTurn();
    }

    // Methode, die den Gegnerzug ausf�hrt
    void ExecuteEnemyTurn()
    {
        // Logik f�r den Gegnerzug, z.B. Angriff auf den Spieler
        GameObject enemy = enemyObjects[0];
        GameObject player = selectedPlayers[0];

        ExecuteAttack(enemy, player); // Beispiel: Gegner greift den Spieler an

        // Der Zug ist vorbei, der Spieler ist wieder dran
        isPlayerTurn = true;
        EndTurn();
    }
}

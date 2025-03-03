using System.Collections.Generic;
using UnityEngine;

public class BattleManager1 : MonoBehaviour
{
    public static BattleManager1 instance;
    public BattleUI battleUI;

    private bool isBossFight;

    public List<GameObject> enemyPrefabs; // Liste von Gegner-Prefabs
    public List<Transform> enemySpawnPositions; // Liste von Positionen, an denen die Gegner spawnen sollen.
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public List<GameObject> selectedPlayers = new List<GameObject>(); // Die 3 aktiven Kämpfer
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
            }


            // Füge das hier ein, um zu prüfen, ob SpawnRandomEnemies ausgeführt wird
            Debug.Log("Führe SpawnRandomEnemies aus...");
            SpawnRandomEnemies(); // Gegner spawnen
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

            // Zufällige Anzahl an Gegnern (z.B. zwischen 1 und 3)
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
    }
}
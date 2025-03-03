using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 lastPlayerPosition;
    public string enemyToDestroy = "";
    public GameObject playerPrefab;

    public BattleType currentBattleType;
    public List<GameObject> allCharacterPrefabs = new List<GameObject>(); // Alle 6 Charaktere als Prefabs
    public List<GameObject> availableCharacterPrefabs = new List<GameObject>(); // Freigeschaltete Charakter-Prefabs
    public List<GameObject> selectedPlayerPrefabs = new List<GameObject>(); // Die 3 Kämpfer-Prefabs
    public List<GameObject> selectedEnemyPrefabs = new List<GameObject>(); // Die 3 Kämpfer-Prefabs
    public List<GameObject> currentEnemies = new List<GameObject>(); // Hier speichern wir die Gegner



    public List<Transform> enemySpawnPositions; // Liste von Positionen, an denen die Gegner spawnen sollen
    private Vector3 defaultStartPosition = new Vector3(0, 0, 0); // Standard-Startposition
    private string currentArea;

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


    void Start()
    {
        // Suche nach allen Spawn-Positionen in der Szene
        GameManager.instance.SetEnemySpawnPositions(new List<Transform>(FindObjectsOfType<Transform>()));
        Debug.Log("Spawn-Positionen in BattleScene gesetzt.");
      
    }

    public void SetCurrentArea(string areaName)
    {
        currentArea = areaName;  // Setzt das Gebiet im GameManager
        Debug.Log("Aktuelles Gebiet: " + currentArea);
    }

    public string GetCurrentArea()
    {
        return currentArea;
    }


    // Die Battle-Funktion anpassen, um den `EnemyManager` zu verwenden
    public void StartBattle(BattleType battleType, string area)
    {
        Debug.Log($"Starte Kampf in Gebiet {area}: {battleType}");

        currentBattleType = battleType;  // Speichere den aktuellen Kampf-Typ

        currentEnemies = EnemyManager.instance.GetEnemiesForBattle(battleType, area); // Speichern!

        SceneManager.LoadScene("BattleScene");

        StartCoroutine(SpawnEnemiesAfterSceneLoad()); // Spawne nach dem Szenenwechsel
    }


    // Gegner und Spieler für den Kampf setzen
    public void SetBattleParticipants(List<GameObject> selectedPlayerPrefabs, List<GameObject> enemyPrefabs, bool bossFight)
    {
        Debug.Log("SetBattleParticipants aufgerufen");
        this.selectedPlayerPrefabs = selectedPlayerPrefabs; // Setze die Spieler für den Kampf
        Debug.Log("Anzahl der ausgewählten Spieler: " + selectedPlayerPrefabs.Count);

        // Gehe sicher, dass die Gegner auch gesetzt wurden:
        this.selectedEnemyPrefabs = enemyPrefabs;
        Debug.Log("Anzahl der ausgewählten Gegner: " + selectedEnemyPrefabs.Count);

        // Es könnte sein, dass hier noch eine Prüfung auf `null` erfolgen sollte
        if (this.selectedPlayerPrefabs.Count == 0 || this.selectedEnemyPrefabs.Count == 0)
        {
            Debug.LogError("Es wurden keine Spieler oder Gegner gesetzt!");
            return;
        }
    }


    private IEnumerator LoadBattleSceneAndSpawnEnemies()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene");
        while (!asyncLoad.isDone)
        {
            yield return null; // Warte, bis die Szene geladen ist
        }

        yield return new WaitForSeconds(0.5f); // Sicherheitspause

        Debug.Log("Szene wurde geladen. Spawne Gegner...");
        SetBattleParticipants(selectedPlayerPrefabs, selectedEnemyPrefabs, currentBattleType == BattleType.BossBattle);
    }
    private IEnumerator SpawnEnemiesAfterSceneLoad()
    {
        // Warten, bis die Szene vollständig geladen ist
        yield return new WaitForSeconds(1f);

        // Weiter mit dem Spawn-Prozess
        Debug.Log("Beginne mit dem Spawn der Gegner...");
        Debug.Log($"Anzahl gespeicherter Gegner: {currentEnemies.Count}");

        int enemyCount = Mathf.Min(currentEnemies.Count, enemySpawnPositions.Count);

        for (int i = 0; i < enemyCount; i++)
        {
            if (currentEnemies[i] != null && enemySpawnPositions[i] != null)
            {
                GameObject spawnedEnemy = Instantiate(currentEnemies[i], enemySpawnPositions[i].position, Quaternion.identity);
                Debug.Log($"Gegner gespawnt: {currentEnemies[i].name} an Position {enemySpawnPositions[i].position}");
            }
            else
            {
                Debug.LogWarning("Fehler: Ein Gegner oder eine Spawn-Position ist null!");
            }
        }
    }


    // BattleType-Enum für den Kampf
    public enum BattleType { WildBattle, BossBattle }

    // Setzt die Positionen für das Spawn der Gegner
    public void SetEnemySpawnPositions(List<Transform> spawnPositions)
    {
        enemySpawnPositions = new List<Transform>(spawnPositions);
        Debug.Log("Enemy spawn positions übernommen.");
    }

    public void SavePlayerPosition(Vector3 position)
    {
        lastPlayerPosition = position;
        Debug.Log("Spielerposition gespeichert: " + lastPlayerPosition);
    }

    public void RemoveEnemy()
    {
        if (!string.IsNullOrEmpty(enemyToDestroy))
        {
            GameObject enemy = GameObject.Find(enemyToDestroy);
            if (enemy != null)
            {
                Destroy(enemy);
                Debug.Log("Gegner entfernt: " + enemyToDestroy);
            }
        }
    }

    public void UnlockCharacter(GameObject newCharacterPrefab)
    {
        if (!availableCharacterPrefabs.Contains(newCharacterPrefab))
        {
            availableCharacterPrefabs.Add(newCharacterPrefab);
            Debug.Log(newCharacterPrefab.name + " wurde freigeschaltet!");
        }
    }

    public void SelectBattleTeam(List<GameObject> chosenCharacters)
    {
        if (chosenCharacters.Count == 3)
        {
            selectedPlayerPrefabs = new List<GameObject>(chosenCharacters);
            Debug.Log("Neues Team gewählt!");
            foreach (var character in selectedPlayerPrefabs)
            {
                Debug.Log(character.name);
            }
        }
        else
        {
            Debug.LogError("Genau 3 Charaktere müssen ausgewählt werden!");
        }
    }

    // Startet ein neues Spiel
    public void StartNewGame()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("FEHLER: SaveManager.instance ist NULL!");
            return;
        }

        // Speicherstand löschen und Neustart
        SaveManager.instance.DeleteSave();
        PlayerPrefs.DeleteAll();
        lastPlayerPosition = Vector3.zero;
        enemyToDestroy = "";

        SceneManager.LoadScene("Overworld"); // Falls das Spiel IMMER im Hauptmenü beginnt
        Debug.Log("Neues Spiel gestartet! Alles zurückgesetzt.");

        StartCoroutine(SaveAfterSceneLoad());
    }

    // Speichern, wenn die Szene geladen ist
    private IEnumerator SaveAfterSceneLoad()
    {
        yield return new WaitForSeconds(1f); // Warte, bis die Szene sicher geladen ist
        SaveGame();
        Debug.Log("Neues Spiel automatisch gespeichert!");
    }

    // Speicherfunktion
    public void SaveGame()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("FEHLER: SaveManager.instance ist NULL!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }
        else
        {
            Debug.LogWarning("Spieler konnte nicht gefunden werden! Speichere letzte bekannte Position.");
        }

        CharacterStats characterStats = player.GetComponent<CharacterStats>();
        SaveData data = new SaveData();
        data.SetPlayerPosition(lastPlayerPosition);
        data.sceneName = SceneManager.GetActiveScene().name;

        data.enemyToDestroy = enemyToDestroy;

        if (characterStats != null)
        {
            data.characterName = characterStats.characterName;
            data.level = characterStats.level;
            data.maxHP = characterStats.maxHP;
            data.currentHP = characterStats.currentHP;
            data.attack = characterStats.attack;
            data.defense = characterStats.defense;
            data.element = characterStats.element;
            data.type = characterStats.type;
            data.xp = characterStats.xp;
            data.xpToNextLevel = characterStats.xpToNextLevel;
        }

        SaveManager.instance.SaveGame(data);
        Debug.Log("Spiel gespeichert! Position: " + lastPlayerPosition + ", Gegnerstatus: " + enemyToDestroy);
    }

    // Lade das Spiel
    public void LoadGame()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("FEHLER: SaveManager.instance ist NULL!");
            return;
        }

        SaveData data = SaveManager.instance.LoadGame();
        if (data != null)
        {
            lastPlayerPosition = data.GetPlayerPosition();
            enemyToDestroy = data.enemyToDestroy;
            Debug.Log("Spiel geladen! Position: " + lastPlayerPosition + ", Gegnerstatus: " + enemyToDestroy);

            StartCoroutine(LoadSceneAndPlacePlayer(data.sceneName, lastPlayerPosition));
        }
        else
        {
            Debug.LogWarning("Kein Speicherstand gefunden!");
        }
    }

    // Lade die Szene und setze den Spieler
    private IEnumerator LoadSceneAndPlacePlayer(string sceneName, Vector3 position)
    {
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position;
            Debug.Log("Spieler geladen und auf gespeicherte Position gesetzt: " + position);
        }
        else
        {
            Debug.LogError("Spieler konnte nicht gefunden oder erstellt werden!");
        }
    }
}

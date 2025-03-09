using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string previousSceneName; // Variable, um den Namen der vorherigen Szene zu speichern
    public Vector3 lastPlayerPosition;
    public List<string> enemiesToDestroy = new List<string>(); // Geändert in eine Liste
    public GameObject playerPrefab;
    public static CharacterStats savedCharacterStats;  // Definiere diese statisch, um sie über Szenen hinweg zugänglich zu machen
    private GameObject lastBattleTrigger; // Das letzte BattleTrigger-Objekt, in das der Spieler gelaufen ist
    private string lastBattleTriggerName;

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
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // **NEU:** Stelle sicher, dass das Event abgemeldet wird, wenn das Objekt zerstört wird
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // **NEU:** Diese Methode wird aufgerufen, wenn eine Szene geladen wurde
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Szene geladen: " + scene.name);

        if (scene.name == previousSceneName)  // Wenn die vorherige Szene geladen wird
        {
            Debug.Log("Overworld Szene geladen. Daten werden angewendet.");
            StartCoroutine(PlacePlayerAfterSceneLoad());

            // Zerstöre den gespeicherten BattleTrigger, wenn er existiert
            if (!string.IsNullOrEmpty(lastBattleTriggerName))
            {
                GameObject battleTrigger = GameObject.Find(lastBattleTriggerName);
                if (battleTrigger != null)
                {
                    Destroy(battleTrigger);
                    Debug.Log("BattleTrigger " + battleTrigger.name + " wurde zerstört.");
                }
                else
                {
                    Debug.Log("BattleTrigger mit dem Namen " + lastBattleTriggerName + " wurde nicht gefunden.");
                }
            }


            ApplyCharacterStats();
            RemoveEnemy();
        }
    }


    public void SetLastBattleTrigger(GameObject battleTrigger)
    {
        lastBattleTrigger = battleTrigger;
        lastBattleTriggerName = battleTrigger.name;  // Speichere den Namen des Triggers
        Debug.Log("lastBattleTrigger wurde gesetzt: " + battleTrigger.name);
    }

    private IEnumerator PlacePlayerAfterSceneLoad()
    {
        yield return null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = lastPlayerPosition;
            Debug.Log("Spielerposition gesetzt auf: " + lastPlayerPosition);
        }
        else
        {
            Debug.LogError("Spieler konnte nach dem Laden der Szene nicht gefunden werden!");
        }
    }

    void Start()
    {
        // Suche nach allen Spawn-Positionen in der Szene
        GameManager.instance.SetEnemySpawnPositions(new List<Transform>(FindObjectsOfType<Transform>()));
        Debug.Log("Spawn-Positionen in BattleScene gesetzt.");
      
    }


    // Im GameManager oder dem Skript, das den Kampf startet
    public void StartBattle()
    {
        Debug.Log("Startbattle wird ausgeführt !!!!!!!!!!!!!!!!!!!!!!!!!!");
        previousSceneName = SceneManager.GetActiveScene().name;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
            Debug.Log("Overworld Position gespeichert.");
            CharacterStats stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                SaveCharacterStats(stats); // Speicher die Charakterdaten
            }
        }
        else
        {
            Debug.LogWarning("Kein Spieler mit dem Tag 'Player' in der Szene gefunden. Die Position wird nicht gespeichert.");
        }

        // Hier sicherstellen, dass lastBattleTrigger gesetzt wird, bevor die Szene geladen wird
        GameObject battleTrigger = GameObject.Find("BattleTrigger"); // Zum Beispiel den Trigger hier suchen
        if (battleTrigger != null)
        {
            GameManager.instance.SetLastBattleTrigger(battleTrigger);
        }

        SceneManager.LoadScene("BattleScene");
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

    // BattleType-Enum für den Kampf
    public enum BattleType { WildBattle, BossBattle }

    // Setzt die Positionen für das Spawn der Gegner
    public void SetEnemySpawnPositions(List<Transform> spawnPositions)
    {
        enemySpawnPositions = new List<Transform>(spawnPositions);
        Debug.Log("Enemy spawn positions übernommen.");
    }


    public void RemoveEnemy()
    {
        if (enemiesToDestroy != null && enemiesToDestroy.Count > 0)
        {
            foreach (string enemyName in enemiesToDestroy)
            {
                GameObject enemy = GameObject.Find(enemyName);
                if (enemy != null)
                {
                    Destroy(enemy);
                    Debug.Log("Gegner entfernt: " + enemyName);
                }
                else
                {
                    Debug.LogWarning("Gegner zum Zerstören nicht gefunden: " + enemyName);
                }
            }
            enemiesToDestroy.Clear(); // Leere die Liste, um zu verhindern, dass die Zerstörung mehrmals ausgeführt wird
        }
        else
        {
            Debug.Log("Keine Gegner zum Zerstören gefunden.");
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
        PlayerPrefs.Save();
        lastPlayerPosition = Vector3.zero;
        // Setze die HP des Spielers auf den Maximalwert zurück
        foreach (var player in selectedPlayerPrefabs)
        {
            CharacterStats stats = player.GetComponent<CharacterStats>();
            stats.currentHP = stats.maxHP; // Setze die HP des Spielers zurück
        }

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

        data.enemiesToDestroy = enemiesToDestroy;

        if (characterStats != null)
        {
            // Speichere die Charakterdaten mit der SaveCharacterData Methode
            characterStats.SaveCharacterData();  // Hier wird deine Methode aufgerufen

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
        Debug.Log("Spiel gespeichert! Position: " + lastPlayerPosition + ", Gegnerstatus: " + enemiesToDestroy);
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
            enemiesToDestroy = data.enemiesToDestroy;
            Debug.Log("Spiel geladen! Position: " + lastPlayerPosition + ", Gegnerstatus: " + enemiesToDestroy);

            StartCoroutine(LoadSceneAndPlacePlayer(data.sceneName, lastPlayerPosition));
        }
        else
        {
            Debug.LogWarning("Kein Speicherstand gefunden!");
        }
    }
    public void SaveCharacterStatsBeforeSceneChange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterStats stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                savedCharacterStats = new CharacterStats
                {
                    characterName = stats.characterName,
                    currentHP = stats.currentHP,
                    xp = stats.xp,
                    level = stats.level,
                    maxHP = stats.maxHP,
                    attack = stats.attack,
                    defense = stats.defense,
                    element = stats.element,
                    type = stats.type,
                    xpToNextLevel = stats.xpToNextLevel
                };

                Debug.Log("Charakterstatistiken vor dem Szenenwechsel gespeichert.");
            }
            else
            {
                Debug.LogError("CharacterStats-Komponente nicht gefunden!");
            }
        }
        else
        {
            Debug.LogError("Spieler nicht gefunden!");
        }
    }

    public void SaveCharacterStats(CharacterStats stats)
    {
        if (stats != null)
        {
            savedCharacterStats = new CharacterStats
            {
                characterName = stats.characterName,
                currentHP = stats.currentHP,
                xp = stats.xp,
                level = stats.level,
                maxHP = stats.maxHP,
                attack = stats.attack,
                defense = stats.defense,
                element = stats.element,
                type = stats.type,
                xpToNextLevel = stats.xpToNextLevel
            };

            Debug.Log("Charakterstatistiken gespeichert.");
        }
        else
        {
            Debug.LogError("CharacterStats-Komponente nicht gefunden!");
        }
    }

    public void ApplyCharacterStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterStats stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.characterName = savedCharacterStats.characterName;
                stats.currentHP = savedCharacterStats.currentHP;
                stats.xp = savedCharacterStats.xp;
                stats.level = savedCharacterStats.level;
                stats.maxHP = savedCharacterStats.maxHP;
                stats.attack = savedCharacterStats.attack;
                stats.defense = savedCharacterStats.defense;
                stats.element = savedCharacterStats.element;
                stats.type = savedCharacterStats.type;
                stats.xpToNextLevel = savedCharacterStats.xpToNextLevel;

                Debug.Log("Charakterstatistiken angewendet.");
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




    // Lade die Szene und setze den Spieler
    // Lade die Szene und setze den Spieler
    // Lade die Szene und setze den Spieler
    // Lade die Szene und setze den Spieler
    private IEnumerator LoadSceneAndPlacePlayer(string sceneName, Vector3 position)
    {
        // Läd die Szene asynchron
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Warte bis die Szene vollständig geladen ist
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Spieler nach dem Szenenwechsel finden
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Spieler gefunden, setze seine Position
            player.transform.position = position;

            // Charakterstatistiken aus dem GameManager anwenden
            ApplyCharacterStats();
        }
        else
        {
            Debug.LogWarning("Spieler nicht gefunden! Instanziiere einen neuen Spieler.");
            player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.tag = "Player";

            // Statistiken auf den neuen Spieler anwenden
            ApplyCharacterStats();
        }

        // Gegner entfernen, falls nötig
        RemoveEnemy();
    }


}



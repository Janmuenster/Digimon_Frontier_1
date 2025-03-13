using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public string previousSceneName; // Variable, um den Namen der vorherigen Szene zu speichern
    public Vector3 lastPlayerPosition;
    public List<string> enemiesToDestroy = new List<string>(); // Geändert in eine Liste
    public GameObject playerPrefab;
    public static CharacterStats savedCharacterStats;  // Definiere diese statisch, um sie über Szenen hinweg zugänglich zu machen
    private GameObject lastBattleTrigger; // Das letzte BattleTrigger-Objekt, in das der Spieler gelaufen ist
    private string lastBattleTriggerName;
    [SerializeField] private CharacterData defaultCharacterData;


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
    public async Task StartNewGameAsync()
    {
        if (defaultCharacterData == null)
        {
            Debug.LogError("DefaultCharacterData ist nicht gesetzt!");
            return;
        }

        CharacterStats tempStats = new CharacterStats();
        tempStats.characterData = defaultCharacterData;
        tempStats.LoadDefaultCharacterData();

        var newSaveData = new SaveData
        {
            sceneName = "Overworld",
            playerPosition = new SaveData.SerializableVector3(Vector3.zero),
            characterProgress = new SaveData.CharacterProgress
            {
                characterName = tempStats.characterName,
                level = tempStats.level,
                currentHP = tempStats.currentHP,
                maxHP = tempStats.maxHP,
                attack = tempStats.attack,
                defense = tempStats.defense,
                speed = tempStats.speed,
                element = tempStats.element,
                characterType = tempStats.type,
                xp = 0,
                xpToNextLevel = 100,
                isDigitized = false
            },
            totalPlayTime = 0
        };

        await SaveManager.Instance.SaveGameAsync(newSaveData);
        await LoadGameAsync();
    }



    // Speicherfunktion
    public async Task SaveGameAsync()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        var characterStats = player.GetComponent<CharacterStats>();
        var saveData = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = new SaveData.SerializableVector3(player.transform.position),
            characterProgress = characterStats.GetSaveData(),
            totalPlayTime = Time.realtimeSinceStartup
        };

        await SaveManager.Instance.SaveGameAsync(saveData);

    }


    // Lade das Spiel
    public async Task LoadGameAsync()
    {
        var saveData = await SaveManager.Instance.LoadGameAsync();
        if (saveData == null) return;

        // AsyncOperation für asynchrones Laden der Szene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.sceneName);

        // Warte, bis die Szene vollständig geladen ist
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerPosition.ToVector3();
            var characterStats = player.GetComponent<CharacterStats>();
            characterStats.LoadFromSaveData(saveData.characterProgress);
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



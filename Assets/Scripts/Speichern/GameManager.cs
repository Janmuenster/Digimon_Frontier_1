using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 lastPlayerPosition;
    public string enemyToDestroy = "";
    public GameObject playerPrefab;

    public List<GameObject> allCharacterPrefabs = new List<GameObject>(); // Alle 6 Charaktere als Prefabs
    public List<GameObject> availableCharacterPrefabs = new List<GameObject>(); // Freigeschaltete Charakter-Prefabs
    public List<GameObject> selectedPlayerPrefabs = new List<GameObject>(); // Die 3 Kämpfer-Prefabs
    public List<GameObject> enemyPrefabs = new List<GameObject>(); // Gegner-Prefabs für den Kampf
    public bool isBossFight = false;

    public List<GameObject> bossEnemies = new List<GameObject>(); // Liste für Bossgegner
    public List<GameObject> randomEnemies = new List<GameObject>(); // Liste für normale Gegner

    private Vector3 defaultStartPosition = new Vector3(0, 0, 0); // Standard-Startposition
   
    void Start()
{
    Debug.Log("GameManager aktiv. Spieler-Team:");
    foreach (var player in selectedPlayerPrefabs)
    {
        Debug.Log(player.name);
    }
}
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialisiert.");
        }
        else
        {
            Debug.LogWarning("Doppelter GameManager gefunden, zerstöre diesen!");
            Destroy(gameObject);
        }
    }
    public enum BattleType { WildBattle, BossBattle }

    public void StartBattle(BattleType battleType, List<GameObject> enemies)
    {
        isBossFight = (battleType == BattleType.BossBattle);
        Debug.Log($"Starte Kampf: {battleType}");

        // Übergib die Liste der Gegner basierend auf dem Typ des Kampfes
        if (isBossFight)
        {
            enemyPrefabs = new List<GameObject>(bossEnemies); // Boss-Gegner
        }
        else
        {
            enemyPrefabs = new List<GameObject>(randomEnemies); // Normale Gegner
        }

        SceneManager.LoadScene("BattleScene"); // Lade die Kampf-Szene
    }

    // Gegner und Spieler für den Kampf setzen
    public void SetBattleParticipants(List<GameObject> selectedPlayerPrefabs, List<GameObject> enemyPrefabs, bool bossFight)
    {
        Debug.Log("SetBattleParticipants aufgerufen");
        this.selectedPlayerPrefabs = selectedPlayerPrefabs; // Setze die Spieler für den Kampf
        isBossFight = bossFight; // Setze, ob es ein Bosskampf ist

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("Keine Gegner im SetBattleParticipants-Call!");
            return;
        }

        // Ausgabe der Namen der Gegner, die übergeben wurden
        foreach (var enemy in enemyPrefabs)
        {
            Debug.Log("Gegner, der übergeben wurde: " + enemy.name);
        }

        // Spawne die Gegner in der Battle-Szene, wenn diese geladen wurde
        StartCoroutine(SpawnEnemiesAfterSceneLoad(enemyPrefabs));
    }
    private IEnumerator SpawnEnemiesAfterSceneLoad(List<GameObject> enemyPrefabs)
    {
        // Warte, bis die Szene vollständig geladen ist
        yield return new WaitForSeconds(1f); // Warte etwas länger, um sicherzustellen, dass die Szene wirklich vollständig geladen ist

        Debug.Log("Beginne mit dem Spawn der Gegner...");

        // Spawne die Gegner an zufälligen Positionen (oder an festen Positionen je nach Bedarf)
        foreach (var enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab != null)
            {
                // Zufällige Position (oder eine andere Logik nach deinen Wünschen)
                Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0);
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Debug-Ausgabe
                Debug.Log($"Gegner gespawnt: {enemyPrefab.name} an Position {spawnPosition}");
            }
            else
            {
                Debug.LogWarning("Fehler: Ein Gegner-Prefab war null und konnte nicht gespawnt werden!");
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


    public void StartNewGame()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("FEHLER: SaveManager.instance ist NULL!");
            return;
        }

        // **1. Speicherstand löschen**
        SaveManager.instance.DeleteSave();

        PlayerPrefs.DeleteAll();

        // **2. Spielerposition & Fortschritt zurücksetzen**
        lastPlayerPosition = Vector3.zero;
        enemyToDestroy = ""; // Falls du Gegner speicherst

        // **3. Neustart mit kompletter Spiel-Reset**
        SceneManager.LoadScene("Overworld"); // Falls das Spiel IMMER im Hauptmenü beginnt
        Debug.Log("Neues Spiel gestartet! Alles zurückgesetzt.");

        // **4. Automatisch speichern, sobald das neue Spiel gestartet wurde**
        StartCoroutine(SaveAfterSceneLoad());
    }

    // Gegner + Spieler für den Kampf setzen
    public void SetBattleParticipants(List<GameObject> enemies, List<GameObject> enemyPrefabs, List<GameObject> selectedEnemyPrefabs, bool bossFight)
    {
        selectedPlayerPrefabs = new List<GameObject>(PartyManager.instance.battleParty);
        this.enemyPrefabs = new List<GameObject>(enemyPrefabs);
        isBossFight = bossFight;
    }


    // Warte kurz, bis die Szene geladen ist, dann speichere
    private IEnumerator SaveAfterSceneLoad()
    {
        yield return new WaitForSeconds(1f); // Warte 1 Sekunde, damit die Szene sicher geladen ist
        SaveGame(); // Speichert das neue Spiel
        Debug.Log("Neues Spiel automatisch gespeichert!");
    }

    public void SaveGame()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("FEHLER: SaveManager.instance ist NULL!");
            return;
        }

        // **Aktuelle Spielerposition speichern**
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }
        else
        {
            Debug.LogWarning("Spieler konnte nicht gefunden werden! Speichere letzte bekannte Position.");
        }

        // Hole die CharacterStats-Komponente
        CharacterStats characterStats = player.GetComponent<CharacterStats>();

        SaveData data = new SaveData();
        data.SetPlayerPosition(lastPlayerPosition);
        data.sceneName = SceneManager.GetActiveScene().name;

        // **Jetzt wird der Gegnerstatus NUR gespeichert, wenn gespeichert wird**
        data.enemyToDestroy = enemyToDestroy;

        // Speichern der Charakterdaten
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
            enemyToDestroy = data.enemyToDestroy; // **Nur setzen, wenn geladen wird!**
            Debug.Log("Spiel geladen! Position: " + lastPlayerPosition + ", Gegnerstatus: " + enemyToDestroy);

            // Szene laden & Spieler setzen
            StartCoroutine(LoadSceneAndPlacePlayer(data.sceneName, lastPlayerPosition));

            // Setze die Charakterdaten
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterStats characterStats = player.GetComponent<CharacterStats>();
                if (characterStats != null)
                {
                    characterStats.characterName = data.characterName;
                    characterStats.level = data.level;
                    characterStats.maxHP = data.maxHP;
                    characterStats.currentHP = data.currentHP;
                    characterStats.attack = data.attack;
                    characterStats.defense = data.defense;
                    characterStats.element = data.element;
                    characterStats.type = data.type;
                    characterStats.xp = data.xp;
                    characterStats.xpToNextLevel = data.xpToNextLevel;

                    // Setze den Digitation-Status zurück (immer auf "nicht digitiert" setzen)
                    characterStats.ToggleDigitation(false); // Rückkehr zur Basisform
                }
            }
        }
        else
        {
            Debug.LogWarning("Kein Speicherstand gefunden!");
        }
    }


    private IEnumerator LoadSceneAndPlacePlayer(string sceneName, Vector3 position)
    {
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f); // Warte kurz, bis Szene geladen ist

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position;
            Debug.Log("Spieler geladen und auf gespeicherte Position gesetzt: " + position);
        }
        else
        {
            Debug.LogError("Spieler konnte nicht gefunden werden!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Overworld")
        {
            Debug.Log("Overworld geladen, versuche Spieler zu platzieren...");

            // Suche den Spieler
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning("Kein Spieler gefunden, erstelle neuen Spieler!");

                if (playerPrefab == null)
                {
                    Debug.LogError("FEHLER: `playerPrefab` ist NICHT im Inspector gesetzt!");
                    return;
                }

                // Falls kein Spieler existiert, erstelle einen an der letzten bekannten Position oder Standardposition
                Vector3 spawnPosition = lastPlayerPosition != Vector3.zero ? lastPlayerPosition : defaultStartPosition;
                player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("Neuer Spieler erstellt an Position: " + spawnPosition);
            }
            else
            {
                // Falls Spieler existiert, setze ihn direkt auf die gespeicherte Position
                player.transform.position = lastPlayerPosition;
                Debug.Log("Spieler direkt an Position gesetzt: " + lastPlayerPosition);
            }

            // Stelle sicher, dass der Spieler aktiv ist
            player.SetActive(true);
            Debug.Log("Spieler aktiviert.");

            // Entferne den Gegner aus der Overworld
            RemoveEnemy();
        }
    }


    private IEnumerator RestorePlayerAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Minimal warten, falls nötig

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Kein Spieler gefunden, erstelle neuen Spieler!");

            if (playerPrefab == null)
            {
                Debug.LogError("FEHLER: `playerPrefab` ist NICHT im Inspector gesetzt!");
                yield break;
            }

            // Erstelle den Player an der gespeicherten Position oder Standardposition
            Vector3 spawnPosition = lastPlayerPosition != Vector3.zero ? lastPlayerPosition : defaultStartPosition;
            player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Neuer Spieler erstellt an Position: " + spawnPosition);
        }

        // Stelle sicher, dass der Spieler die richtige Position hat, BEVOR er aktiviert wird
        if (player != null)
        {
            player.transform.position = lastPlayerPosition;
            Debug.Log("Spieler direkt an Position gesetzt: " + lastPlayerPosition);

            player.SetActive(true);
            Debug.Log("Spieler aktiviert.");
        }
        else
        {
            Debug.LogError("Spieler konnte nicht gefunden oder erstellt werden!");
        }
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
}

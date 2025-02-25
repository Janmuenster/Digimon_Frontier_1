using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 lastPlayerPosition;
    public string enemyToDestroy = "";
    public GameObject playerPrefab;

    private Vector3 defaultStartPosition = new Vector3(0, 0, 0); // Standard-Startposition
   

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lastPlayerPosition = player.transform.position;
                Debug.Log("Startposition gespeichert: " + lastPlayerPosition);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Falls Speicherstand existiert -> Frage anzeigen
        if (SaveManager.instance.SaveExists() && SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log("Ein Speicherstand existiert bereits!");
            // Hier kannst du ein UI-Popup anzeigen lassen.
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

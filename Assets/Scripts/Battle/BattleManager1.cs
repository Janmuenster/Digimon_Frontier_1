using System.Collections.Generic;
using UnityEngine;

public class BattleManager1 : MonoBehaviour
{
    public static BattleManager1 instance;
    public BattleUI battleUI;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public List<GameObject> selectedPlayers = new List<GameObject>(); // Die 3 aktiven K�mpfer
    public List<GameObject> bossEnemies; // Liste f�r Bossgegner
    public List<GameObject> randomEnemies; // Liste f�r normale Gegner

    private List<GameObject> playerObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();


    void Start()
    {
        StartBattle();
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void StartBattle()
    {
        Debug.Log("StartBattle aufgerufen");

        selectedPlayers = new List<GameObject>(PartyManager.instance.battleParty);

        // Pr�fen, ob enemyPrefabs bef�llt ist
        if (GameManager.instance.enemyPrefabs == null || GameManager.instance.enemyPrefabs.Count == 0)
        {
            Debug.LogError("enemyPrefabs ist leer oder null!");
        }
        else
        {
            Debug.Log("enemyPrefabs enth�lt " + GameManager.instance.enemyPrefabs.Count + " Gegner.");
        }

        // Weitere Logik...
        SpawnCharacters();
    }





    void SpawnCharacters()
    {
        Debug.Log("Anzahl der ausgew�hlten Spieler: " + GameManager.instance.selectedPlayerPrefabs.Count);

        //  Spieler instanziieren
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

            // Charakterdaten setzen (holt CharacterStats von Character-Objekt)
            CharacterStats stats = newPlayer.GetComponent<CharacterStats>();
            if (stats == null)
            {
                Debug.LogError("CharacterStats fehlt bei: " + newPlayer.name);
            }
            else
            {
                Debug.Log("CharacterStats gefunden bei: " + newPlayer.name);
                Character character = stats.character; // Wir gehen davon aus, dass 'CharacterStats' das 'Character'-Objekt hat.
                newPlayer.GetComponent<CharacterDisplay>().SetCharacter(character);
            }
        }


        // Gegner instanziieren
        if (GameManager.instance.isBossFight)
        {
            // Bosskampf - Zuf�lligen Bossgegner aus der Liste ausw�hlen
            SpawnRandomBossEnemy();
        }
        else
        {
            // Normaler Kampf - Zuf�llige Gegner aus der Liste ausw�hlen
            SpawnRandomEnemies();
        }
    }

    void SpawnRandomBossEnemy()
    {
        if (bossEnemies.Count > 0)
        {
            // Zuf�llig einen Bossgegner aus der Liste ausw�hlen
            GameObject bossPrefab = bossEnemies[Random.Range(0, bossEnemies.Count)];
            Vector3 spawnPos = enemySpawnPoint.position;
            GameObject bossEnemy = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Bossgegner gespawnt: " + bossEnemy.name);

            // Hier kannst du auch CharacterStats setzen, wenn du sie hast
            CharacterStats stats = bossEnemy.GetComponent<CharacterStats>();
            if (stats != null)
            {
                Character character = stats.character;
                bossEnemy.GetComponent<CharacterDisplay>().SetCharacter(character);
            }
            else
            {
                Debug.LogError("CharacterStats f�r Bossgegner fehlen!");
            }

            enemyObjects.Add(bossEnemy);
        }
        else
        {
            Debug.LogError("Keine Bossgegner im Inspector zugewiesen!");
        }
    }

    void SpawnRandomEnemies()
    {
        if (randomEnemies.Count > 0)
        {
            // Anzahl der Gegner, die gespawnt werden sollen (z.B. 3 Gegner)
            int enemyCount = 3; // Diese Zahl kannst du nach Belieben �ndern
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject enemyPrefab = randomEnemies[Random.Range(0, randomEnemies.Count)];
                Vector3 spawnPos = enemySpawnPoint.position + new Vector3(i * 2.0f, 0, 0);
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                // Charakterdaten setzen (holt CharacterStats von Character-Objekt)
                CharacterStats stats = newEnemy.GetComponent<CharacterStats>();
                if (stats != null)
                {
                    Character character = stats.character;
                    newEnemy.GetComponent<CharacterDisplay>().SetCharacter(character);
                }
                else
                {
                    Debug.LogError("CharacterStats-Komponente fehlt bei: " + enemyPrefab.name);
                }

                enemyObjects.Add(newEnemy);
            }
        }
        else
        {
            Debug.LogError("Keine normalen Gegner im Inspector zugewiesen!");
        }
    }


GameObject GetCharacterPrefab(string characterName, List<GameObject> prefabList)
    {
        foreach (GameObject prefab in prefabList)
        {
            if (prefab.name == characterName) //  Findet das richtige Prefab per Name
            {
                return prefab;
            }
        }
        return prefabList[0]; // Falls nichts gefunden, Standard-Prefab nehmen
    }
    public void SwapCharacter(int battleIndex, GameObject newCharacter)
    {
        if (!PartyManager.instance.battleParty.Contains(newCharacter))
        {
            Debug.LogWarning("Dieser Charakter ist nicht in der Party!");
            return;
        }

        selectedPlayers[battleIndex] = newCharacter;
        UpdateBattleUI(); // UI aktualisieren
    }

    private void UpdateBattleUI()
    {
        Debug.Log("UI aktualisieren: K�mpfende Charaktere ge�ndert.");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField]

    public string areaName; // Gebiet wählen
    public EnemyManager enemyManager;

    void Start()
    {
        // Hole den EnemyManager zur Laufzeit, wenn es nötig ist
        enemyManager = EnemyManager.instance;

        // Optional: Sicherheitscheck, wenn keine Instanz gefunden wird
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager instance nicht gefunden! Stelle sicher, dass der EnemyManager im Spiel existiert.");
        }
    }

    IEnumerator LoadBattleScene(List<GameObject> enemies)
    {
        yield return SceneManager.LoadSceneAsync("BattleScene"); // Warten, bis die Szene geladen ist
        yield return new WaitForSeconds(0.5f); // Zusätzliche Sicherheitspause

        SpawnEnemies(enemies); // Gegner spawnen
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debugging: Name des Gebiets ausgeben
            Debug.Log("Betritt Gebiet: " + areaName);

            GameManager.instance.SetCurrentArea(areaName);

            // Hier rufen wir die Gegner für das Gebiet ab
            List<GameObject> enemies = enemyManager.GetEnemiesForBattle(GameManager.instance.currentBattleType, areaName);

            if (enemies.Count == 0)
            {
                Debug.LogError("Keine Gegner für dieses Gebiet gefunden!");
                return; // Keine Szene laden, wenn keine Gegner existieren
            }

            StartCoroutine(LoadBattleScene(enemies));
        }
    }

    private IEnumerator SpawnEnemiesWithDelay(List<GameObject> enemies)
    {
        // Warte eine kurze Zeit, um sicherzustellen, dass spawnPositions gesetzt wurden
        yield return new WaitForSeconds(0.5f);

        // Jetzt kannst du die Gegner spawnnen, sicher dass enemySpawnPositions korrekt gesetzt sind
        SpawnEnemies(enemies);
    }

    void SpawnEnemies(List<GameObject> enemies)
    {
        // Zugriff auf enemySpawnPositions über GameManager
        List<Transform> enemySpawnPositions = GameManager.instance.enemySpawnPositions;

        foreach (var enemy in enemies)
        {
            if (enemy != null && enemySpawnPositions.Count > 0)
            {
                // Füge hier eine Position aus der enemySpawnPositions-Liste hinzu
                Transform spawnPosition = enemySpawnPositions[Random.Range(0, enemySpawnPositions.Count)];
                GameObject spawnedEnemy = Instantiate(enemy, spawnPosition.position, Quaternion.identity);
                Debug.Log($"Gegner {enemy.name} wurde gespawnt an {spawnPosition.position}");
            }
            else
            {
                Debug.LogWarning("Keine Spawn-Positionen oder Gegner vorhanden!");
            }
        }
    }
}

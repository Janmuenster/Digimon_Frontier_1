using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    // Die verschiedenen Gegnerlisten für verschiedene Gebiete
    public List<GameObject> wildEnemiesForest; // Wild-Gegner
    public List<GameObject> wildEnemiesCave; // Wild-Gegner
    public List<GameObject> bossEnemies; // Boss-Gegner

    // Gebietsspezifische Gegner, z.B. für "Forest" oder "Cave"
    public Dictionary<string, List<GameObject>> areaEnemies = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Beispiel-Daten für verschiedene Gebiete
        areaEnemies.Add("Forest", wildEnemiesForest); // Beispielgebiet: Forest
        areaEnemies.Add("Cave", wildEnemiesCave); // Beispielgebiet: Cave
        areaEnemies.Add("Boss1", bossEnemies); // Beispielgebiet: Cave

    }

    // Diese Funktion gibt die Gegner zurück, je nachdem, ob es sich um einen Wildkampf oder einen Bosskampf handelt
    public List<GameObject> GetEnemiesForBattle(GameManager.BattleType battleType, string area)
    {
        List<GameObject> enemies = new List<GameObject>();

        Debug.Log("Übergebener Areaname: " + area); // Log für den Areanamen, der übergeben wird

        if (battleType == GameManager.BattleType.WildBattle)
        {
            // Wildkampf: Zufällige Gegner aus dem Bereich auswählen
            if (areaEnemies.ContainsKey(area))
            {
                enemies = GetRandomEnemies(areaEnemies[area]);
                Debug.Log($"Gegner für WildBattle im Gebiet {area} gefunden: {enemies.Count} Gegner.");
            }
            else
            {
                Debug.LogError($"Kein Gebiet mit dem Namen {area} in der Dictionary-Liste gefunden.");
            }
        }
        else if (battleType == GameManager.BattleType.BossBattle)
        {
            // Bosskampf: Einen Boss aus der Boss-Liste auswählen
            if (bossEnemies.Count > 0)
            {
                enemies.Add(bossEnemies[Random.Range(0, bossEnemies.Count)]);
            }
        }

        if (enemies.Count == 0)
        {
            Debug.LogWarning($"Keine Gegner für {area} gefunden.");
        }

        return enemies;
    }



    // Hilfsmethode, um zufällige Gegner aus einer Liste zu wählen
    public List<GameObject> GetRandomEnemies(List<GameObject> availableEnemies)
    {
        int enemyCount = Random.Range(1, 4); // Wähle 1 bis 3 Gegner aus
        List<GameObject> selectedEnemies = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
            selectedEnemies.Add(enemy);
        }

        return selectedEnemies;
    }
   
}

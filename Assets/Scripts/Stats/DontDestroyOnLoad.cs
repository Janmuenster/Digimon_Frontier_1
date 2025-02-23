using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }
    private UnitStats playerStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerStats(UnitStats stats)
    {
        playerStats = new UnitStats();
        playerStats.CopyStatsFrom(stats);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene") // Ersetzen Sie "BattleScene" mit dem tatsächlichen Namen Ihrer Battle-Szene
        {
            ApplyPlayerStatsToBattlePrefab();
        }
    }

    private void ApplyPlayerStatsToBattlePrefab()
    {
        GameObject battlePlayer = GameObject.FindGameObjectWithTag("Takuya");
        if (battlePlayer != null)
        {
            UnitStats battlePlayerStats = battlePlayer.GetComponent<UnitStats>();
            if (battlePlayerStats != null && playerStats != null)
            {
                battlePlayerStats.CopyStatsFrom(playerStats);
            }
            else
            {
            }
        }
        else
        {
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

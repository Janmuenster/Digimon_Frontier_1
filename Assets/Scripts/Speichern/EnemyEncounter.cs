using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyEncounter : MonoBehaviour
{
    public string battleSceneName = "Battles 1";
    public PlayerStats playerStats;
    public Transform playerTransform;
    public saveManager saveManager;

    public string spriteID;

    public static string SpriteToDestroyID = null;
    public static bool BattleWon = false;

    void Start()
    {
        if (string.IsNullOrEmpty(spriteID))
        {
            spriteID = System.Guid.NewGuid().ToString();
            Debug.Log("Generated new Sprite ID: " + spriteID);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Takuya"))
        {
            if (playerStats == null)
            {
                Debug.LogError("Kein PlayerStats-Skript zugewiesen!");
                return;
            }

            Debug.Log("Speichere Position vor Kampf: " + playerTransform.position);
            playerStats.SavePosition(playerTransform.position, SceneManager.GetActiveScene().name);
            saveManager.SavePlayer(playerStats);

            SceneManager.LoadScene(battleSceneName);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded aufgerufen! Szene: " + scene.name + ", SpriteToDestroyID: " + SpriteToDestroyID + ", spriteID: " + spriteID + ", BattleWon: " + BattleWon);
        if (scene.name != battleSceneName && SpriteToDestroyID == spriteID && BattleWon)
        {
            Destroy(gameObject);
            SpriteToDestroyID = null;
            BattleWon = false;
        }
    }
}

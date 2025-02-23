using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveLoadManager : MonoBehaviour
{
    public PlayerStats playerStats; // Ziehe das PlayerStats-Skript hier im Inspector rein
    public static SaveLoadManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Behält das Objekt beim Szenenwechsel
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        saveManager.SavePlayer(playerStats);
        Debug.Log("Spiel gespeichert!");
    }

    public void LoadGame()
    {
        PlayerData data = saveManager.LoadPlayer();
        if (data != null)
        {
            Debug.Log(" Geladene Position nach Kampf: " + data.positionX + ", " + data.positionY);
            Debug.Log(" Geladene Szene nach Kampf: " + data.overworldSceneName);

            playerStats.level = data.level;
            playerStats.currentHealth = data.currentHealth;
            playerStats.attack = data.attack;
            playerStats.baseDefense = data.baseDefense;
            playerStats.experience = data.experience;

            SceneManager.LoadScene(data.overworldSceneName);
            StartCoroutine(DelayedPositionLoad(data.positionX, data.positionY));
        }
        else
        {
            Debug.LogWarning("Kein Speicherstand gefunden!");
        }
    }


    private IEnumerator DelayedPositionLoad(float x, float y)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        yield return null; // Einen Frame warten

        playerStats.transform.position = new Vector2(x, y);
        Debug.Log("Spielerposition nach Szenenwechsel gesetzt: " + x + ", " + y);
    }

    public void StartNewGame()
    {
        // Setze Spielerstats auf Standardwerte
        playerStats.level = 1;
        playerStats.currentHealth = 100;
        playerStats.attack = 10;
        playerStats.baseDefense = 5;
        playerStats.experience = 0;

        // Lade die Startszene
        SceneManager.LoadScene("level"); // Ersetze mit dem Namen deiner Startszene
    }
}

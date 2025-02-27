using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTrigger : MonoBehaviour
{
    public bool isBossFight; // Ist es ein Bosskampf?
    private List<GameObject> bossPrefabs;
    private List<GameObject> wildEnemyPrefabs;

    // Diese Listen sind nicht mehr nötig, da die Gegner aus dem GameManager geladen werden
    // public List<GameObject> enemyCharacters;  // Liste der Gegner
    // public List<GameObject> bossPrefabs;  // Boss-Enemy-Prefabs
    // public List<GameObject> wildEnemyPrefabs; // Wilde Gegner-Prefabs

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("BattleTrigger betreten. Starte den Kampf...");

            // Wähle die Gegner basierend auf dem Kampf-Typ
            List<GameObject> selectedEnemyPrefabs = isBossFight ? bossPrefabs : wildEnemyPrefabs;

            // Übergebe die Gegner an den GameManager
           
            GameManager.instance.SetBattleParticipants(GameManager.instance.selectedPlayerPrefabs, selectedEnemyPrefabs, isBossFight);

            // Lade die Battle-Szene
            SceneManager.LoadScene("BattleScene");
        }
    }
}

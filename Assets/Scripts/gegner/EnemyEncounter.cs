using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyEncounter : MonoBehaviour
{
    public string battleSceneName = "BattleScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Speichere die Position, bevor der Kampf beginnt
           // GameManager.instance.SavePlayerPosition(other.transform.position);

            // Deaktiviere den Spieler in der Overworld
            other.gameObject.SetActive(false);

            // Setze Gegner-ID für spätere Zerstörung
            //GameManager.instance.enemyToDestroy = gameObject.name;

            // Wechsel in die Kampfszene
            SceneManager.LoadScene("BattleScene");
        }
    }

}

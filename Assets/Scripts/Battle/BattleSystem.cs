using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleSystem : MonoBehaviour
{
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private GameObject playerUnit;
    private GameObject enemyUnit;

    void Start()
    {
        // Spieler und Gegner an fixen Positionen in der Battle-Szene instanziieren
        playerUnit = Instantiate(playerPrefab, playerBattleStation.position, Quaternion.identity);
        enemyUnit = Instantiate(enemyPrefab, enemyBattleStation.position, Quaternion.identity);
    }

    public void PlayerAttack()
    {
        Debug.Log("Spieler greift an!");

        // Schaden zufügen (Beispiel)
        Destroy(enemyUnit, 1.5f); // Gegner besiegt

        // Nach der Attacke zurück zur Overworld
        StartCoroutine(EndBattle());
    }

    private IEnumerator EndBattle()
    {
        yield return new WaitForSeconds(2);

        // Szene wechseln
        SceneManager.LoadScene("Overworld");
    }
}

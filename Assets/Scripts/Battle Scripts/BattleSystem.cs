using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum ElementType { Fire, Water, Wind, Earth, Light, Darkness, Ice, Electricity, Plant, Metal, Free }
public enum DigimonType { Virus, Vaccine, Data, Free }

public class BattleSystem : MonoBehaviour
{
    public BattleHUD playerHUD;
    public BattleHUDEnemy EnemyHUD;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject newPlayerPrefab; // Für die Digitation

    public string animationTriggerName = "TransformTrigger";
    public float animationDuration = 2f;

    // Keine feste Szene mehr, wir laden die gespeicherte Szene!
    public string gameOverSceneName = "GameOver"; // Name der Game Over Szene

    public Transform PlayerBattleStation;
    public Transform EnemyBattleStation;

    public PlayerStats playerStats;  // Geändert zu PlayerStats
    public saveManager saveManager;
    EnemyStats enemyStats;    // Geändert zu EnemyStats

    private Animator playerAnimator;

    public TextMeshProUGUI dialogueText;

    public BattleHUDEnemy enemyHUD;

    public BattleState state;

    public float attackMoveSpeed = 5f;
    public float attackDuration = 1f;
    public string attackAnimationTrigger = "Attack";
    public string idleAnimationState = "Idle";

    private Vector3 desiredPlayerPosition;
    private bool needToSetPlayerPosition = false;
    private bool hasDigitized = false;

    public string spriteToDestroyID;
    public bool battleWon;
    private Vector3 playerStartPosition;
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerStartPosition = playerStats.transform.position;
        // Spieler instanziieren
        GameObject playerGo = Instantiate(playerPrefab, PlayerBattleStation.position, PlayerBattleStation.rotation);
        playerGo.transform.SetParent(PlayerBattleStation);
        playerStats = playerGo.GetComponent<PlayerStats>(); // Geändert zu PlayerStats
        if (playerStats == null)
        {
            Debug.LogError("Kein PlayerStats-Skript am Player-Prefab gefunden!");
            yield break;
        }
        Debug.Log($"Player instantiated. Name: {playerStats.unitName}, Tag: {playerGo.tag}");
        playerAnimator = playerGo.GetComponent<Animator>();

        desiredPlayerPosition = PlayerBattleStation.position;
        needToSetPlayerPosition = true;

        // Gegner instanziieren
        GameObject enemyGo = Instantiate(enemyPrefab, EnemyBattleStation);
        enemyStats = enemyGo.GetComponent<EnemyStats>(); // Geändert zu EnemyStats

        if (enemyStats == null)
        {
            Debug.LogError("Kein EnemyStats-Skript am Enemy-Prefab gefunden!");
            
                Destroy(enemyGo); // Stelle sicher, dass der ungültige Gegner zerstört wird
                yield break;
            }

            dialogueText.text = "A wild " + enemyStats.unitName + " attacks";

            playerHUD.SetHUD(playerStats);
            Debug.Log($"SetHUD called for player. Name: {playerStats.unitName}");
            EnemyHUD.SetHUD(enemyStats);

            yield return new WaitForSeconds(2f);

            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    

    private void LateUpdate()
    {
        if (needToSetPlayerPosition && playerStats != null)
        {
            playerStats.transform.position = desiredPlayerPosition;
            needToSetPlayerPosition = false;
        }
    }

    IEnumerator PlayerAttack()
    {
        GameObject player = playerStats.gameObject; // Hol dir das GameObject vom PlayerStats-Skript
        Vector3 originalPosition = player.transform.position;
        Vector3 targetPosition = enemyStats.transform.position;

        // Bewegung zum Gegner
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            player.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * attackMoveSpeed;
            yield return null;
        }

        // Angriff Animation abspielen
        playerAnimator.SetTrigger(attackAnimationTrigger);

        yield return new WaitForSeconds(attackDuration);

        // Schaden berechnen und anwenden
        float damageMultiplier = CalculateDamageMultiplier(playerStats.elementType, enemyStats.elementType, playerStats.digimonType, enemyStats.digimonType);
        int damage = Mathf.RoundToInt(playerStats.attack * damageMultiplier);

        enemyStats.TakeDamage(damage); // Verwende die TakeDamage-Methode
        EnemyHUD.SetHP(enemyStats.currentHealth);
        dialogueText.text = "The Attack hit";

        // Zurück zur Ausgangsposition bewegen
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            player.transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * attackMoveSpeed;
            yield return null;
        }
        playerAnimator.Play(idleAnimationState);
        yield return new WaitForSeconds(1f);

        // Überprüfen, ob der Kampf beendet ist
        if (enemyStats.currentHealth <= 0)
        {
            Destroy(enemyStats.gameObject);
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerKickAttack()
    {
        GameObject player = playerStats.gameObject; // Hol dir das GameObject vom PlayerStats-Skript
        Vector3 originalPosition = player.transform.position;
        Vector3 targetPosition = enemyStats.transform.position - (enemyStats.transform.position - player.transform.position).normalized * 0.5f; // Näher am Gegner für einen Kick

        // Bewegung zum Gegner
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            player.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * attackMoveSpeed * 1.2f; // Etwas schneller für einen dynamischen Kick
            yield return null;
        }

        // Kick Animation abspielen
        playerAnimator.SetTrigger("KickTrigger"); // Angenommen, Sie haben einen "KickTrigger" in Ihrem Animator

        yield return new WaitForSeconds(0.5f); // Kürzere Dauer für einen schnellen Kick

        // Schaden berechnen und anwenden (vielleicht etwas mehr Schaden für einen Kick)
        float damageMultiplier = CalculateDamageMultiplier(playerStats.elementType, enemyStats.elementType, playerStats.digimonType, enemyStats.digimonType);
        int damage = Mathf.RoundToInt(playerStats.attack * damageMultiplier);

        enemyStats.TakeDamage(damage); // Verwende die TakeDamage-Methode
        EnemyHUD.SetHP(enemyStats.currentHealth);
        dialogueText.text = "The Kick landed!";

        // Zurück zur Ausgangsposition bewegen
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            player.transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * attackMoveSpeed * 1.5f; // Schneller zurück nach dem Kick
            yield return null;
        }
        // Idle Animation abspielen
        playerAnimator.Play(idleAnimationState);

        yield return new WaitForSeconds(1f);

        // Überprüfen, ob der Kampf beendet ist
        if (enemyStats.currentHealth <= 0)
        {
            Destroy(enemyStats.gameObject);
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyStats.unitName + " is thinking...";
        yield return new WaitForSeconds(1f);

        IAction chosenAction = enemyStats.ChooseAction();
        if (chosenAction != null)
        {
            dialogueText.text = $"{enemyStats.unitName} uses {chosenAction.Name}!";
            yield return new WaitForSeconds(1f);

            chosenAction.Execute(enemyStats, playerStats);

            // Aktualisiere die HUDs
            playerHUD.SetHP(playerStats.currentHealth);
            EnemyHUD.SetHP(enemyStats.currentHealth);

            yield return new WaitForSeconds(1f);
        }
        else
        {
            dialogueText.text = enemyStats.unitName + " does nothing!";
            yield return new WaitForSeconds(1f);
        }

        if (playerStats.currentHealth <= 0)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator PlayerHSpiritDigitation()
    {
        if (hasDigitized)
        {
            Debug.Log("Digitation wurde bereits in diesem Kampf ausgeführt.");
            yield break;
        }

        playerAnimator.SetTrigger(animationTriggerName);

        yield return new WaitForSeconds(animationDuration);

        GameObject currentPlayerObject = playerStats.gameObject; // Hol dir das GameObject vom PlayerStats-Skript

        GameObject newPlayer = Instantiate(newPlayerPrefab, currentPlayerObject.transform.position, currentPlayerObject.transform.rotation);

        // Verstärke die Stats des neuen Spielers basierend auf dem Charakter
        PlayerStats newPlayerStats = newPlayer.GetComponent<PlayerStats>();
        if (newPlayerStats != null)
        {
            float statMultiplier = GetStatMultiplierForCharacter(playerStats.unitName);
            newPlayerStats.maxHealth = Mathf.RoundToInt(newPlayerStats.maxHealth * statMultiplier);
            newPlayerStats.currentHealth = newPlayerStats.maxHealth;
            newPlayerStats.attack = Mathf.RoundToInt(newPlayerStats.attack * statMultiplier);
            newPlayerStats.baseDefense = Mathf.RoundToInt(newPlayerStats.baseDefense * statMultiplier);

            if (playerStats.unitName == "Takuya")
            {
                newPlayerStats.elementType = ElementType.Fire;
                newPlayerStats.digimonType = DigimonType.Vaccine;
            }
        }

        Destroy(currentPlayerObject);

        UpdatePlayerReferences(newPlayer);
        playerHUD.SetHUD(playerStats);

        hasDigitized = true;

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private float GetStatMultiplierForCharacter(string characterName)
    {
        switch (characterName)
        {
            case "Takuya":
                return 2.0f;
            case "Zoe":
                return 1.5f;
            default:
                return 1.0f; // Standardmultiplikator für unbekannte Charaktere
        }
    }

    void UpdatePlayerReferences(GameObject newPlayer)
    {
        playerStats = newPlayer.GetComponent<PlayerStats>(); // Geändert zu PlayerStats
        playerAnimator = newPlayer.GetComponent<Animator>();
        playerHUD.SetHUD(playerStats);
    }

    void EndBattle()
    {
        // Speichern der Spielerdaten
        if (state == BattleState.WON)
        {
            int expGained = CalculateExperienceGained(enemyStats);
            playerStats.GainExperience(expGained);
            playerHUD.SetEXP(playerStats.experience);
            dialogueText.text += $"\nYou gained {expGained} EXP!";

            if (playerStats.experience >= playerStats.experienceToNextLevel)
            {
                dialogueText.text += $"\n{playerStats.unitName} leveled up to Level {playerStats.level + 1}!";
            }
        }

        saveManager.SavePlayer(playerStats);
        OnBattleOver();
    }

    public void OnBattleOver()
    {
        // XP verteilen
        if (state == BattleState.WON)
        {
            int expGained = CalculateExperienceGained(enemyStats);
            playerStats.GainExperience(expGained);
            playerHUD.SetEXP(playerStats.experience);
            dialogueText.text += $"\nYou gained {expGained} EXP!";

            if (playerStats.experience >= playerStats.experienceToNextLevel)
            {
                dialogueText.text += $"\n{playerStats.unitName} leveled up to Level {playerStats.level + 1}!";
            }
        }

        // Spielerdaten speichern (inkl. Position VOR dem Kampf!)
        saveManager.SavePlayer(playerStats);

        // Warte kurz, bevor Szene wechselt (damit der Spieler das XP-Update sieht)
        StartCoroutine(WaitAndLoadOverworld());
    }

    IEnumerator WaitAndLoadOverworld()
    {
        yield return new WaitForSeconds(2f); // Lässt Zeit für XP-Text
        PlayerData data = saveManager.LoadPlayer();
        if (data != null)
        {
            SceneManager.LoadScene(data.overworldSceneName);
            StartCoroutine(LoadPlayerPositionAfterBattle(data));
        }
        else
        {
            Debug.LogError("Keine gespeicherten Daten gefunden!");
        }
    }

    //  Spieler wieder an seine gespeicherte Position setzen
    IEnumerator LoadPlayerPositionAfterBattle(PlayerData data)
    {
        yield return new WaitForEndOfFrame(); // Warte auf Szenenwechsel

        GameObject player = GameObject.FindGameObjectWithTag("Takuya");
        if (player != null)
        {
            player.transform.position = new Vector3(data.positionX, data.positionY);
        }
        else
        {
            Debug.LogError("Spieler konnte nicht gefunden werden!");
        }
    }


    IEnumerator LoadPlayerPositionAfterBattle()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        yield return null; // Warte einen Frame

        GameObject player = GameObject.FindGameObjectWithTag("Takuya");
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                Debug.Log(" Spieler nach Kampf gefunden!");
                player.transform.position = new Vector3(playerStats.positionX, playerStats.positionY, 0);
            }
            else
            {
                Debug.LogError(" PlayerStats nicht gefunden!");
            }
        }
        else
        {
            Debug.LogError(" Spieler konnte nicht gefunden werden!");
        }
    }
    private int CalculateExperienceGained(EnemyStats enemyStats) // Geändert zu EnemyStats
    {
        int baseXP = enemyStats.level * 10;
        int bonusXP = 0;

        if (enemyStats.gameObject.CompareTag("Cerberusmon"))
        {
            bonusXP += 90; // Zusätzliche 60 XP für Cerberusmon
        }

        return baseXP + bonusXP;
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHSpiritDigitationButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHSpiritDigitation());
    }

    public void OnKickButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerKickAttack());
    }

    private float CalculateDamageMultiplier(ElementType attackerElement, ElementType defenderElement, DigimonType attackerType, DigimonType defenderType)
    {
        float typeMultiplier = 1f;
        float elementMultiplier = 1f;

        // Digimon Type multipliers
        switch (attackerType)
        {
            case DigimonType.Data:
                if (defenderType == DigimonType.Vaccine)
                    typeMultiplier = 1.5f;
                else if (defenderType == DigimonType.Virus)
                    typeMultiplier = 0.5f;
                break;
            case DigimonType.Virus:
                if (defenderType == DigimonType.Data)
                    typeMultiplier = 1.5f;
                else if (defenderType == DigimonType.Vaccine)
                    typeMultiplier = 0.5f;
                break;
            case DigimonType.Vaccine:
                if (defenderType == DigimonType.Virus)
                    typeMultiplier = 1.5f;
                else if (defenderType == DigimonType.Data)
                    typeMultiplier = 0.5f;
                break;
        }

        // Element multipliers
        switch (attackerElement)
        {
            case ElementType.Fire:
                if (defenderElement == ElementType.Plant)
                    elementMultiplier = 2f;
                else if (defenderElement == ElementType.Water)
                    elementMultiplier = 0.5f;
                break;
            case ElementType.Water:
                if (defenderElement == ElementType.Fire)
                    elementMultiplier = 2f;
                else if (defenderElement == ElementType.Electricity)
                    elementMultiplier = 0.5f;
                break;
                // Weitere Element-Multiplikatoren hier
        }

        return typeMultiplier * elementMultiplier;
    }
}

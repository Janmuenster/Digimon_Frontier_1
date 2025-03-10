using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterStats : MonoBehaviour
{


    public Character character;

    [Header("Datenquelle")]
    public CharacterData characterData; // Ziehe hier das ScriptableObject rein!

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite digivolvedSprite;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    [Header("Stats")]
    public string characterName;
    public int level;
    public int maxHP;
    public int currentHP;
    public int attack;
    public int speed;
    public int defense;
    public int attackPower;
    public string element;
    public string type;
    public int xp = 0;
    public int xpToNextLevel = 100;


    [Header("Digitation")]
    public int currentDigivolutionIndex = 0; // Index der aktuellen Digitation
    public bool isDigitized = false;
    private int baseHP;
    private int baseAttack;
    private int baseDefense;
    private int baseSpeed;
    private string baseElement;
    private string baseType;

    public List<Attack> attacks = new List<Attack>();
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer nicht gefunden!");
        }
        animator = GetComponent<Animator>();
        attacks.Add(new Attack("Schlag", 1, ElementType.Free, AttackType.Normal));
        attacks.Add(new Attack("Feuerball", 2, ElementType.Fire, AttackType.Special));

        // Stelle sicher, dass die gespeicherten Daten zuerst überprüft werden
        if (PlayerPrefs.HasKey("CurrentHP_" + characterName))
        {
            // Wenn gespeicherte Daten existieren, lade sie
            LoadCharacterData();
        }
        else
        {
            // Wenn keine gespeicherten Daten existieren, lade die Standardwerte
            LoadDefaultCharacterData();
        }
    }

    void LoadDefaultCharacterData()
    {
        if (characterData != null)
        {
            characterName = characterData.characterName;
            level = characterData.startLevel;
            maxHP = characterData.startMaxHP;
            currentHP = maxHP;
            attack = characterData.startAttack;
            speed = characterData.startSpeed;
            defense = characterData.startDefense;
            element = characterData.element;
            type = characterData.type;

            Debug.Log("Standardwerte geladen für " + characterName);
        }
        else
        {
            Debug.LogError("Keine CharacterData gesetzt!");
        }
    }
    // Speichern der Charakterdaten nach dem Kampf oder beim Verlassen der Szene
    public void SaveCharacterData()
    {
        PlayerPrefs.SetString("CharacterName_" + characterName, characterName);
        PlayerPrefs.SetInt("Level_" + characterName, level);
        PlayerPrefs.SetInt("MaxHP_" + characterName, maxHP);
        PlayerPrefs.SetInt("CurrentHP_" + characterName, currentHP);
        PlayerPrefs.SetInt("Attack_" + characterName, attack);
        PlayerPrefs.SetInt("Speed_" + characterName, speed);
        PlayerPrefs.SetInt("Defense_" + characterName, defense);
        PlayerPrefs.SetString("Element_" + characterName, element);
        PlayerPrefs.SetString("Type_" + characterName, type);
        PlayerPrefs.SetInt("XP_" + characterName, xp);
        PlayerPrefs.SetInt("XPToNextLevel_" + characterName, xpToNextLevel);

        PlayerPrefs.Save();
        Debug.Log("Charakterdaten für " + characterName + " gespeichert!");
    }


    // Laden der Charakterdaten, falls sie vorhanden sind (bevor Standardwerte geladen werden)
    private void LoadCharacterData()
    {
        characterName = PlayerPrefs.GetString("CharacterName_" + characterName, characterName);
        level = PlayerPrefs.GetInt("Level_" + characterName, level);
        maxHP = PlayerPrefs.GetInt("MaxHP_" + characterName, maxHP);
        currentHP = PlayerPrefs.GetInt("CurrentHP_" + characterName, currentHP);
        attack = PlayerPrefs.GetInt("Attack_" + characterName, attack);
        speed = PlayerPrefs.GetInt("Speed_" + characterName, speed);
        defense = PlayerPrefs.GetInt("Defense_" + characterName, defense);
        element = PlayerPrefs.GetString("Element_" + characterName, element);
        type = PlayerPrefs.GetString("Type_" + characterName, type);
        xp = PlayerPrefs.GetInt("XP_" + characterName, xp);
        xpToNextLevel = PlayerPrefs.GetInt("XPToNextLevel_" + characterName, xpToNextLevel);

        Debug.Log("Daten aus PlayerPrefs geladen für: " + characterName);
    }


    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log(characterName + " hat " + damage + " Schaden erlitten. Aktuelle Gesundheit: " + currentHP);

        SaveCharacterData(); // Speichern nach erlittenem Schaden

        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log(characterName + " ist gestorben!");

        // Rufe die OnEnemyDefeated-Methode im BattleManager auf, wenn es ein Gegner ist
        if (gameObject.CompareTag("Enemy")) // Stelle sicher, dass deine Gegner den Tag "Enemy" haben
        {
            BattleManager1.instance.OnEnemyDefeated(gameObject); // Gib das Gameobject des Gegners weiter
        }
        else if (gameObject.CompareTag("Player"))
        {
            // Hier kannst du Logik für den Tod des Spielers hinzufügen, falls erforderlich
            Debug.Log("Spieler ist gestorben!");
            // Beispiel: EndBattleSequence starten, wenn alle Spieler tot sind
            // BattleManager1.instance.CheckIfAllPlayersAreDefeated();
        }

        // Zerstöre das Gameobject des Charakters
        Destroy(gameObject);
    }
    public void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
        PlayerPrefs.Save();
        Debug.Log("Spielerposition gespeichert: " + transform.position);
    }
    void LoadPlayerPosition()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            transform.position = new Vector3(x, y, z);
            Debug.Log("Spielerposition geladen: " + transform.position);
        }
    }

}


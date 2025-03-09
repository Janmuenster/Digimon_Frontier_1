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
        PlayerPrefs.SetString("CharacterName", characterName);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("MaxHP", maxHP);
        PlayerPrefs.SetInt("CurrentHP", currentHP);
        PlayerPrefs.SetInt("Attack", attack);
        PlayerPrefs.SetInt("Speed", speed);
        PlayerPrefs.SetInt("Defense", defense);
        PlayerPrefs.SetString("Element", element);
        PlayerPrefs.SetString("Type", type);

        PlayerPrefs.Save(); // Speichern der Daten dauerhaft
        Debug.Log("Charakterdaten gespeichert!");
    }

    // Laden der Charakterdaten, falls sie vorhanden sind (bevor Standardwerte geladen werden)
   private void LoadCharacterData()
{
    // Hier lädst du die gespeicherten Charakterdaten
    characterName = PlayerPrefs.GetString("CharacterName_" + characterName);
    currentHP = PlayerPrefs.GetInt("CurrentHP_" + characterName);
    maxHP = PlayerPrefs.GetInt("MaxHP_" + characterName);
    level = PlayerPrefs.GetInt("Level_" + characterName);
    xp = PlayerPrefs.GetInt("XP_" + characterName);
    attack = PlayerPrefs.GetInt("Attack_" + characterName);
    defense = PlayerPrefs.GetInt("Defense_" + characterName);
    // Füge alle anderen gespeicherten Werte hinzu...
    
    Debug.Log("Daten aus PlayerPrefs geladen: " + characterName);
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


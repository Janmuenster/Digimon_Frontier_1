using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Datenquelle")]
    public CharacterData characterData; // Ziehe hier das ScriptableObject rein!

    [Header("Stats")]
    public string characterName;
    public int level;
    public int maxHP;
    public int currentHP;
    public int attack;
    public int defense;
    public string element;
    public string type;
    public int xp = 0;
    public int xpToNextLevel = 100;

    [Header("Digitation")]
    public bool isDigitized = false;
    private int baseHP;
    private int baseAttack;
    private int baseDefense;
    private string baseElement;
    private string baseType;

    public List<Attack> attacks = new List<Attack>();
    void Start()
    {
        attacks.Add(new Attack("Schlag", 10, ElementType.Free, AttackType.Normal));
        attacks.Add(new Attack("Feuerball", 20, ElementType.Fire, AttackType.Special));
    

        if (characterData != null)
        {
            // Lade die Startwerte aus dem `CharacterData` Objekt
            characterName = characterData.characterName;
            level = characterData.startLevel;
            maxHP = characterData.startMaxHP;
            attack = characterData.startAttack;
            defense = characterData.startDefense;
            element = characterData.element;
            type = characterData.type;
        }
        else
        {
            Debug.LogError("Keine CharacterData für " + gameObject.name + " gesetzt!");
        }

        // Basis-Stats für Digitation speichern
        baseHP = maxHP;
        baseAttack = attack;
        baseDefense = defense;
        baseElement = element;
        baseType = type;

        currentHP = maxHP;
    }

    public void ToggleDigitation(bool digitize)
    {
        isDigitized = digitize;

        if (digitize)
        {
            maxHP = baseHP * 2;
            attack = baseAttack * 2;
            defense = baseDefense * 2;
            element = characterData.digiElement;  // Element wechselt zur Digiform
            type = characterData.digiType;
            currentHP = maxHP; // Setze das Leben auf das neue Maximum!
        }
        else
        {
            maxHP = baseHP;
            attack = baseAttack;
            defense = baseDefense;
            element = baseElement;  // Zurück zum normalen Element
            type = baseType;
            currentHP = Mathf.Min(currentHP, maxHP); // Falls mehr HP als das Maximum nach Digitation
        }

        Debug.Log(characterName + " hat " + (digitize ? "digitiert!" : "zurückdigiitiert!"));
    }
    public void GainXP(int amount)
    {
        xp += amount;
        Debug.Log(characterName + " hat " + amount + " XP erhalten! (Total: " + xp + ")");

        while (xp >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        xp -= xpToNextLevel;
        level++;
        xpToNextLevel += 50; // Erhöhe die benötigte XP für das nächste Level

        // Erhöhe die Stats basierend auf einem Wachstumsschema
        maxHP += Mathf.RoundToInt(baseHP * 0.2f);
        attack += Mathf.RoundToInt(baseAttack * 0.15f);
        defense += Mathf.RoundToInt(baseDefense * 0.1f);

        Debug.Log(characterName + " ist auf Level " + level + " aufgestiegen!");
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log(characterName + " wurde besiegt!");
        BattleManager.instance.playerTeam.Remove(this); // Entferne den Charakter aus dem Kampf
    }
}

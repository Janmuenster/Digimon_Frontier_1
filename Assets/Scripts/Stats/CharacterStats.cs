using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError(" SpriteRenderer nicht gefunden!");
        }
        animator = GetComponent<Animator>();
        attacks.Add(new Attack("Schlag", 1, ElementType.Free, AttackType.Normal));
        attacks.Add(new Attack("Feuerball", 2, ElementType.Fire, AttackType.Special));
    

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
            StartCoroutine(DigivolveSequence(digitize));
            
       }

    IEnumerator DigivolveSequence(bool digitize)
    {
        isDigitized = digitize;

        // 1️⃣ Animation starten
        animator.SetTrigger("Digivolve");

        // Warte, bis die Animation vorbei ist
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 2️⃣ Animator deaktivieren, damit er nicht das Sprite überschreibt
        animator.enabled = false;

        // 3️⃣ Flash-Effekt hinzufügen
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;

        // 4️⃣ Sprite wechseln
        spriteRenderer.sprite = digitize ? digivolvedSprite : normalSprite;

        Debug.Log("Digitation abgeschlossen: " + spriteRenderer.sprite.name);
    
        // 4️⃣ Stats anpassen
        if (digitize)
        {
            maxHP *= 2;
            attack *= 2;
            defense *= 2;
            currentHP = maxHP;
        }
        else
        {
            maxHP /= 2;
            attack /= 2;
            defense /= 2;
            currentHP = Mathf.Min(currentHP, maxHP);
        }

        Debug.Log(characterName + " hat digitiert!");
    }

   

    //  Diese Funktion wird von der Animation aufgerufen!
    public void OnDigivolveAnimationEnd()
    {
        Debug.Log("Animation Event wurde ausgelöst! ");
        spriteRenderer.sprite = isDigitized ? digivolvedSprite : normalSprite;

        Debug.Log("Neues Sprite: " + spriteRenderer.sprite.name); // Prüfen, ob das Sprite wirklich geändert wurde
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

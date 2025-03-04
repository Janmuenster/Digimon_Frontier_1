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
            speed = characterData.startSpeed;
            defense = characterData.startDefense;
            element = characterData.element;
            type = characterData.type;
            Debug.Log("Charakterdaten geladen: " + characterName + ", Level: " + level);  // Debugging-Zeile

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
        baseSpeed = speed;
        baseType = type;



        if (PlayerPrefs.HasKey("CurrentHP_" + characterName))
        {
            currentHP = PlayerPrefs.GetInt("CurrentHP_" + characterName);
            Debug.Log("HP für " + characterName + " aus Speicher geladen: " + currentHP);


        }
        else
        {
            currentHP = maxHP; // Falls keine gespeicherten Werte existieren, setze es auf maxHP
            Debug.Log("Kein gespeicherter Wert gefunden. Setze " + characterName + " HP auf: " + maxHP);

        }
        // Stelle sicher, dass der Charakter nicht als besiegt gilt
        if (currentHP <= 0)
        {
            currentHP = 1; // Um sicherzustellen, dass der Charakter mit minimaler HP ins Spiel startet
        }
        Debug.Log("Geladene HP: " + currentHP + "/" + maxHP + " für " + characterName);

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

        // 4️⃣ Stats und Name anpassen
        if (digitize)
        {
            // Ändere die Digitation-Attribute, basierend auf dem aktuellen Digivolutionsindex
            if (currentDigivolutionIndex < characterData.digivolutions.Length)
            {
                Digivolution digivolution = characterData.digivolutions[currentDigivolutionIndex];
                characterName = digivolution.digiName; // Name der Digitation setzen
                element = digivolution.digiElement;   // Element setzen
                type = digivolution.digiType;         // Typ setzen

                // Berechnung der neuen Werte basierend auf den aktuellen Werten, nicht einfach verdoppeln
                float hpMultiplier = 2f;
                maxHP = Mathf.RoundToInt(maxHP * hpMultiplier);
                currentHP = Mathf.RoundToInt(currentHP * hpMultiplier);
                attack = Mathf.RoundToInt(attack * 2f);
                defense = Mathf.RoundToInt(defense * 2f);
                speed = Mathf.RoundToInt(speed * 2f);



                currentDigivolutionIndex++; // Zum nächsten Digivolutionslevel wechseln
            }
        }
        else
        {
            // Zurück zur Basis-Digitation
            characterName = characterData.characterName;
            element = characterData.element;
            type = characterData.type;

            // Berechne die Rückkehr-Stats basierend auf den aktuellen HP und den Standardwerten
            float hpMultiplier = 0.5f;
            maxHP = Mathf.RoundToInt(maxHP * hpMultiplier);
            currentHP = Mathf.RoundToInt(currentHP * hpMultiplier);
            attack = Mathf.RoundToInt(attack * 0.5f);
            defense = Mathf.RoundToInt(defense * 0.5f);
            speed = Mathf.RoundToInt(speed * 0.5f);

            // Setze die aktuellen HP auf den minimalen Wert zwischen den aktuellen HP und den maximalen
            currentHP = Mathf.Min(currentHP, maxHP);

            currentDigivolutionIndex = 0; // Zurücksetzen des Digivolutionsindex
        }

        Debug.Log(characterName + " hat digitiert!");

        // UI nach Digitation updaten
        //BattleManager1.instance.UpdateUIAfterDigivolution();
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
        speed += Mathf.RoundToInt(baseSpeed * 0.1f);

        Debug.Log(characterName + " ist auf Level " + level + " aufgestiegen!");
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        Debug.Log(characterName + " wurde besiegt!");
    }
}
    


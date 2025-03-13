using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CharacterStats : MonoBehaviour
{
    public Character character;

    [Header("Datenquelle")]
    public CharacterData characterData;

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
    public int currentDigivolutionIndex = 0;
    public bool isDigitized = false;
    private int baseHP;
    private int baseAttack;
    private int baseDefense;
    private int baseSpeed;
    private string baseElement;
    private string baseType;

    public List<Attack> attacks = new List<Attack>();

    private async void Start()
    {
        InitializeComponents();
        InitializeAttacks();
        await LoadCharacterData();
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            Debug.LogError($"SpriteRenderer nicht gefunden auf {gameObject.name}!");
        if (animator == null)
            Debug.LogError($"Animator nicht gefunden auf {gameObject.name}!");
    }

    private void InitializeAttacks()
    {
        attacks.Add(new Attack("Schlag", 1, ElementType.Free, AttackType.Normal));
        attacks.Add(new Attack("Feuerball", 2, ElementType.Fire, AttackType.Special));
    }

    private async Task LoadCharacterData()
    {
        if (CompareTag("Player"))
        {
            SaveData saveData = await SaveManager.Instance.LoadGameAsync();
            if (saveData != null && saveData.characterProgresses != null && saveData.characterProgresses.Count > 0)
            {
                // Finde den passenden CharacterProgress für diesen Charakter
                var characterProgress = saveData.characterProgresses.Find(cp => cp.characterName == this.characterName);
                if (characterProgress != null)
                {
                    LoadFromSaveData(characterProgress);
                    Debug.Log($"Charakterdaten für {characterName} geladen.");
                }
                else
                {
                    LoadDefaultCharacterData();
                }
            }
            else
            {
                LoadDefaultCharacterData();
            }
        }
        else
        {
            LoadDefaultCharacterData();
        }
    }


    public void LoadDefaultCharacterData()
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

            Debug.Log($"Standardwerte geladen für {characterName}");
        }
        else
        {
            Debug.LogError("Keine CharacterData gesetzt!");
        }
    }

    public SaveData.CharacterProgress GetSaveData()
    {
        return new SaveData.CharacterProgress
        {
            characterName = this.characterName,
            level = this.level,
            currentHP = this.currentHP,
            maxHP = this.maxHP,
            attack = this.attack,
            defense = this.defense,
            element = this.element,
            characterType = this.type,
            xp = this.xp,
            xpToNextLevel = this.xpToNextLevel,
            isDigitized = this.isDigitized
        };
    }

    public void LoadFromSaveData(SaveData.CharacterProgress data)
    {
        characterName = data.characterName;
        level = data.level;
        currentHP = data.currentHP;
        maxHP = data.maxHP;
        attack = data.attack;
        defense = data.defense;
        element = data.element;
        type = data.characterType;
        xp = data.xp;
        xpToNextLevel = data.xpToNextLevel;
        isDigitized = data.isDigitized;
    }

    public async void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"{characterName} hat {damage} Schaden erlitten. Aktuelle Gesundheit: {currentHP}");

        if (CompareTag("Player"))
        {
            SaveData currentSave = await SaveManager.Instance.LoadGameAsync() ?? new SaveData();
            var characterProgress = currentSave.characterProgresses.Find(cp => cp.characterName == this.characterName);
            if (characterProgress != null)
            {
                characterProgress = GetSaveData();
            }
            else
            {
                currentSave.characterProgresses.Add(GetSaveData());
            }
            await SaveManager.Instance.SaveGameAsync(currentSave);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        Debug.Log($"{characterName} ist gestorben!");

        if (CompareTag("Enemy"))
        {
            BattleManager1.instance.OnEnemyDefeated(gameObject);
        }
        else if (CompareTag("Player"))
        {
            Debug.Log("Spieler ist gestorben!");
        }

        Destroy(gameObject);
    }

    public async Task SavePlayerPosition()
    {
        if (CompareTag("Player"))
        {
            SaveData currentSave = await SaveManager.Instance.LoadGameAsync() ?? new SaveData();
            currentSave.playerPosition = new SaveData.SerializableVector3(transform.position);

            // Aktualisiere oder füge den CharacterProgress hinzu
            var characterProgress = currentSave.characterProgresses.Find(cp => cp.characterName == this.characterName);
            if (characterProgress != null)
            {
                characterProgress = GetSaveData();
            }
            else
            {
                currentSave.characterProgresses.Add(GetSaveData());
            }

            await SaveManager.Instance.SaveGameAsync(currentSave);
            Debug.Log($"Spielerposition und Daten für {characterName} gespeichert: {transform.position}");
        }
    }


    public async Task LoadPlayerPosition()
    {
        if (CompareTag("Player"))
        {
            SaveData saveData = await SaveManager.Instance.LoadGameAsync();
            if (saveData != null)
            {
                Vector3 loadedPosition = saveData.GetPlayerPosition();
                if (loadedPosition != Vector3.zero)
                {
                    transform.position = loadedPosition;
                    Debug.Log($"Spielerposition geladen: {transform.position}");
                }
                else
                {
                    Debug.LogWarning("Keine gültige Spielerposition in den Speicherdaten gefunden.");
                }
            }
            else
            {
                Debug.LogWarning("Keine Speicherdaten gefunden.");
            }
        }
    }
}




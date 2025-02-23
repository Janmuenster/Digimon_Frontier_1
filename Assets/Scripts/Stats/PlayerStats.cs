using System;
using UnityEngine;

[System.Serializable]
public class PlayerStats : CharacterStats
{

    public int experience;
    public int experienceToNextLevel;

    // Zusätzliche Daten für die Overworld-Position
    public float positionX;
    public float positionY;
    public string overworldSceneName;

    public void SavePosition(Vector2 position, string sceneName)
    {
        positionX = position.x;
        positionY = position.y;
        overworldSceneName = sceneName;
    }

    public event Action OnLevelUp;

    protected override void Awake()
    {
        base.Awake();
        experienceToNextLevel = CalculateExperienceNeeded();
    }

    // Erfahrungspunkte sammeln
    private bool xpAwarded = false;
    public void GainExperience(int amount)
    {
        if (!xpAwarded)
        {
            experience += amount;
            CheckLevelUp();
            xpAwarded = true; // Verhindert doppelte Vergabe
        }
    }

    private void CheckLevelUp()
    {
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
            experience -= experienceToNextLevel;
            experienceToNextLevel = CalculateExperienceNeeded();
        }
    }

    public void LevelUp()
    {
        level++;
        maxHealth += UnityEngine.Random.Range(2, 5);
        currentHealth = maxHealth;
        attack += UnityEngine.Random.Range(1, 3);
        baseDefense += UnityEngine.Random.Range(1, 3);
        currentDefense = baseDefense;

        Debug.Log($"{unitName} ist auf Level {level} aufgestiegen!");

        OnLevelUp?.Invoke();
    }

    private int CalculateExperienceNeeded()
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(1.2f, level)); // XP wächst exponentiell
    }

    protected override void AssignBaseStats()
    {
        switch (gameObject.tag)
        {
            case "Takuya":
                unitName = "Takuya";
                level = 1;
                experience = 0;
                maxHealth = 10;
                attack = 2;
                baseDefense = 1;
                digimonType = DigimonType.Free;
                elementType = ElementType.Free;
                break;

            case "Zoe":
                unitName = "Zoe";
                level = 1;
                experience = 0;
                maxHealth = 11;
                attack = 2;
                baseDefense = 1;
                digimonType = DigimonType.Free;
                elementType = ElementType.Free;
                break;

            default:
                Debug.LogWarning("Unbekannter Spieler-Tag!");
                break;
        }

        currentHealth = maxHealth;
        currentDefense = baseDefense;
    }

    // Methoden für das Speichersystem
    public void SavePosition(Vector3 position, string sceneName)
    {
        positionX = position.x;
        positionY = position.y;
        overworldSceneName = sceneName;
    }

    public Vector3 LoadPosition()
    {
        return new Vector3(positionX, positionY, 0);
    }

    public new void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}

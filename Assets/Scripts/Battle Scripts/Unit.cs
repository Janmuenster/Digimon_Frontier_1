using UnityEngine;

public class Unit : MonoBehaviour
{
    public SharedCharacterData sharedData;
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    // Spieler-spezifische Variablen
    private bool isPlayer;
    public int currentXP;
    public int xpToNextLevel;

    // Gegner-spezifische Variable
    public int xpReward;

    // Konstante für die XP-Steigerung pro Level
    private const float XP_INCREASE_FACTOR = 1.5f;

    void Start()
    {
        isPlayer = CompareTag("Player");
        InitializeUnit();
    }

    void InitializeUnit()
    {
        if (isPlayer)
        {
            unitLevel = 1;
            currentXP = 0;
            xpToNextLevel = 100; // Startwert für Level 1
            
        }
        else
        {
            // Für Gegner: XP-Belohnung basierend auf Level setzen
            xpReward = 50 + (unitLevel - 1) * 10;
        }
    }
    public void UpdatePlayerStats()
    {
        if (isPlayer)
        {
            // Basiswerte für Level 1
            int baseMaxHP = maxHP;
            int baseDamage = damage;

            // Berechnung der Steigerung pro Level
            float hpIncreasePerLevel = baseMaxHP * 0.1f; // 10% Steigerung pro Level
            float damageIncreasePerLevel = baseDamage * 0.05f; // 5% Steigerung pro Level

            // Aktualisierung der Werte basierend auf dem aktuellen Level
            maxHP = Mathf.RoundToInt(baseMaxHP + hpIncreasePerLevel * (unitLevel - 1));
            damage = Mathf.RoundToInt(baseDamage + damageIncreasePerLevel * (unitLevel - 1));

            // Optional: Vollständige Heilung bei Statistik-Update
            currentHP = maxHP;
        }
    }


    public void UpdateSharedData()
    {
        sharedData.level = unitLevel;
        sharedData.currentXP = currentXP;
        sharedData.xpToNextLevel = xpToNextLevel;
    }

    public void SyncWithSharedData()
    {
        unitLevel = sharedData.level;
        currentXP += sharedData.currentXP;
        xpToNextLevel = sharedData.xpToNextLevel;
        UpdatePlayerStats();
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
            return true;
        else
            return false;
    }

    // Nur für Spieler: Erfahrungspunkte hinzufügen
    public void AddExperience(int xp)
    {
        if (!isPlayer) return;

        currentXP += xp;
        CheckLevelUp();
    }

    // Nur für Spieler: Level-Up überprüfen
    private void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    // Nur für Spieler: Level-Up durchführen
    private void LevelUp()
    {
        unitLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * XP_INCREASE_FACTOR);
      
        Debug.Log($"{unitName} ist auf Level {unitLevel} aufgestiegen!");
    }

    // Nur für Spieler: Statistiken aktualisieren

    // Für Gegner: XP-Belohnung abrufen
    public int GetXPReward()
    {
        return isPlayer ? 0 : xpReward;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

//public enum DigimonType
//{
  //  Virus,
    //Vaccine,
   // Data,
    //Free
//}

//public enum ElementType
//{
  //  Fire,
   // Water,
    //Wind,
   // Earth,
   // Light,
    //Darkness,
    //Ice,
    //Electricity,
   // Plant,
//    Metal,
  //  Free
//}

public class UnitStats : MonoBehaviour
{

    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
    public int level;
    public int experience;
    public int experienceToNextLevel;
    public DigimonType digimonType;
    public ElementType elementType; // Neues Feld für das Element
    public Image typeIcon;
    public Image elementIcon; // Neues Image für das Element-Symbol

    public DigimonAndElementIcons iconManager;

    public event Action OnLevelUp;
    public void CopyStatsFrom(UnitStats other)
    {
        if (other != null)
        {
            this.unitName = other.unitName;
            this.maxHealth = other.maxHealth;
            this.currentHealth = other.currentHealth;
            this.attack = other.attack;
            this.defense = other.defense;
            this.level = other.level;
            this.digimonType = other.digimonType;
            this.elementType = other.elementType; // Kopieren des Elements
            UpdateTypeIcon();
            UpdateElementIcon(); // Aktualisieren des Element-Icons
        }
        else
        {
            Debug.LogWarning("Versuch, Stats von einem null-Objekt zu kopieren.");
        }
    }
    public void GainExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp() 
    {
        // Implementieren Sie hier Ihre Levelaufstiegslogik
        int experienceNeeded = CalculateExperienceNeeded();
        if (experience >= experienceNeeded)
        {
            LevelUp();
            experience -= experienceNeeded;
            CheckLevelUp(); // Überprüfen Sie erneut, falls mehrere Level aufgestiegen wurden
        }
    }

public void LevelUp()
    {
        level++;
        maxHealth += UnityEngine.Random.Range(2, 5);
        currentHealth = maxHealth;
        attack += UnityEngine.Random.Range(1, 3);
        defense += UnityEngine.Random.Range(1, 3);
        Debug.Log($"{unitName} ist auf Level {level} aufgestiegen!");
        // Erhöhen Sie hier die Statuswerte
        experience -= experienceToNextLevel;
        experienceToNextLevel = CalculateExperienceNeeded();
        OnLevelUp?.Invoke();
    }
    private int CalculateExperienceNeeded()
    {
        // Einfache Formel für benötigte EP: Level * 100
        return level * 100;
    }

    private void Awake()
    {
        AssignBaseStats();
        Debug.Log($"UnitStats initialized for {gameObject.name}. Name: {unitName}, Health: {currentHealth}/{maxHealth}, Type: {digimonType}, Element: {elementType}");
    }

    private void Start()
    {
        
        iconManager = FindObjectOfType<DigimonAndElementIcons>(); // Initialisierung des Element-Icon-Managers
        UpdateTypeIcon();
        UpdateElementIcon(); // Aktualisieren des Element-Icons
    }
    
    private void AssignBaseStats()
    {
        switch (gameObject.tag)
        {
            case "Takuya":
                unitName = "Takuya";
                level = 1;
                experience = 0;
                experienceToNextLevel = CalculateExperienceNeeded();
                maxHealth = 10;
                currentHealth = maxHealth;
                attack = 1;
                defense = 1;
                digimonType = DigimonType.Free;
                elementType = ElementType.Free; // Zuweisung des Elements
                break;
            case "Cerberusmon":
                unitName = "Cerberusmon";
                level = 3;
                maxHealth = 14;
                currentHealth = maxHealth;
                attack = 2;
                defense = 2;
                digimonType = DigimonType.Virus;
                elementType = ElementType.Darkness; // Zuweisung des Elements
                break;
            case "Zoe":
                unitName = "Zoe";
                level = 1;
                maxHealth = 11;
                currentHealth = maxHealth;
                attack = 1;
                defense = 1;
                digimonType = DigimonType.Free;
                elementType = ElementType.Free; // Zuweisung des Elements
                break;
        }
        UpdateTypeIcon();
        UpdateElementIcon(); // Aktualisieren des Element-Icons
    }

    private void UpdateTypeIcon()
    {
        if (typeIcon != null && iconManager != null)
        {
            typeIcon.sprite = iconManager.GetSpriteForType(digimonType);
        }
    }

    private void UpdateElementIcon()
    {
        if (elementIcon != null && iconManager != null)
        {
            elementIcon.sprite = iconManager.GetSpriteForElement(elementType);
        }
    }
}

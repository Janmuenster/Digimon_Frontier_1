using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int baseDefense;
    public int currentDefense;
    public int level;

    public DigimonType digimonType;
    public ElementType elementType;

    public Image typeIcon;
    public Image elementIcon;

    public DigimonAndElementIcons iconManager;

    protected virtual void Awake()
    {
        AssignBaseStats();
        currentHealth = maxHealth;
        currentDefense = baseDefense;
        Debug.Log($"CharacterStats initialized for {gameObject.name}. Name: {unitName}, Health: {currentHealth}/{maxHealth}, Type: {digimonType}, Element: {elementType}");
    }

    protected virtual void Start()
    {
        iconManager = FindObjectOfType<DigimonAndElementIcons>();
        UpdateTypeIcon();
        UpdateElementIcon();
    }

    public virtual void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage - currentDefense, 0);
        currentHealth = Mathf.Max(currentHealth - actualDamage, 0);
        Debug.Log($"{unitName} nimmt {actualDamage} Schaden. Verbleibende HP: {currentHealth}/{maxHealth}");
    }

    public virtual void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{unitName} wird um {amount} geheilt. Aktuelle HP: {currentHealth}/{maxHealth}");
    }

    public virtual void ApplyDefense(int amount)
    {
        currentDefense += amount;
        Debug.Log($"{unitName} erhöht die Verteidigung um {amount}. Aktuelle Verteidigung: {currentDefense}");
    }

    protected virtual void AssignBaseStats()
    {
        Debug.LogWarning("AssignBaseStats wurde nicht in der abgeleiteten Klasse implementiert.");
    }

    protected void UpdateTypeIcon()
    {
        if (typeIcon != null && iconManager != null)
        {
            typeIcon.sprite = iconManager.GetSpriteForType(digimonType);
        }
    }

    protected void UpdateElementIcon()
    {
        if (elementIcon != null && iconManager != null)
        {
            elementIcon.sprite = iconManager.GetSpriteForElement(elementType);
        }
    }

}

using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName;
    public int baseDamage;
    public ElementType element;
    public AttackType type; // Normal oder Spezial

    public Attack(string name, int damage, ElementType element, AttackType type)
    {
        attackName = name;
        baseDamage = damage;
        this.element = element;
        this.type = type;
    }
}

public enum ElementType { Free, Fire, Water, Earth, Air, Electric, Plant, Metal, Ice, Light, Darkness }
public enum AttackType { Normal, Special }

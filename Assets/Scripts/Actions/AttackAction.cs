using UnityEngine;

public class AttackAction : IAction
{
    public string Name => "Attack";

    public void Execute(CharacterStats user, CharacterStats target)
    {
        int damage = Mathf.Max(user.attack - target.baseDefense, 0);
        target.TakeDamage(damage);
        Debug.Log($"{user.unitName} attacks {target.unitName} for {damage} damage!");
    }

    public void Execute(EnemyStats enemyStats, PlayerStats playerStats)
    {
        int damage = Mathf.Max(enemyStats.attack - playerStats.baseDefense, 0);
        playerStats.TakeDamage(damage);
        Debug.Log($"{enemyStats.unitName} attacks {playerStats.unitName} for {damage} damage!");
    }

    public float GetDamage() => 0; // Schaden wird dynamisch berechnet
    public ElementType GetElementType() => ElementType.Free; // Kein spezifisches Element
    public bool IsAreaOfEffect() => false;
    public int GetManaCost() => 0; // Kein Mana-Verbrauch
    public float GetCooldown() => 0; // Keine Abklingzeit
    public bool CanExecute(CharacterStats user) => true; // Kann immer ausgeführt werden
    public void PlayEffects(Vector3 position) { /* Implementieren Sie hier Effekte */ }
}

public class DefendAction : IAction
{
    public string Name => "Defend";

    public void Execute(CharacterStats user, CharacterStats target)
    {
        user.ApplyDefense(1); // Erhöht die Verteidigung um 1
        Debug.Log($"{user.unitName} defends and increases their defense!");
    }

    public void Execute(EnemyStats enemyStats, PlayerStats playerStats)
    {
        enemyStats.ApplyDefense(1); // Erhöht die Verteidigung um 1
        Debug.Log($"{enemyStats.unitName} defends and increases their defense!");
    }

    public float GetDamage() => 0; // Kein Schaden
    public ElementType GetElementType() => ElementType.Free;
    public bool IsAreaOfEffect() => false;
    public int GetManaCost() => 0;
    public float GetCooldown() => 0;
    public bool CanExecute(CharacterStats user) => true;
    public void PlayEffects(Vector3 position) { /* Implementieren Sie hier Effekte */ }
}

public class HealAction : IAction
{
    public string Name => "Heal";

    public void Execute(CharacterStats user, CharacterStats target)
    {
        int healAmount = Mathf.RoundToInt(user.maxHealth * 0.2f); // Heilt 20% der maximalen Gesundheit
        user.Heal(healAmount);
        Debug.Log($"{user.unitName} heals for {healAmount} HP!");
    }

    public void Execute(EnemyStats enemyStats, PlayerStats playerStats)
    {
        int healAmount = Mathf.RoundToInt(enemyStats.maxHealth * 0.2f);
        enemyStats.Heal(healAmount);
        Debug.Log($"{enemyStats.unitName} heals for {healAmount} HP!");
    }

    public float GetDamage() => 0; // Kein Schaden
    public ElementType GetElementType() => ElementType.Light; // Heilung könnte als Licht-Element betrachtet werden
    public bool IsAreaOfEffect() => false;
    public int GetManaCost() => 20; // Beispiel: 20 Mana-Kosten
    public float GetCooldown() => 3f; // Beispiel: 3 Sekunden Abklingzeit
    public bool CanExecute(CharacterStats user) => user.currentHealth < user.maxHealth; // Nur ausführbar, wenn nicht volle Gesundheit
    public void PlayEffects(Vector3 position) { /* Implementieren Sie hier Heilungseffekte */ }
}

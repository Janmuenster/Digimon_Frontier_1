using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public int experienceReward; // Erfahrungspunkte, die der Spieler bei Sieg erh�lt
    public List<IAction> actions = new List<IAction>(); // Liste m�glicher Angriffe/Aktionen des Gegners

    protected override void Awake()
    {
        base.Awake();
        AssignBaseStats();
        InitializeActions();
    }

    protected override void AssignBaseStats()
    {
        switch (gameObject.tag)
        {
            case "Cerberusmon":
                unitName = "Cerberusmon";
                level = 3;
                maxHealth = 14;
                attack = 2;
                baseDefense = 2;
                digimonType = DigimonType.Virus;
                elementType = ElementType.Darkness;
                experienceReward = 50; // Gibt dem Spieler z.B. 50 EP bei Sieg
                break;

            default:
                Debug.LogWarning("Unbekannter Gegner-Tag!");
                break;
        }

        currentHealth = maxHealth; // Setze die aktuellen HP auf das Maximum
    }

    private void InitializeActions()
    {
        actions.Add(new AttackAction());
        actions.Add(new DefendAction());

        // F�ge Heal-Aktion nur f�r bestimmte Gegner hinzu
        if (unitName == "Cerberusmon")
        {
            actions.Add(new HealAction());
        }
    }

    // Gegner w�hlt zuf�llig eine Aktion aus der Liste aus
    public IAction ChooseAction()
    {
        if (actions == null || actions.Count == 0)
        {
            Debug.LogWarning($"{unitName} has no actions!");
            return null;
        }

        return actions[Random.Range(0, actions.Count)];
    }
}

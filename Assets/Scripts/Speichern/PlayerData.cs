using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int attack;
    public int currentHealth;
    public int baseDefense;
    public int experience;
    public float positionX;
    public float positionY;
    public string overworldSceneName;

    public PlayerData(PlayerStats player)
    {
        level = player.level;
        currentHealth = player.currentHealth;
        attack = player.attack;
        baseDefense = player.baseDefense;
        experience = player.experience;
        positionX = player.positionX;
        positionY = player.positionY;
        overworldSceneName = player.overworldSceneName;
    }
}

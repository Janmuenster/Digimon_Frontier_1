using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float posX;
    public float posY;
    public float posZ;
    public string sceneName;
    public List<string> enemiesToDestroy = new List<string>(); // Geändert in eine Liste

    public string characterName;
    public int level;
    public int maxHP;
    public int currentHP;
    public int attack;
    public int defense;
    public string element;
    public string type;
    public int xp;
    public int xpToNextLevel;
    public bool isDigitized;

    // Getter & Setter für die Position
    public Vector3 GetPlayerPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    public void SetPlayerPosition(Vector3 position)
    {
        posX = position.x;
        posY = position.y;
        posZ = position.z;
    }
}

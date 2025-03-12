using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int version = 1;
    public DateTime saveTimestamp;
    public string sceneName;
    public SerializableVector3 playerPosition;
    public CharacterProgress characterProgress;
    public float totalPlayTime;
    public List<string> destroyedEnemies = new List<string>();

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    [System.Serializable]
    public class CharacterProgress
    {
        public string characterName;
        public int level;
        public int currentHP;
        public int maxHP;
        public int attack;
        public int defense;
        public int speed;
        public int attackPower;
        public string element;
        public string characterType;
        public int xp;
        public int xpToNextLevel;
        public bool isDigitized;
        public int currentDigivolutionIndex;
    }

    public SaveData()
    {
        saveTimestamp = DateTime.Now;
        characterProgress = new CharacterProgress();
    }

    public void SetPlayerPosition(Vector3 position)
    {
        playerPosition = new SerializableVector3(position);
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition.ToVector3();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance;

    public List<GameObject> allCharactersPrefabs; // Alle Charakter-Prefabs
    public List<GameObject> battleParty; // Die 3 aktiven Kämpfer

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBattleParty(List<GameObject> selectedCharacters)
    {
        battleParty = new List<GameObject>(selectedCharacters);
    }

    public void AddCharacterToParty(GameObject newCharacter)
    {
        if (!allCharactersPrefabs.Contains(newCharacter))
        {
            allCharactersPrefabs.Add(newCharacter);
        }
    }
}

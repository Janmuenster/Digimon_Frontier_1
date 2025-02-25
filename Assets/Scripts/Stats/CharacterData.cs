using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int startLevel;
    public int startMaxHP;
    public int startAttack;
    public int startDefense;
    public string element;
    public string type;

    [Header("Digitation Stats")]
    public Digivolution[] digivolutions; // Array für mehrere Digitationen
}

[System.Serializable]
public class Digivolution
{
    public string digiName;    // Name der Digitation
    public string digiElement; // Element der Digitation
    public string digiType;    // Typ der Digitation
}

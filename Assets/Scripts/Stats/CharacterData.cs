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
    public string digiElement;
    public string digiType;
}

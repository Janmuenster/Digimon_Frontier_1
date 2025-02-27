using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpBar;

    private Character characterData;

    public void SetCharacter(Character character)
    {
        characterData = character;
        nameText.text = character.characterName;
        hpBar.maxValue = character.maxHP;
        hpBar.value = character.currentHP;
    }

    public void UpdateHP()
    {
        hpBar.value = characterData.currentHP;
    }
}

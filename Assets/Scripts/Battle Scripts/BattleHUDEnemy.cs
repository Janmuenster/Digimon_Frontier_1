using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUDEnemy : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider hpSlider;
    public Image typeImage;
    public Image elementImage;

    public DigimonAndElementIcons iconManager; // Referenz auf das kombinierte Skript

    private EnemyStats currentUnit; // Geändert zu EnemyStats

    // Setzt die HUD-Werte für den Gegner
    public void SetHUD(EnemyStats unit) // Geändert zu EnemyStats
    {
        currentUnit = unit;

        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.level;
        hpSlider.maxValue = unit.maxHealth;
        hpSlider.value = unit.currentHealth;

        // Setzen Sie das Typ-Sprite mit Hilfe des kombinierten Skripts
        Sprite typeSprite = iconManager.GetSpriteForType(unit.digimonType);
        if (typeSprite != null)
        {
            typeImage.sprite = typeSprite;
            typeImage.enabled = true;
        }
        else
        {
            typeImage.enabled = false;
        }

        // Setzen Sie das Element-Sprite mit Hilfe des kombinierten Skripts
        Sprite elementSprite = iconManager.GetSpriteForElement(unit.elementType);
        if (elementSprite != null)
        {
            elementImage.sprite = elementSprite;
            elementImage.enabled = true;
        }
        else
        {
            elementImage.enabled = false;
        }
    }

    // Aktualisiert die HP im HUD
    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Slider hpSlider;
    public Slider expSlider;
    public Image typeImage;
    public Image elementImage;

    public DigimonAndElementIcons iconManager; // Referenz auf das kombinierte Skript

    private PlayerStats currentUnit; // Geändert zu PlayerStats
    private Coroutine expFillCoroutine;

    public float expFillDuration = 1f; // Dauer in Sekunden für das Auffüllen des EXP-Sliders
    public void SetHUD(PlayerStats unit) // Geändert zu PlayerStats
    {
        currentUnit = unit;
        currentUnit.OnLevelUp += UpdateHUDOnLevelUp;

        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.level;
        hpSlider.maxValue = unit.maxHealth;
        hpSlider.value = unit.currentHealth;
        expSlider.maxValue = unit.experienceToNextLevel;
        expSlider.value = unit.experience;

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
    public void SetEXP(int exp)
    {
        if (expFillCoroutine != null)
        {
            StopCoroutine(expFillCoroutine);
        }
        expFillCoroutine = StartCoroutine(FillEXPSlider(exp));
    }

    private IEnumerator FillEXPSlider(int targetExp)
    {
        float startExp = expSlider.value; // Der aktuelle Wert des Sliders
        float elapsedTime = 0f;

        // Phase 1: Fülle den Slider bis zum Zielwert oder Max-Wert
        while (elapsedTime < expFillDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / expFillDuration;
            expSlider.value = Mathf.Lerp(startExp, Mathf.Min(targetExp, expSlider.maxValue), t);
            yield return null;
        }

        expSlider.value = Mathf.Min(targetExp, expSlider.maxValue); // Sicherstellen, dass der Slider genau beim Zielwert endet

        // Wenn der Zielwert den Maximalwert erreicht oder überschreitet
        if (targetExp >= expSlider.maxValue)
        {
            // Warte kurz, bevor der Slider zurückgesetzt wird
            yield return new WaitForSeconds(0.2f);

            // Phase 2: Setze den Slider auf 0 und fülle ihn mit dem verbleibenden XP
            int overflowExp = targetExp - (int)expSlider.maxValue; // Berechne überschüssige XP
            currentUnit.LevelUp(); // Level-Up durchführen
            expSlider.value = 0; // Slider zurücksetzen
            expSlider.maxValue = currentUnit.experienceToNextLevel; // Neues Max setzen

            if (overflowExp > 0) // Nur auffüllen, wenn es überschüssige XP gibt
            {
                elapsedTime = 0f;
                while (elapsedTime < expFillDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / expFillDuration;
                    expSlider.value = Mathf.Lerp(0, overflowExp, t);
                    yield return null;
                }

                expSlider.value = overflowExp; // Sicherstellen, dass der Slider den Zielwert erreicht
            }
        }
    }

    private void UpdateHUDOnLevelUp()
    {
        levelText.text = "Lvl " + currentUnit.level;
        hpSlider.maxValue = currentUnit.maxHealth;
        hpSlider.value = currentUnit.currentHealth;

        // Aktualisieren Sie den EXP-Slider sofort beim Level-Up
        expSlider.maxValue = currentUnit.experienceToNextLevel;
        SetEXP(currentUnit.experience);
    }
    private void OnDisable()
    {
        if (currentUnit != null)
        {
            currentUnit.OnLevelUp -= UpdateHUDOnLevelUp;
        }
    }
    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}

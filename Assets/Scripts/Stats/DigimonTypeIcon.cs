using UnityEngine;

public class DigimonAndElementIcons : MonoBehaviour
{
    [Header("Digimon Type Sprites")]
    public Sprite virusSprite;
    public Sprite vaccineSprite;
    public Sprite dataSprite;
    public Sprite freeSprite;

    [Header("Element Type Sprites")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite windSprite;
    public Sprite earthSprite;
    public Sprite IceSprite;
    public Sprite ElectricitySprite;
    public Sprite MetalSprite;
    public Sprite PlantSprite;
    public Sprite lightSprite;
    public Sprite darknessSprite;
    public Sprite FreeSprite;

    public Sprite GetSpriteForType(DigimonType type)
    {
        switch (type)
        {
            case DigimonType.Virus:
                return virusSprite;
            case DigimonType.Vaccine:
                return vaccineSprite;
            case DigimonType.Data:
                return dataSprite;
            case DigimonType.Free:
                return freeSprite;
            default:
                return null;
        }
    }

    public Sprite GetSpriteForElement(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return fireSprite;
            case ElementType.Water:
                return waterSprite;
            case ElementType.Wind:
                return windSprite;
            case ElementType.Earth:
                return earthSprite;
            case ElementType.Light:
                return lightSprite;
            case ElementType.Darkness:
                return darknessSprite;
            case ElementType.Ice:
                return IceSprite;
            case ElementType.Plant:
                return PlantSprite;
            case ElementType.Free:
                return FreeSprite;
            case ElementType.Electricity:
                return ElectricitySprite;
            case ElementType.Metal:
                return MetalSprite;
            default:
                return null;
        }
    }
}

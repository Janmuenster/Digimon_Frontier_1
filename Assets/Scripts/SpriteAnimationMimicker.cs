using UnityEngine;
using UnityEngine.UI;

// @kurtdekker
// intended to copy the .sprite field from an animated
// SpriteRenderer over to a UI.Image field.

public class SpriteAnimationMimicker : MonoBehaviour
{
    [Header("FROM: The SpriteRenderer being animated.")]
    public SpriteRenderer spriteRenderer;

    [Header("TO: The target UI.Image you want to update.")]
    public Image image;

    void LateUpdate()
    {
        image.sprite = spriteRenderer.sprite;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject replacementObject;
    public string animationName;
    public Button button;

    private Animator animator;
    private bool animationPlayed = false;

    void Start()
    {
        animator = originalObject.GetComponent<Animator>();
        button.onClick.AddListener(PlayAnimationAndReplace);
    }

    void PlayAnimationAndReplace()
    {
        if (!animationPlayed)
        {
            animator.Play(animationName);
            animationPlayed = true;

            // Warte auf das Ende der Animation, bevor das Objekt ersetzt wird
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("ReplaceObject", animationLength);
        }
    }

    void ReplaceObject()
    {
        originalObject.SetActive(false);
        replacementObject.SetActive(true);
        button.interactable = false; // Optional: Deaktiviert den Button nach einmaliger Benutzung
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenewechsel : MonoBehaviour
{
    public Image fadeImage; // Das UI-Panel für den Fade-Effekt
    public float delayBeforeChange = 5f; // Verzögerung vor dem Szenenwechsel in Sekunden
    public int nextSceneIndex = 1; // Index der nächsten Szene

    void Start()
    {
        // Deaktiviere das Fade-Image zu Beginn
        fadeImage.gameObject.SetActive(false);
        StartCoroutine(AutoChangeScene());
    }

    private IEnumerator AutoChangeScene()
    {
        // Warte für die angegebene Verzögerung
        yield return new WaitForSeconds(delayBeforeChange);

        // Starte den Szenenwechsel mit Fade-Effekt
        StartCoroutine(LoadSceneWithFade(nextSceneIndex));
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        // Aktiviere das Fade-Image
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0); // Setze die Anfangsfarbe auf vollständig transparent

        // Fade-Out
        yield return StartCoroutine(FadeOut());

        // Lade die neue Szene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade-In
        yield return StartCoroutine(FadeIn());

        // Deaktiviere das Fade-Image nach dem Fade-In
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        float duration = 1f; // Dauer des Fades
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / duration));
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float duration = 1f; // Dauer des Fades
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(1 - (elapsedTime / duration)));
            yield return null;
        }
    }
}

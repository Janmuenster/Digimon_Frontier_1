using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characters;
    private int currentCharacterIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        // Speichere die Position des aktuellen Charakters
        Vector3 currentPosition = characters[currentCharacterIndex].transform.position;

        // Deaktiviere den aktuellen Charakter
        characters[currentCharacterIndex].SetActive(false);

        // Wechsle zum nächsten Charakter
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;

        // Setze die Position des neuen Charakters
        characters[currentCharacterIndex].transform.position = currentPosition;

        // Aktiviere den neuen Charakter
        characters[currentCharacterIndex].SetActive(true);
    }
}

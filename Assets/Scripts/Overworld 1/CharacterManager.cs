using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characters; // Liste aller spielbaren Charaktere
    private int activeCharacterIndex = 0; // Aktueller Charakter

    void Start()
    {
        // Stelle sicher, dass nur der erste Charakter aktiv ist
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == activeCharacterIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Vorheriger Charakter
        {
            SwitchCharacter(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Nächster Charakter
        {
            SwitchCharacter(1);
        }
    }

    void SwitchCharacter(int direction)
    {
        // Speichere die aktuelle Position
        Vector3 currentPosition = characters[activeCharacterIndex].transform.position;

        // Deaktiviere den aktuellen Charakter
        characters[activeCharacterIndex].SetActive(false);

        // Wechsel zum neuen Charakter (loop durch Array)
        activeCharacterIndex += direction;
        if (activeCharacterIndex < 0) activeCharacterIndex = characters.Length - 1;
        if (activeCharacterIndex >= characters.Length) activeCharacterIndex = 0;

        // Setze neue Position und aktiviere neuen Charakter
        characters[activeCharacterIndex].transform.position = currentPosition;
        characters[activeCharacterIndex].SetActive(true);
    }
}

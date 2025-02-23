using UnityEngine;

public class CharacterSyncManager : MonoBehaviour
{
    public Unit character1;
    public Unit character2;
    public SharedCharacterData sharedData;

    void Update()
    {
        if (character1.sharedData.currentXP != sharedData.currentXP ||
            character2.sharedData.currentXP != sharedData.currentXP)
        {
            SyncCharacters();
        }
    }

    void SyncCharacters()
    {
        if (character1.sharedData.currentXP > sharedData.currentXP)
        {
            character1.UpdateSharedData();
            sharedData = character1.sharedData;
        }
        else if (character2.sharedData.currentXP > sharedData.currentXP)
        {
            character2.UpdateSharedData();
            sharedData = character2.sharedData;
        }

        character1.SyncWithSharedData();
        character2.SyncWithSharedData();
    }
}

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance; // Singleton f�r einfachen Zugriff
    public Vector2 lastPosition;
    public string lastScene;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
             // �berlebt Szenenwechsel
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePosition(Vector2 pos, string sceneName)
    {
        lastPosition = pos;
        lastScene = sceneName;
        Debug.Log("Gespeichert: " + lastPosition + " in Szene " + lastScene);
    }
}

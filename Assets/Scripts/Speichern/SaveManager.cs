using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private string savePath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/savegame.dat";
    }

    public void SaveGame(SaveData data)
    {
        try
        {
            using (FileStream file = new FileStream(savePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, data);
            }

            Debug.Log("Spiel gespeichert! Position: " + data.GetPlayerPosition());
        }
        catch (Exception ex)
        {
            Debug.LogError("Fehler beim Speichern: " + ex.Message);
        }
    }

    public bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Kein Speicherstand gefunden!");
            return null;
        }

        try
        {
            using (FileStream file = new FileStream(savePath, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                SaveData data = (SaveData)bf.Deserialize(file);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Fehler beim Laden des Spiels: " + e.Message);
            return null;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Spielstand gelöscht!");
        }
    }
}

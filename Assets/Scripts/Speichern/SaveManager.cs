using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private string saveFileName = "savegame.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private const int CurrentVersion = 1;
    private const string EncryptionKey = "YourSecretKey123"; // Ändere dies zu einem sicheren Schlüssel

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<bool> SaveGameAsync(SaveData data)
    {
        try
        {
            data.version = CurrentVersion;
            data.saveTimestamp = DateTime.Now;

            string json = JsonUtility.ToJson(data);
            string encrypted = Encrypt(json);

            await File.WriteAllTextAsync(SavePath, encrypted);
            CreateBackup();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fehler beim Speichern: {ex.Message}");
            return false;
        }
    }

    public async Task<SaveData> LoadGameAsync()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Kein Speicherstand gefunden.");
            return null;
        }

        try
        {
            string encrypted = await File.ReadAllTextAsync(SavePath);
            string json = Decrypt(encrypted);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data.version > CurrentVersion)
            {
                Debug.LogError("Speicherstand ist von einer neueren Version.");
                return null;
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fehler beim Laden: {ex.Message}");
            return null;
        }
    }

    private string Encrypt(string data)
    {
        byte[] encryptedBytes;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = new byte[16]; // Für Einfachheit, in Produktion sollte ein zufälliger IV verwendet werden

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }
                encryptedBytes = ms.ToArray();
            }
        }
        return Convert.ToBase64String(encryptedBytes);
    }

    private string Decrypt(string encryptedData)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
        string decryptedData;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(encryptedBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                decryptedData = sr.ReadToEnd();
            }
        }
        return decryptedData;
    }

    private void CreateBackup()
    {
        string backupPath = SavePath + ".bak";
        File.Copy(SavePath, backupPath, true);
    }
}

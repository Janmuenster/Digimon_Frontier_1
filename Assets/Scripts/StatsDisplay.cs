using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private string targetTag; // Tag des Ziels, dessen Stats angezeigt werden sollen

    private PlayerStats targetStats;

    private void Start()
    {
        // Finde das GameObject mit dem spezifizierten Tag
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            targetStats = targetObject.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogWarning($"Kein GameObject mit dem Tag '{targetTag}' gefunden.");
        }

        UpdateStatsDisplay();
    }

    private void Update()
    {
        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
        if (statsText != null && targetStats != null)
        {
            statsText.text = $"Name: {targetStats.unitName}\n" +
                             $"HP: {targetStats.currentHealth}/{targetStats.maxHealth}\n" +
                             $"Lv. {targetStats.level}\n" +
                             $"Attack: {targetStats.attack}\n" +
                             $"Defense: {targetStats.baseDefense}";
        }
    }
}

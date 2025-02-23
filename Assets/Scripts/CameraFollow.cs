using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public string[] targetTags; // Array von Tags, denen die Kamera folgen soll

    private List<Transform> targets = new List<Transform>();

    void Start()
    {
        FindTargets();
    }

    void FindTargets()
    {
        targets.Clear();
        foreach (string tag in targetTags)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in taggedObjects)
            {
                targets.Add(obj.transform);
            }
        }

        if (targets.Count == 0)
        {
            Debug.LogWarning("Keine Objekte mit den angegebenen Tags gefunden!");
        }
    }

    void Update()
    {
        if (targets.Count > 0)
        {
            Vector3 averagePos = GetAveragePosition();
            Vector3 newPos = new Vector3(averagePos.x, averagePos.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }

    Vector3 GetAveragePosition()
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in targets)
        {
            sum += target.position;
        }
        return sum / targets.Count;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad; // Name der zu ladenden Szene
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Takuya"))
        {
            LoadNextScene();
        }
        if (other.CompareTag("Zoe"))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

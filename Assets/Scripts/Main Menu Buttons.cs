using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
  
    public void Home()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void Exit()
    {
         #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1; 
    }

}

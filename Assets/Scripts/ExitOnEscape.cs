using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitOnEscape : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the main menu scene to load when escape is pressed")]
    public string mainMenuSceneName = "MainMenu";
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

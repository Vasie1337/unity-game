using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuMng : MonoBehaviour
{
    [Header("Main Menu UI")]
    public Button[] levelButtons;
    public Button exitBtn;

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex + 1}");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int capturedIndex = i;
            levelButtons[i].onClick.AddListener(() => LoadLevel(capturedIndex));
        }

        exitBtn.onClick.AddListener(OnClickExit);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Level1";      // the scene your game is in
    [SerializeField] string menuSceneName = "MainMenu";    // the scene that holds the menu
    [SerializeField] GameObject welcomePanel;              // optional: the "welcome" panel
    [SerializeField] GameObject mainMenuPanel;             // optional: a second panel for the main menu

    void Start()
    {
        Time.timeScale = 1f;                               // the menu must never start paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowMainMenu()
    {
        // With a second panel: switch panels. Without one: go back to the menu scene.
        if (mainMenuPanel != null)
        {
            if (welcomePanel != null) welcomePanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // stops Play mode in the Editor
#else
        Application.Quit();                                // closes the built game
#endif
    }
}
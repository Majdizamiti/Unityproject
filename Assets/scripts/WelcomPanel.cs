using UnityEngine;
using UnityEngine.EventSystems;

public class WelcomePanel : MonoBehaviour
{
    [SerializeField] GameObject panel;                 // your welcome panel
    [SerializeField] MonoBehaviour playerController;   // Remy's PlayerControls component (optional)
    [SerializeField] bool showEveryTime = false;  
    [SerializeField] UnityEngine.UI.Button startButton;   // drag your Start button
    [SerializeField] UnityEngine.UI.Button exitButton;    // drag your Exit button     

    static bool alreadyShown;

    // Resets the flag each time you press Play in the Editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetFlag() { alreadyShown = false; }

    void Start()
    {
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (panel == null)
        {
            Debug.LogError("WelcomePanel: Panel is not assigned!", this);
            return;
        }

        if (FindFirstObjectByType<EventSystem>() == null)
            Debug.LogError("WelcomePanel: no EventSystem in the scene, so buttons can't be clicked. Use GameObject > UI > Event System.");

        if (alreadyShown && !showEveryTime)
        {
            panel.SetActive(false);
            return;
        }

        alreadyShown = true;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();            // draws over the score, health bar, etc.

        Time.timeScale = 0f;                           // everything waits behind the panel
        AudioListener.pause = true;                    // music waits too
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerController != null) playerController.enabled = false;
    }

    public void StartGame()
    {
        Debug.Log("WelcomePanel: Start clicked");
        panel.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (playerController != null) playerController.enabled = true;
    }

    public void ExitGame()
    {
        Debug.Log("WelcomePanel: Exit clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
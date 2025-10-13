using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;     // PauseMenuPanel
    public Button restartButton;
    public Button levelSelectButton;
    public Button mainMenuButton;

    bool isPaused;

    void Awake()
    {
        if (panel) panel.SetActive(false);

        // Optional: wire buttons here so prefabs are self-contained
        if (restartButton)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }
        if (levelSelectButton)
        {
            levelSelectButton.onClick.RemoveAllListeners();
            levelSelectButton.onClick.AddListener(() => LoadSceneByName("LevelSelect"));
        }
        if (mainMenuButton)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() => LoadSceneByName("MainMenu"));
        }
    }

    void Update()
    {
        // Escape toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) Pause();
            else Resume();
        }
    }

    public void Pause()
    {
        isPaused = true;
        if (panel) panel.SetActive(true);
        Time.timeScale = 0f;
        // Optional: set selected button for keyboard/controller
        // UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(restartButton?.gameObject);
    }

    public void Resume()
    {
        isPaused = false;
        if (panel) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}

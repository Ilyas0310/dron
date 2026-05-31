using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject pausePanel;       // Сама менюшка паузы
    public GameObject settingsPanel;    // Панель с настройками кнопок (KeybindManager)
    
    [Header("Защита от конфликтов")]
    public GameObject modeSelectionPanel; // Панель выбора дрона при старте

    private bool isPaused = false;

    void Start()
    {
        // При старте убеждаемся, что меню паузы и настройки спрятаны
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void Update()
    {
        // 1. ЗАЩИТА: Если мы сейчас выбираем режим дрона на старте - кнопка ESC не работает
        if (modeSelectionPanel != null && modeSelectionPanel.activeSelf) return;

        // 2. ЗАЩИТА: Если открыта панель победы - ESC не работает
        if (VictoryManager.instance != null && VictoryManager.instance.victoryPanel != null)
        {
            if (VictoryManager.instance.victoryPanel.activeSelf) return;
        }

        // Если нажали Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если мы сейчас находимся ВНУТРИ настроек, то ESC просто вернет нас в меню паузы
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                // Иначе просто включаем/выключаем паузу
                TogglePause();
            }
        }
        if (isPaused && Input.GetKeyDown(KeyCode.F8))
        {
            if (AchievementManager.instance != null)
            {
                AchievementManager.instance.ResetOnlyAchievements();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // СТАВИМ НА ПАУЗУ
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            
            // Показываем мышку
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // ВОЗВРАЩАЕМСЯ В ИГРУ
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            
            // Прячем мышку (если в твоей игре нужно летать мышкой)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Эти методы мы повесим на кнопки в UI:
    
    public void ResumeGame()
    {
        if (isPaused) TogglePause();
    }

    public void RestartMode()
    {
        // Обязательно возвращаем время в норму перед перезагрузкой
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры...");
        // Эта команда закроет игру после того, как ты её сбилдишь (в самом редакторе Unity она не сработает)
        Application.Quit(); 
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    // Синглтон - дает доступ к скрипту без поиска по сцене
    public static VictoryManager instance;

    [Header("Интерфейс")]
    public GameObject victoryPanel;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f; // Гарантируем, что время идет
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void FinishLevel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // Останавливаем мир
            
            // Показываем мышку, чтобы нажать кнопку в меню
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Этот метод повесь на кнопку "Restart" на WinPanel
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
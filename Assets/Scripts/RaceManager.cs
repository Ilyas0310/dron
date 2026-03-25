using UnityEngine;
using TMPro; // Подключаем библиотеку для текста
using UnityEngine.SceneManagement; // Чтобы работала загрузка сцен

public class RaceManager : MonoBehaviour
{
    [Header("Настройки трассы")]
    public CheckpointTrigger[] allCheckpoints; // Список всех ворот по порядку

    [Header("Интерфейс")]
    public TextMeshProUGUI timerText; // Текст таймера в углу
    public GameObject winPanel;       // Панель победы
    public TextMeshProUGUI finalTimeText; // Текст времени на панели победы

    private int currentCheckpointIndex = 0; // Какой чекпоинт следующий?
    private float currentTime = 0f;
    private bool isRacing = false;

    void Start()
    {
        currentCheckpointIndex = 0;
        currentTime = 0f;
        isRacing = true; // Гонка начинается сразу (можно потом переделать на отсчет 3-2-1)
        
        if(winPanel != null) winPanel.SetActive(false); // Скрываем победную панель
    }

    void Update()
    {
        if (isRacing)
        {
            currentTime += Time.deltaTime;
            
            // Обновляем таймер на экране (формат 00:00.00)
            if(timerText != null)
                timerText.text = FormatTime(currentTime);
        }
    }

    public void PlayerPassedCheckpoint(CheckpointTrigger checkpoint)
    {
        // Если гонка уже закончена, игнорируем
        if (!isRacing) return;

        // Проверяем, правильный ли это чекпоинт по порядку
        if (checkpoint == allCheckpoints[currentCheckpointIndex])
        {
            Debug.Log($"Чекпоинт {currentCheckpointIndex} пройден!");

            // --- ВОТ ЭТА НОВАЯ СТРОЧКА ---
            // Выключаем (скрываем) пройденный чекпоинт
            checkpoint.gameObject.SetActive(false); 
            // -----------------------------

            currentCheckpointIndex++; // Ожидаем следующий

            // Если это был последний чекпоинт
            if (currentCheckpointIndex >= allCheckpoints.Length)
            {
                FinishRace();
            }
        }
        else
        {
            Debug.Log("Не тот чекпоинт! Ищи нужный.");
        }
    }

    void FinishRace()
    {
        isRacing = false;
        Debug.Log("ФИНИШ!");
        
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            finalTimeText.text = "Время: " + FormatTime(currentTime);
            FindFirstObjectByType<DroneController>().enabled = false;
        }
    }

    // Вспомогательная функция для красивого отображения времени
    string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu"); // Убедись, что твоя сцена меню называется именно "Menu"
    }
}
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class RaceManager : MonoBehaviour
{
    [Header("Настройки трассы")]
    public CheckpointTrigger[] allCheckpoints; 

    [Header("Интерфейс")]
    public TextMeshProUGUI timerText; 
    public GameObject winPanel;       
    public TextMeshProUGUI finalTimeText; 

    [Header("Лидерборд (Новое)")]
    public TextMeshProUGUI leaderboardText; // Сюда перетащи объект текста рекордов на панели
    private const int MaxLeaderboardEntries = 3; // Храним Топ-3 результата

    private int currentCheckpointIndex = 0; 
    private float currentTime = 0f;
    private bool isRacing = false;

    void Start()
    {
        currentCheckpointIndex = 0;
        currentTime = 0f;
        isRacing = true; 
        
        if(winPanel != null) winPanel.SetActive(false); 
        
        // Сразу编новляем UI лидерборда (на случай если панель победы включена в редакторе)
        UpdateLeaderboardUI();
    }

    void Update()
    {
        if (isRacing)
        {
            currentTime += Time.deltaTime;
            
            if(timerText != null)
                timerText.text = FormatTime(currentTime);
        }
    }

    public void PlayerPassedCheckpoint(CheckpointTrigger checkpoint)
    {
        if (!isRacing) return;

        if (checkpoint == allCheckpoints[currentCheckpointIndex])
        {
            Debug.Log($"Чекпоинт {currentCheckpointIndex} пройден!");
            checkpoint.gameObject.SetActive(false); 

            currentCheckpointIndex++; 

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
        
        // 1. Проверяем и сохраняем новый результат в таблицу рекордов
        SaveToLeaderboard(currentTime);

        // === ВЫДАЕМ НОВУЮ АЧИВКУ ЗА ПОБЕДУ В ГОНКЕ! ===
        if (AchievementManager.instance != null)
        {
            AchievementManager.instance.UnlockAchievement(
                "first_race_win", 
                "Молния", 
                "Впервые успешно завершите гоночную трассу по чекпоинтам."
            );
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            finalTimeText.text = "Время: " + FormatTime(currentTime);
            
            // 2. Обновляем текст таблицы на экране
            UpdateLeaderboardUI();

            // ИСПРАВЛЕНИЕ: Безопасно отключаем управление у любого заспавненного дрона (Acro или Стандарт)
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                DroneController dc = playerObj.GetComponent<DroneController>();
                if (dc != null) dc.enabled = false;

                AcroDroneController adc = playerObj.GetComponent<AcroDroneController>();
                if (adc != null) adc.enabled = false;
            }

            // Показываем курсор для взаимодействия с меню победы
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Алгоритм сортировки и сохранения рекордов
    private void SaveToLeaderboard(float newTime)
    {
        // Загружаем текущий топ (9999f используется как пустой слот)
        float[] bestTimes = new float[MaxLeaderboardEntries];
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            bestTimes[i] = PlayerPrefs.GetFloat("race_time_" + i, 9999f);
        }

        // Ищем место для нового результата (в гонках чем меньше время, тем лучше)
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            if (newTime < bestTimes[i])
            {
                // Сдвигаем старые рекорды ниже по списку
                for (int j = MaxLeaderboardEntries - 1; j > i; j--)
                {
                    bestTimes[j] = bestTimes[j - 1];
                }
                
                // Вставляем новый рекорд на законное место
                bestTimes[i] = newTime;
                break;
            }
        }

        // Записываем обновленный топ обратно в память
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            PlayerPrefs.SetFloat("race_time_" + i, bestTimes[i]);
        }
        PlayerPrefs.Save();
    }

    // Обновление текстового блока на панели
    private void UpdateLeaderboardUI()
    {
        if (leaderboardText == null) return;

        string textToShow = "ТАБЛИЦА РЕКОРДОВ:\n";
        
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            float savedTime = PlayerPrefs.GetFloat("race_time_" + i, 9999f);
            
            // Если в слоте лежит реальное время (меньше заглушки)
            if (savedTime < 9998f)
            {
                textToShow += $"{i + 1}. {FormatTime(savedTime)}\n";
            }
            else
            {
                textToShow += $"{i + 1}. --:--.--\n";
            }
        }

        leaderboardText.text = textToShow;
    }

    // МЕТОД ДЛЯ СБРОСА: Удаляет только рекорды гонки, не ломая настройки кнопок
    public void ResetLeaderboard()
    {
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            PlayerPrefs.DeleteKey("race_time_" + i);
        }
        PlayerPrefs.Save();
        
        Debug.Log("Лидерборд гонки сброшен.");
        UpdateLeaderboardUI(); // Сразу перерисовываем пустую таблицу
    }

    string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // На случай если игра была на паузе
        SceneManager.LoadScene("Menu"); 
    }
}
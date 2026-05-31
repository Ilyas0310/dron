using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("Список всех ID достижений для сброса")]
    public string[] allAchievementIDs = new string[] { "first_target", "first_crash", "moving_landing", "first_follow_win",
    "first_race_win", "first_chase_win"};

    [Header("UI Элементы")]
    public GameObject achievementPanel; // Сама всплывающая панель
    public TextMeshProUGUI titleText;   // Текст названия ачивки
    public TextMeshProUGUI descText;    // Текст описания

    [Header("Настройки")]
    public float displayTime = 3f;      // Сколько секунд висит плашка на экране

    private bool isDisplaying = false;  // Флаг, чтобы ачивки не перекрывали друг друга

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (achievementPanel != null) 
        {
            achievementPanel.SetActive(false);
        }
    }

    // Главный метод. Вызываем его из любого места игры!
    public void UnlockAchievement(string id, string title, string description)
    {
        // Проверяем в реестре: если значение == 1, значит ачивка уже была получена ранее
        if (PlayerPrefs.GetInt(id, 0) == 1)
        {
            return; // Выходим, не даем ачивку второй раз
        }

        // Сохраняем информацию о получении в память компьютера
        PlayerPrefs.SetInt(id, 1);
        PlayerPrefs.Save();

        Debug.Log($"[АЧИВКА ПОЛУЧЕНА]: {title} - {description}");

        // Запускаем процесс показа UI
        StartCoroutine(ShowAchievementRoutine(title, description));
    }

    // Корутина для показа плашки на определенное время
    private IEnumerator ShowAchievementRoutine(string title, string description)
    {
        // Если сейчас уже показывается другая ачивка, ждем пока она пропадет
        while (isDisplaying)
        {
            yield return new WaitForSeconds(1f);
        }

        isDisplaying = true;

        if (achievementPanel != null)
        {
            titleText.text = title;
            descText.text = description;
            achievementPanel.SetActive(true);

            // Ждем указанное время (displayTime)
            yield return new WaitForSeconds(displayTime);

            achievementPanel.SetActive(false);
        }

        isDisplaying = false;
    }
    
    // Метод для тестов: сбросить все ачивки (можно повесить на секретную кнопку в меню)
    public void ResetOnlyAchievements()
    {
        foreach (string id in allAchievementIDs)
        {
            // Удаляем из памяти только ключи ачивок
            if (PlayerPrefs.HasKey(id))
            {
                PlayerPrefs.DeleteKey(id);
            }
        }
    
        PlayerPrefs.Save(); // Сохраняем изменения на диск
        Debug.Log("Все достижения успешно сброшены! Настройки управления не тронуты.");
    }
}
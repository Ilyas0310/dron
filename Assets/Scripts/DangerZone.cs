using UnityEngine;
using TMPro; // Для работы с текстом
using UnityEngine.SceneManagement; // Для перезагрузки уровня при смерти

public class DangerZone : MonoBehaviour
{
    [Header("Настройки зоны")]
    public float timeToKill = 3f; // Сколько секунд дается на побег

    [Header("Интерфейс")]
    public TextMeshProUGUI warningText; // Сюда перетащим наш красный текст

    private bool isPlayerInZone = false;
    private float timer;

    void Start()
    {
        // Убедимся, что при старте текст выключен
        if (warningText != null) 
        {
            warningText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Если игрок внутри зоны, запускаем обратный отсчет
        if (isPlayerInZone)
        {
            timer -= Time.deltaTime; // Отнимаем время кадра

            // Обновляем текст на экране (F1 означает 1 знак после запятой)
            if (warningText != null)
            {
                warningText.text = "ВЫ В ОПАСНОЙ ЗОНЕ!\nУничтожение через: " + timer.ToString("F1");
            }

            // Если время вышло — убиваем игрока
            if (timer <= 0f)
            {
                KillPlayer();
            }
        }
    }

    // Срабатывает, когда кто-то ВЛЕТАЕТ в зону
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это именно наш дрон
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            timer = timeToKill; // Сбрасываем таймер на максимум
            
            if (warningText != null) warningText.gameObject.SetActive(true); // Включаем текст
        }
    }

    // Срабатывает, когда кто-то ВЫЛЕТАЕТ ИЗ зоны
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            
            if (warningText != null) warningText.gameObject.SetActive(false); // Прячем текст
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Дрон уничтожен!");
        // Самый простой способ "убить" — перезагрузить текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
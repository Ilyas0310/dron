using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private RaceManager raceManager;

    private void Start()
    {
        // Находим менеджера гонки на сцене автоматически
        raceManager = FindFirstObjectByType<RaceManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. ОТЛАДКА: Пишем имя всего, что коснулось триггера
        Debug.Log("В чекпоинт влетел: " + other.name); 

        // 2. Старая проверка
        if (other.CompareTag("Player"))
        {
            raceManager.PlayerPassedCheckpoint(this);
        }
    }
}
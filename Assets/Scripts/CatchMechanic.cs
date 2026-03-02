using UnityEngine;

public class CatchMechanic : MonoBehaviour
{
    private ChaseGameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<ChaseGameManager>();
    }

    // Срабатывает при физическом ударе двух объектов (без галочки Is Trigger)
    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, врезался ли в нас Игрок
        if (collision.gameObject.CompareTag("Player"))
        {
            // Вызываем победу
            if (gameManager != null) gameManager.WinGame();
            
            // Уничтожаем врага (чтобы пропал)
            Destroy(gameObject); 
        }
    }
}